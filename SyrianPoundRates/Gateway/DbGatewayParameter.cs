using System.Data;

namespace SyrianPoundRates.Gateway
{
    public class DbGatewayParameter
    {
        #region Constructors
        public DbGatewayParameter()
        {
        }

        public DbGatewayParameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value; 
        }

        public DbGatewayParameter(string parameterName, object value, DbType dbType)
        {
            ParameterName = parameterName;
            Value = value;
            DbType = dbType;             
        }



        public DbGatewayParameter(string parameterName, object defaultValue, DbType dbType, ParameterDirection direction)
        {
            ParameterName = parameterName;
            Value = defaultValue;
            DbType = dbType;           
            Direction = direction; 
        }
        #endregion

        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public string ParameterName { get; set; }

        public string SourceColumn { get; set; }

        public DataRowVersion SourceVersion { get; set; }

        public object Value { get; set; }

        public bool IsGet { get; set; }

        public bool IsInsertUpdate { get; set; }
    }
}
