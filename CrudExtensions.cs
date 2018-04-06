using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Crud
{
    public static class CrudExtensions
    {
        public static async Task<int> Insert(this SqlConnection con, string tableName, object parametersObject)
        {
            string sql = CreateInsertSqlStatement(tableName, parametersObject);
            sql += "SELECT SCOPE_IDENTITY();";

            return await con.QuerySingleAsync<int>(sql, parametersObject);
        }

        public static async Task InsertMulti(this SqlConnection con, string tableName, IEnumerable<object> parametersObjects, SqlTransaction transaction = null)
        {
            string sql = CreateInsertSqlStatement(tableName, parametersObjects.ElementAt(0));  

            if(transaction == null) 
            {
                await con.ExecuteAsync(sql, parametersObjects);
            }
            else {
                await con.ExecuteAsync(sql, parametersObjects, transaction);
            }
        }

        public static async Task Update(this SqlConnection con, string tableName, object identityParameters, object parametersObject)
        {
            IEnumerable<PropertyInfo> propertyInfo = parametersObject.GetType().GetProperties();
            IEnumerable<PropertyInfo> identityPropertyInfo = identityParameters.GetType().GetProperties();

            IEnumerable<string> columnsEqualToParameter = propertyInfo.Select(p => $"[{p.Name}] = @{p.Name}");
            IEnumerable<string> identityColumnsEqualToParameter = identityPropertyInfo.Select(p => $"[{p.Name}] = @{p.Name}");

            string sql = new StringBuilder()
               .Append("UPDATE " + tableName + " SET ")
               .Append(CommaSeparated(columnsEqualToParameter))
               .Append($" WHERE ")
               .Append(SeparatedByAndStatement(identityColumnsEqualToParameter))
               .Append(";")
               .ToString();
            IDictionary<string, object> allParameters = new ExpandoObject();
            foreach (PropertyInfo info in propertyInfo)
                allParameters.Add(info.Name, parametersObject.GetType().GetProperty(info.Name).GetValue(parametersObject));
            foreach (PropertyInfo info in identityPropertyInfo)
                allParameters.Add(info.Name, identityParameters.GetType().GetProperty(info.Name).GetValue(identityParameters));

            await con.ExecuteAsync(sql, allParameters);
        }

        public static async Task Delete(this SqlConnection con, string tableName, object identityProperties)
        {
            string sql = CreateDeleteSqlStatement(tableName, identityProperties);

            await con.ExecuteAsync(sql, identityProperties);
        }

        // -------------------
        // --- Private Methods
        // -------------------
        private static string CreateInsertSqlStatement(string tableName, object parametersObject)
        {
            IEnumerable<PropertyInfo> propertyInfo = parametersObject.GetType().GetProperties();
            IEnumerable<string> propertyNames = propertyInfo.Select(p => $"[{p.Name}]");
            IEnumerable<string> parameterNames = propertyInfo.Select(p => $"@{p.Name}");

            string sql = new StringBuilder()
                .Append("INSERT INTO " + tableName + "( ")
                .Append(CommaSeparated(propertyNames))
                .Append(" ) VALUES ( ")
                .Append(CommaSeparated(parameterNames))
                .Append(");")
                .ToString();
            return sql;
        }

        private static string CreateDeleteSqlStatement(string tableName, object identityProperties)
        {
            IEnumerable<PropertyInfo> identityPropertyInfo = identityProperties.GetType().GetProperties();
            IEnumerable<string> identityColumnsEqualToParameter = identityPropertyInfo.Select(p => $"[{p.Name}] = @{p.Name}");

            string sql = new StringBuilder()
                .Append($"DELETE FROM {tableName} WHERE ")
                .Append(SeparatedByAndStatement(identityColumnsEqualToParameter))
                .ToString();
            return sql;
        }

        private static string CommaSeparated(IEnumerable<string> list) => string.Join(",", list);
        private static string SeparatedByAndStatement(IEnumerable<string> list) => string.Join(" AND ", list);
    }
}