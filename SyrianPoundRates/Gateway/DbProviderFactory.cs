using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web.Configuration;

namespace SyrianPoundRates.Gateway
{
    public class DbProviderFactory
    {       
        private static string _currentDbName = string.Empty;
        private static DbProviderFactory _instance;

        private readonly ConnectionStringSettings _settings;
        private readonly System.Data.Common.DbProviderFactory _frameworkProviderFactory;

        private DbProviderFactory(ConnectionStringSettings settings)
        {
            _frameworkProviderFactory = DbProviderFactories.GetFactory(settings.ProviderName);
            _settings = settings;
        }

        #region IDbProviderFactory Members

        public string ProviderName
        {
            get { return _settings.ProviderName; }
        }

        public IDbConnection CreateConnection()
        {
            IDbConnection connection = _frameworkProviderFactory.CreateConnection();
            if (connection == null) throw new ApplicationException("Failed to create connection");
            connection.ConnectionString = _settings.ConnectionString;
            return connection;
        }

        public IDbCommand CreateCommand()
        {
            return _frameworkProviderFactory.CreateCommand();
        }

        #endregion

        public static DbProviderFactory GetInstance(string dbConnectionName)
        {
            if (_instance != null && _currentDbName == dbConnectionName) 
                return _instance;

            var connectionStrings = GetDbConnection(dbConnectionName);
            _instance = new DbProviderFactory(connectionStrings);            
            _currentDbName = dbConnectionName;
            return _instance;
        }

        private static ConnectionStringSettings GetDbConnection(string dbName)
        {
            var configConnStringSettings = WebConfigurationManager.ConnectionStrings[dbName];
            if (configConnStringSettings == null || string.IsNullOrEmpty(configConnStringSettings.ConnectionString)) 
               throw new ApplicationException(string.Format("({0}) was not found in the AppSettings section of .Config file.", dbName));
            return configConnStringSettings; 
        }



              
    }
}