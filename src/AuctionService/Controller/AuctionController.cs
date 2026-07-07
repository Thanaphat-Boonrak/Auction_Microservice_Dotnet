using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controller;

public class AuctionsController(IAuctionRepository context, IMapper mapper, IPublishEndpoint publishEndpoint)
    : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
    {
        return await context.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await context.GetAuctionByIdAsync(id);
        if (auction == null) return NotFound();
        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto dto)
    {
        var auctin = mapper.Map<Auction>(dto);
        auctin.Seller = User.Identity?.Name!;
        context.AddAuction(auctin);

        var newAuction = mapper.Map<AuctionDto>(auctin);
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));
        var result = await context.SaveChangesAsync();
        if (!result) return BadRequest("Problem saving changes");

        return CreatedAtAction(nameof(GetAuction), new { id = auctin.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.GetAuctionEntityById(id);

        if (auction == null) return NotFound();
        if (auction.Seller != User.Identity!.Name) return Forbid();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        await publishEndpoint.Publish(mapper.Map<UpdateAuction>(auction));

        var result = await context.SaveChangesAsync();
        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.GetAuctionEntityById(id);
        if (auction == null) return NotFound();
        if (auction.Seller != User.Identity!.Name) return Forbid();


        context.RemoveAuction(auction);

        await publishEndpoint.Publish(new AuctionDeleted { Id = id.ToString() });

        var result = await context.SaveChangesAsync();

        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }
}