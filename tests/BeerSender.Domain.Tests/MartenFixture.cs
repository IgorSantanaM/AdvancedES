using Marten;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.PostgreSql;

namespace BeerSender.Domain.Tests
{
    public class MartenFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer =
            new PostgreSqlBuilder()
                .WithDatabase("beersender_test_db")
                .WithUsername("postgres")
                .WithPassword("Marten123")
                .WithImage("postgres:15-alpine")
                .WithPortBinding(0, 5432)
                .Build();

        private readonly string schema = $"bstest{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
        public IDocumentStore Store { get; protected set; }

        public MartenFixture()
        {
            Store = DocumentStore.For(opt =>
            {
                opt.Connection(GetConnectionString());
                opt.DatabaseSchemaName = schema;
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE SCHEMA IF NOT EXISTS {schema}";
            command.ExecuteNonQuery();
            connection.Close();
        }

        public async Task DisposeAsync()
        {
            using var connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"DROP SCHEMA IF EXISTS{schema} CASCADE";
            command.ExecuteNonQuery();
            connection.Close();
            await _dbContainer.StopAsync();
            await  _dbContainer.DisposeAsync();
        }

        private string? _connectionString;

        private string GetConnectionString()
        {
            if (_connectionString != null)
                return _connectionString;

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = config.GetConnectionString("MartenDB");

            if (_connectionString == null)
                throw new Exception("ConnectionString unavailable");

            return _connectionString;
        }

    }
}
