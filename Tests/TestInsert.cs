
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

namespace Tests
{
    [TestClass]
    public class TestInsert
    {
        [TestMethod]
        public async Task InsertSingle()
        {            
            IEnumerable<TestTableEntity> rows = null;

            try
            {
                using (var connection = new SqlConnection(Settings.ConnectionString))
                {
                    await ClearTestTable.Clear(connection);

                    int id = await connection.Insert("TestTable", new { TestString = "HI", TestInt = 5 });

                    rows = await connection.QueryAsync<TestTableEntity>("Select * from TestTable WHERE TestTableID = @ID", new { ID = id });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            Assert.AreEqual(1, rows.Count());
            Assert.AreEqual("HI", rows.First().TestString);
            Assert.AreEqual(5, rows.First().TestInt);
        }
    }
}
