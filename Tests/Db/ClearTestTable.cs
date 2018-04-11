using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Tests.Db
{
    public class ClearTestTable
    {
        public static async Task Clear(SqlConnection con){
            await con.ExecuteAsync("DELETE FROM TestTable");
        }
    }
}