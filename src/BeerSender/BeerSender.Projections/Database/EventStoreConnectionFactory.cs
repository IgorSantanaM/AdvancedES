using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BeerSender.Projections.Database
{
    public class EventStoreConnectionFactory(IConfiguration configuration)
    {
        private readonly string? _eventStoreConnectionString
      = configuration.GetConnectionString("EventStore");

        public IDbConnection CreateConnection()
            => new SqlConnection(_eventStoreConnectionString);
    }
}
