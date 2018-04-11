
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Db;
using Dapper.Crud;
using System.Threading.Tasks;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using Tests.Db.Models;
using Tests.Db.Utils;

namespace Tests
{
    [TestClass]
    public class TestDelete
    {
        [TestMethod]
        public async Task DeleteByIDTest()
        {
            IEnumerable<TestTableEntity> rows = null;
            IEnumerable<TestTableEntity> rowsAfterDelete = null;

            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                await TableUtils.ClearTestDB(connection);

                IEnumerable<object> list = new List<object>(){
                        new { TestString = "HI", TestInt = 5 },
                        new { TestString = "HI2", TestInt = 6 },
                        new { TestString = "HI3", TestInt = 7 }
                    };

                await connection.InsertMulti("TestTable", list);

                rows = await connection.QueryAsync<TestTableEntity>("Select * from TestTable ORDER BY TestTableID;");

                await connection.Delete("TestTable", new { TestTableID = rows.First().TestTableID });

                rowsAfterDelete = await connection.QueryAsync<TestTableEntity>("Select * from TestTable ORDER BY TestTableID;");
            }

            Assert.AreEqual(3, rows.Count());
            Assert.AreEqual(2, rowsAfterDelete.Count());

            // Compare IDs to ensure proper entity was deleted.
            Assert.AreEqual(rows.Skip(1).First().TestTableID, rowsAfterDelete.First().TestTableID);
            Assert.AreEqual(rows.Skip(2).First().TestTableID, rowsAfterDelete.Skip(1).First().TestTableID);
        }
    }
}
