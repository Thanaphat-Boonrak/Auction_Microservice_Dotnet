using System.Runtime.CompilerServices;

namespace AuctionService.IntegrationTests.Fixtures;

[CollectionDefinition("Shared Collection")]
public class SharedFixture : ICollectionFixture<CustomeWebAppFactory>
{
}