using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Connection
{
    public enum ConnectionType
    {
        Oracle,
        MsSql
    }

    public interface IDbConnection
    {
        ConnectionType ConnectionType { get; set; }

        string ConnectingString { get; set; }

        DbConnection Connection { get; set; }

        IDbConnection SetConnectionString(string connectionString);

        IDbConnection SetConnection(DbConnection connection);

        IDbConnection Connect();

        IDbConnection Disconnect();
    }

    public abstract class CustomConnection : IDbConnection, IDisposable
    {
        public CustomConnection() { }

        public CustomConnection(string provider, string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
                SetConnectionString(connectionString);

            Connection = DbProviderFactories.GetFactory(provider)
                                            .CreateConnection();

            Connection.ConnectionString = ConnectingString;
        }

        public ConnectionType ConnectionType { get; set; }

        public string ConnectingString { get; set; }

        public DbConnection Connection { get; set; }

        public IDbConnection SetConnectionString(string connectionStringName)
        {
            try
            {
                ConnectingString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return this;
        }

        public IDbConnection SetConnection(DbConnection connection)
        {
            Connection = connection;

            return this;
        }

        public IDbConnection Connect()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            return this;
        }

        public IDbConnection Disconnect()
        {
            Connection.Close();

            return this;
        }

        public IDbCommand CreateCommand()
        {
            return Connection.CreateCommand();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }

    public class MsSqlConnection : CustomConnection
    {
        public MsSqlConnection(string provider = "System.Data.SqlClient", string connectionString = "MsSqlConnectionString")
            : base(provider, connectionString)
        {
            ConnectionType = ConnectionType.MsSql;
        }
    }
}
