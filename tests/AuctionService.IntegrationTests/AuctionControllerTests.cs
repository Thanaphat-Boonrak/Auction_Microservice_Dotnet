using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace AuctionService.IntegrationTests;

[Collection("Shared Collection")]
public class AuctionControllerTests
    :   IAsyncLifetime
{
    private readonly CustomeWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

    public AuctionControllerTests(CustomeWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuctions_ShouldReturn3Auctions()
    {
        var response = await _client.GetFromJsonAsync<List<AuctionDto>>("/api/auctions");
        Assert.Equal(3, response.Count);
    }


    [Fact]
    public async Task GetAuctionsById_WithValidId_ShouldReturnAuctions()
    {
        var response = await _client.GetFromJsonAsync<AuctionDto>($"/api/auctions/{GT_ID}");
        Assert.Equal("GT", response.Model);
    }

    [Fact]
    public async Task GetAuctionsById_WithInValidId_ShouldReturn400()
    {
        var response = await _client.GetAsync($"/api/auctions/test123");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task GetAuctionsById_WithInValidIdShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/auctions/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        var auctionDto = new CreateAuctionDto() { Make = "test" };
        var response = await _client.PostAsJsonAsync("/api/auctions", auctionDto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        var auctionDto = GetCreateAuctionDto();
        _client.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        var response = await _client.PostAsJsonAsync("api/auctions", auctionDto);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }


    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        var auctionDto = GetCreateAuctionDto();
        auctionDto.Make = null;
        _client.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        var response = await _client.PostAsJsonAsync("api/auctions", auctionDto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        var auctionDto = GetCreateAuctionDto();
        _client.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        var response = await _client.PutAsJsonAsync($"api/auctions/{GT_ID}", auctionDto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        var auctionDto = GetCreateAuctionDto();
        _client.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("test"));
        auctionDto.Make = null;
        var response = await _client.PutAsJsonAsync($"api/auctions/{GT_ID}", auctionDto);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }


    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReintialDbForTests(db);
        return Task.CompletedTask;
    }


    private CreateAuctionDto GetCreateAuctionDto()
    {
        return new CreateAuctionDto()
        {
            Make = "test",
            Model = "test",
            ImageUrl = "test",
            Color = "test",
            Mileage = 10,
            Year = 10,
            reservePrice = 20000,
            AuctionEnd = new DateTime().ToUniversalTime().AddDays(10),
        };
    }
}