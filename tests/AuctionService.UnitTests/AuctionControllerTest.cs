using AuctionService.Controller;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionControllerTest
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Fixture _fixture;
    private readonly AuctionsController _auctionsController;
    private readonly IMapper _mapper;


    public AuctionControllerTest()
    {
        _fixture = new Fixture();
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        var mockMapper = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(AuctionsController).Assembly); },
                NullLoggerFactory.Instance)
            .CreateMapper()
            .ConfigurationProvider;
        _mapper = new Mapper(mockMapper);
        _auctionsController =
            new AuctionsController(_auctionRepositoryMock.Object, _mapper, _publishEndpointMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
                }
            };
    }

    [Fact]
    public async Task GetAuctions_WithNoParams_Returns10Auctions()
    {
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);
        var result = await _auctionsController.GetAllAuctions(null);
        Assert.Equal(10, result.Value.Count);
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuctions_WithValidGuid_ReturnsAuction()
    {
        var auctions = _fixture.Create<AuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auctions);
        var result = await _auctionsController.GetAuction(auctions.Id);
        Assert.Equal(auctions.Make, result.Value.Make);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }

    [Fact]
    public async Task GetAuctions_WithInValidGuid_ReturnsNotFound()
    {
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
        var result = await _auctionsController.GetAuction(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result.Result);
    }


    [Fact]
    public async Task GetAuctions_WithValidCreateAuctionDto_ReturnsCreatedAtAuction()
    {
        var auctions = _fixture.Create<CreateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepositoryMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _auctionsController.CreateAuction(auctions);

        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdAtActionResult);
        Assert.Equal("GetAuction", createdAtActionResult.ActionName);
        Assert.IsType<AuctionDto>(createdAtActionResult.Value);
    }

    [Fact]
    public async Task CreateAuction_FailedSave_Returns400BadRequest()
    {
        var auctions = _fixture.Create<CreateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepositoryMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);
        var result = await _auctionsController.CreateAuction(auctions);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsOkResponse()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
        _auctionRepositoryMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _auctionsController.UpdateAuction(auction.Id, updateAuctionDto);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidUser_Returns403Forbid()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "test1";
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
        var result = await _auctionsController.UpdateAuction(auction.Id, updateAuctionDto);
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
    {
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
        var result = await _auctionsController.UpdateAuction(Guid.NewGuid(), null);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithValidUser_ReturnsOkResponse()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>())).ReturnsAsync(auction);
        _auctionRepositoryMock.Setup(repo => repo.RemoveAuction(It.IsAny<Auction>()));
        _auctionRepositoryMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        var result = await _auctionsController.DeleteAuction(auction.Id);
        Assert.IsType<OkResult>(result);
    }
}