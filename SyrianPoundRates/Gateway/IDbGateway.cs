using System;
using System.Collections.Generic;
using System.Data;

namespace SyrianPoundRates.Gateway
{
    public interface IDbGateway
    {
        DataTable ExecuteStatement(string sqlStatement, string dbConnectionName = "DefaultConnection");
        DataTable ExecuteStatement(string sqlStatemenet, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection");
        DataTable ExecuteStoredProcedure(string storedProcedure, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection");
        DataSet ExecuteStoredProcedure(string storedProcedure, IList<DbGatewayParameter> parameters, int numOfTblesInResultSet, string dbConnectionName = "DefaultConnection");
        int ExecuteBulkSqlServerInsert(string tableName, DataTable bulkData, int batchSize, string dbConnectionName = "DefaultConnection");
        int ExecuteInsertDelete(string storedProcedure, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection");
        DateTime ExecuteUpdate(string storedProcedure, IList<DbGatewayParameter> parameters, string dbConnectionName = "DefaultConnection");        
    }
}
