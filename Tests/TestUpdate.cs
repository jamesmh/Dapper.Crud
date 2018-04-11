
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
    public class TestUpdate
    {
        [TestMethod]
        public async Task UpdateSinglePropertyTest()
        {
            TestTableEntity firstRow = null;
            TestTableEntity afterUpdate = null;

            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                await TableUtils.ClearTestDB(connection);

                int id = await connection.Insert("TestTable", new { TestString = "HI", TestInt = 5 });

                firstRow = await connection.QuerySingleAsync<TestTableEntity>("Select * from TestTable WHERE TestTableID = @ID", new { ID = id });

                await connection.Update("TestTable", new { TestTableID = id }, new { TestString = "UPDATED" });

                afterUpdate = await connection.QuerySingleAsync<TestTableEntity>("Select * from TestTable WHERE TestTableID = @ID", new { ID = id });
            }

            Assert.AreEqual("HI", firstRow.TestString);
            Assert.AreEqual("UPDATED", afterUpdate.TestString);
        }

        [TestMethod]
        public async Task UpdateMultiPropertyTest()
        {
            TestTableEntity firstRow = null;
            TestTableEntity afterUpdate = null;

            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                await TableUtils.ClearTestDB(connection);

                int id = await connection.Insert("TestTable", new { TestString = "HI", TestInt = 5 });

                firstRow = await connection.QuerySingleAsync<TestTableEntity>("Select * from TestTable WHERE TestTableID = @ID", new { ID = id });

                await connection.Update("TestTable", new { TestTableID = id }, new { TestString = "UPDATED", TestInt = 999 });

                afterUpdate = await connection.QuerySingleAsync<TestTableEntity>("Select * from TestTable WHERE TestTableID = @ID", new { ID = id });
            }

            Assert.AreEqual("HI", firstRow.TestString);
            Assert.AreEqual(5, firstRow.TestInt);

            Assert.AreEqual("UPDATED", afterUpdate.TestString);
            Assert.AreEqual(999, afterUpdate.TestInt);
        }
    }
}
