using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SyrianPoundRates.Gateway
{
    public class DbGateway : IDbGateway
    {       
        /// <summary>
        /// This method is mainly for testing. 
        /// we should not be using inline sql. 
        /// </summary>
        /// <param name="sqlStatemenet">Sql statement to execute.</param>
        /// <param name="dbConnectionName">Name of the database connection in the .config file</param>
        /// <returns>DataTable instance</returns>
        /// <remarks>Mehod consumer must dispose the returned DataTable instance.</remarks>
        public DataTable ExecuteStatement(string sqlStatemenet, string dbConnectionName = "DefaultConnection")
        {
            var rawData = new DataTable();
            using (IDbConnection connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sqlStatemenet;
                command.CommandType = CommandType.Text;
                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    rawData.Load(reader);
                }
            }
            return rawData;
        }


        /// <summary>
        /// Executes a string query with parameters
        /// </summary>
        /// <param name="sqlStatemenet">The sql query string</param>
        /// <param name="parameters">List of parameters included int the <paramref name="sqlStatemenet"/></param>
        /// <param name="dbConnectionName">Name of the database connection in the .config file</param>
        /// <returns>DataTable instance</returns>
        /// <remarks>Mehod consumer must dispose the returned DataTable instance.</remarks>
        public DataTable ExecuteStatement(string sqlStatemenet, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection")
        {
            var rawData = new DataTable();
            using (IDbConnection connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sqlStatemenet;
                command.CommandType = CommandType.Text;

                foreach (DbGatewayParameter parameter in parameters)
                {
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = parameter.ParameterName;
                    param.Value = parameter.Value;
                    param.DbType = parameter.DbType;
                    param.Direction = parameter.Direction;
                    command.Parameters.Add(param);
                }

                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    rawData.Load(reader);
                }
            }
            return rawData;
        }

        /// <summary>
        /// Execute the specified stored procedure and returns the results
        /// </summary>        
        /// <param name="storedProcedure">Name of the stored procedure to execute</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <param name="dbConnectionName">Name of the database connection in the .config file</param>
        /// <returns>DataTable instance</returns>
        /// <remarks>Mehod consumer must dispose the returned DataTable instance.</remarks>
        public DataTable ExecuteStoredProcedure(string storedProcedure, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection")
        {
            var rawData = new DataTable();
            using (IDbConnection connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DbGatewayParameter parameter in parameters)
                {
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = parameter.ParameterName;
                    param.Value = parameter.Value;
                    param.DbType = parameter.DbType;
                    param.Direction = parameter.Direction;
                    command.Parameters.Add(param);
                }

                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    rawData.Load(reader, LoadOption.OverwriteChanges);
                }
            }
            return rawData;
        }


        /// <summary>
        /// Executes a stored procedure that returns a mulitple results set and loads them into a DataSet. 
        /// </summary>
        /// <param name="storedProcedure">Name of the stored procedure to execute</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <param name="numOfTblesInResultSet">Number of tables returned from the sql result set.</param> 
        /// <param name="dbConnectionName">Name of the database connection in the .config file</param>       
        /// <returns>DataSet with mulitple tables</returns>
        /// <remarks>Mehod consumer must dispose the returned DataSet instance.</remarks>
        public DataSet ExecuteStoredProcedure(string storedProcedure, IList<DbGatewayParameter> parameters, int numOfTblesInResultSet, string dbConnectionName = "DefaultConnection")
        {
            var ds = new DataSet();
            using (IDbConnection connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DbGatewayParameter parameter in parameters)
                {
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = parameter.ParameterName;
                    param.Value = parameter.Value;
                    param.DbType = parameter.DbType;
                    param.Direction = parameter.Direction;
                    command.Parameters.Add(param);
                }

                connection.Open();

                using (IDataReader reader = command.ExecuteReader())
                {
                    var tables = new List<string>();
                    for (int i = 1; i <= numOfTblesInResultSet; i++)
                    {
                        tables.Add("Table" + i);
                    }
                    ds.Load(reader, LoadOption.OverwriteChanges, tables.ToArray());
                }
            }
            return ds;
        }


        /// <summary>
        /// This method is not made as a part of IDbGateway interface because
        /// it is a specific implementation of SqlClient. 
        /// Return is not significat, it's only added so the mothed can mock tested.        
        /// </summary>
        /// <param name="tableName">Database table to preform bulk insert on.</param>
        /// <param name="bulkData">Datatble to inserted in the Database</param>
        /// <param name="batchSize">bulk batch size</param>
        /// <param name="dbConnectionName">Name of the database connection in the .config file</param>
        /// <returns>Return value will be always zero. Return was only added for mocking purposes.</returns>
        public int ExecuteBulkSqlServerInsert(string tableName, DataTable bulkData, int batchSize, string dbConnectionName = "DefaultConnection")
        {
            using (var connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection() as SqlConnection)
            {
                if (connection == null)
                {
                    throw new Exception("Unable to create connection to sqlServer.");
                }
                using (var bulkCopy = new SqlBulkCopy(connection.ConnectionString,
                                                              SqlBulkCopyOptions.UseInternalTransaction))
                {
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = tableName;
                    foreach (DataColumn dataColum in bulkData.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(dataColum.ColumnName, dataColum.ColumnName); 
                    }
                    connection.Open();
                    bulkCopy.WriteToServer(bulkData);
                }
            }
            return 0;
        }


        /// <summary>
        /// Execute Insert, Update or Delete on the specified Database. 
        /// If insert is execute the method returns the new row Id,
        /// else it returns the number of effected rows. 
        /// </summary>
        /// <param name="storedProcedure">Name of the stored procedure to execute</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <param name="dbConnectionName">Name of the database connection in the .config file</param>
        /// <returns>new row id/number of rows updated</returns>
        public int ExecuteInsertDelete(string storedProcedure, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection")
        {
            int rowId;           
            using (IDbConnection connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DbGatewayParameter parameter in parameters)
                {
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = parameter.ParameterName;
                    param.Value = parameter.Value;
                    if (param.DbType != DbType.Object)
                    {
                        param.DbType = parameter.DbType;
                    }
                    param.Direction = parameter.Direction;
                    command.Parameters.Add(param);
                }
                connection.Open();
                rowId = command.ExecuteNonQuery();
                IDbDataParameter outputParam = GetOutPutParameter(command.Parameters);
                if (outputParam != null)
                {
                    rowId = (int)outputParam.Value;
                }
            }
            return rowId;
        }

        public DateTime ExecuteUpdate(string storedProcedure, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection")
        {
            DateTime timeStamp = DateTime.MinValue;   
            using (IDbConnection connection = DbProviderFactory.GetInstance(dbConnectionName).CreateConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DbGatewayParameter parameter in parameters)
                {
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = parameter.ParameterName;
                    param.Value = parameter.Value;
                    param.Direction = parameter.Direction;
                    command.Parameters.Add(param);
                }
               // command.Parameters.Remove(command.Parameters["@UpdatedOn"]);
                //var lastChanged = new SqlParameter("@newUpdatedOn", SqlDbType.DateTime);
                //lastChanged.Direction = ParameterDirection.Output;
                //command.Parameters.Add(lastChanged);
                connection.Open();
                command.ExecuteNonQuery();
                IDbDataParameter outputParam = GetOutPutParameter(command.Parameters);
                if (outputParam != null)
                {
                    timeStamp = (DateTime)outputParam.Value;
                }
            }
            return timeStamp;
        }
      

        #region helpers

        private static IDbDataParameter GetOutPutParameter(IDataParameterCollection parameters)
        {
            foreach (IDbDataParameter parameter in parameters)
            {
                if (parameter.Direction == ParameterDirection.Output)
                    return parameter;
            }
            return null;
        }
       

        #endregion
    }
}