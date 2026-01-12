using JasperFx;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Testcontainers.PostgreSql;

namespace BeerSender.Domain.Tests
{
    public class MartenFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer =
            new PostgreSqlBuilder()
                .WithImage("postgres:15-alpine")
                .WithDatabase("beersender_test_db")
                .WithUsername("postgres")
                .WithPassword("Marten123")
                .Build();

        private readonly string _schema = $"bstest{Guid.NewGuid():n}";
        private IHost? _host;

        public IDocumentStore Store { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddMarten(opt =>
                    {
                        opt.Connection(_dbContainer.GetConnectionString());
                        opt.DatabaseSchemaName = _schema;

                        opt.ApplyDomainConfig();
                        opt.AddProjections();
                    })
                    .AddAsyncDaemon(JasperFx.Events.Daemon.DaemonMode.Solo);
                }).Start();

            Store = _host.Services.GetRequiredService<IDocumentStore>();

            await CreateSchemaAsync();
        }

        public async Task DisposeAsync()
        {
            Store?.Dispose();
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }

        private async Task CreateSchemaAsync()
        {
            await using var connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"CREATE SCHEMA IF NOT EXISTS {_schema}";
            await command.ExecuteNonQueryAsync();
        }
    }
    [CollectionDefinition("Marten collection")]
    public class DatabaseConnection : ICollectionFixture<MartenFixture>;
}