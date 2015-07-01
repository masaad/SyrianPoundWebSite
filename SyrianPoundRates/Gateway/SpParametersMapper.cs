using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SyrianPoundRates.Gateway
{
    /// <summary>
    /// Used to as a flag for the stored procedure execute Add, Update, or Delete. 
    /// </summary>
    public enum SpActionType
    {
        Get = 1,
        InsertUpdate = 2        
    }

    public enum PropertyMapsTo
    {
        Get = 1,
        AddUpdate = 2
    }   

    public static class SpParametersMapper
    {
        /// <summary>
        /// Maps Object properties to insert or update Store Procedure parameters. 
        /// In order for this method to work properly, you must confirm to the following: 
        /// 1. Property to be mapped must be Public. 
        /// 2. Property must be decorated with [Mappable] attribute and 
        ///    the attribute value should be set to true. 
        /// 3. Stored Procedure parameters must match the names of your properties.       
        /// </summary>
        /// <param name="objectToMap">The object which properties you need to map</param>
        /// <param name="actionType">Stored Procedure Action Type</param>
        /// <returns>IList</returns>
        public static IList<DbGatewayParameter> MapToSpParameters(Object objectToMap, SpActionType actionType)
        {
            IList<DbGatewayParameter> parameters = new List<DbGatewayParameter>();
            if (objectToMap != null)
            {
                Type type = objectToMap.GetType();
                PropertyInfo[] propertiesInfo = type.GetProperties();
                foreach (PropertyInfo propertyInfo in propertiesInfo)
                {
                    DbGatewayParameter parameter = MapParameter(objectToMap, propertyInfo);
                    if (parameter != null && !HasParameter(parameters, parameter))
                        if (actionType == SpActionType.Get && parameter.IsGet)
                        {
                            parameters.Add(parameter);
                        }
                        else if (actionType == SpActionType.InsertUpdate && parameter.IsInsertUpdate)
                        {
                            parameters.Add(parameter);
                        }
                }
                //if (actionType == SpActionType.InsertUpdate)
                //{
                //    parameters.Add(new DbGatewayParameter("@ByUser", Thread.CurrentPrincipal.Identity.Name));
                //}
            }
            return parameters;
        }

        private static DbGatewayParameter MapParameter(object objectToMap, PropertyInfo propertyInfo)
        {
            DbGatewayParameter newParameter = null;
            object[] myAttribute = propertyInfo.GetCustomAttributes(false);
            foreach (object attr in myAttribute)
            {
                var isMappableAtt = attr as Mappable;
                if (isMappableAtt == null) continue;
                object mapValue = propertyInfo.GetValue(objectToMap, null);               
                if (mapValue == null) continue;
                // if is Nullable type and its value is null don't map it; 
                // Nullable Properties are defaulted to null in the stored procedure, therefore, 
                // no need to set their values since the defualt value will be used. 
               // if (propertyInfo.PropertyType.Name.Contains("Nullable") && mapValue == null) continue;
                if (mapValue is DateTime)
                {
                    var dt = (DateTime)mapValue;
                    if (dt == DateTime.MinValue)
                    {
                        mapValue = DBNull.Value;
                    }
                }
                else if (mapValue.GetType() == typeof(List<string>))
                {
                    var values = (List<string>)mapValue;
                    var strBld = new StringBuilder();
                    foreach (string str in values)
                    {
                        strBld.AppendFormat("{0}|", str);
                    }
                    mapValue = strBld.ToString().TrimEnd(',');
                }
                else if (mapValue.GetType().BaseType == typeof(Enum))
                {
                    mapValue = (int)mapValue;
                }
                string spParameterName = String.IsNullOrEmpty(isMappableAtt.SpParameter)
                                             ? propertyInfo.Name
                                             : isMappableAtt.SpParameter.Trim('@');
                newParameter =
                    new DbGatewayParameter("@" + spParameterName, mapValue,
                                           GetDbParameterType(propertyInfo.PropertyType));
                newParameter.IsGet = isMappableAtt.IsSpGetParameter;
                newParameter.IsInsertUpdate = isMappableAtt.IsSpInsertUpdateParameter;                
            }
            return newParameter;
        }

        /// <summary>
        /// Maps .NET type to DataBase type. 
        /// The list of types is not completed. 
        /// Please add to the list as needed. 
        /// </summary>
        /// <param name="propType"></param>
        /// <returns></returns>
        private static DbType GetDbParameterType(Type propType)
        {
            DbType dbType = DbType.Object;
            //ToDo: Add Types as needed. 
            switch (propType.Name)
            {
                case "Int32":             
                case "Enum":
                    dbType = DbType.Int32;
                    break;
                case "String":
                case "IList`1": // List of strings. 
                    dbType = DbType.String;
                    break;
                case "DateTime":
                    dbType = DbType.DateTime;
                    break;
                case "Nullable`1":
                    TypeConverter converter = TypeDescriptor.GetConverter(propType);
                    if (converter.CanConvertTo(typeof(int)))
                    {
                        dbType = DbType.Int32;
                    }
                    else if (converter.CanConvertTo(typeof(DateTime)))
                    {
                        dbType = DbType.DateTime;
                    }
                    else if (converter.CanConvertTo(typeof(bool)))
                    {
                        dbType = DbType.Boolean;
                    }
                    break;
                case "Byte[]":
                    dbType = DbType.Binary;
                    break;
                case "Boolean":
                    dbType = DbType.Boolean;
                    break;
                case "Single":
                    dbType = DbType.Single;
                    break;
                case "Guid":
                    dbType = DbType.Guid;
                    break;
            }
            if (dbType == DbType.Object &&
                propType.BaseType == typeof(Enum))
            {
                dbType = DbType.Int32;
            }
            return dbType;
        }

        private static bool HasParameter(IEnumerable<DbGatewayParameter> parameters, DbGatewayParameter parameter)
        {
            foreach (DbGatewayParameter dbGatewayParameter in parameters)
            {
                if (dbGatewayParameter.ParameterName == parameter.ParameterName)
                    return true;
            }
            return false;
        }
    }   
}