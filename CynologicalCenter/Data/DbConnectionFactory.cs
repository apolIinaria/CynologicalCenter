using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace CynologicalCenter.Data
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;
        public DbConnectionFactory()
        {
            _connectionString = ConfigurationManager
                .AppSettings["ConnectionString"]!;
        }

        public MySqlConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}
