using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Tests.Db.Utils
{
    public class TableUtils
    {
        public static async Task ClearTestDB(SqlConnection con){
            await con.ExecuteAsync("DELETE FROM TestTable");
        }
    }
}