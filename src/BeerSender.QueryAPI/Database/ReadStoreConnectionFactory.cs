using Microsoft.Data.SqlClient;
using System.Data;

namespace BeerSender.QueryAPI.Database;

public class ReadStoreConnectionFactory(IConfiguration configuration)
{
    private readonly string? _connectionString
    = configuration.GetConnectionString("ReadStore");

    public IDbConnection Create()
        => new SqlConnection(_connectionString);
}
