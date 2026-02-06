using Xunit;

namespace Ksummarized.IntegrationTests;

[CollectionDefinition(CollectionName)]
public class IntegrationTestCollection : ICollectionFixture<PostgresContainerFixture>
{
    public const string CollectionName = "Integration";
}
