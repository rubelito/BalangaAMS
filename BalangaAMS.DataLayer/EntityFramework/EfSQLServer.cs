using System.Data.Common;
using System.Data.SqlClient;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.DataLayer.EntityFramework
{
    public class EfSQLServer : IDatabaseType
    {
        private readonly string _connectionName;

        public EfSQLServer(string connectionName)
        {
            _connectionName = connectionName;
        }

        public DbConnection Connectionstring()
        {
            return
                new SqlConnection(System.Configuration.ConfigurationManager
                    .ConnectionStrings[_connectionName].ConnectionString);
        }
    }
}
