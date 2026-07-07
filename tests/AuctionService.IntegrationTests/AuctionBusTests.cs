using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Utils;
using Contracts;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared Collection")]
public class AuctionBusTests : IAsyncLifetime
{
    private readonly CustomeWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";
    private ITestHarness _harness;

    public AuctionBusTests(CustomeWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _harness = factory.Services.GetTestHarness();
    }

    [Fact]
    public async Task CreateAuction_WithValidObject_ShouldPublishedAuctionCreated()
    {
        var auctionDto = GetCreateAuctionDto();
        _client.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        var response = await _client.PostAsJsonAsync($"api/auctions", auctionDto);
        response.EnsureSuccessStatusCode();
        Assert.True(await _harness.Published.Any<AuctionCreated>());
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