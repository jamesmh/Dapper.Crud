# Dapper.Crud

Extensions methods for `IDbConnection` to enable easy Insert, Update, Delete operations using Dapper.

Currently only supports SQL Server databases.

All methods are async.

## General Usage

Extension methods `Insert`, `Update`, `Delete` and `DeleteAll` are available for the `IDbConnection` interface.

Please remember to use proper disposal of connections:

``` c#
using (IDbConnection connection = new SqlConnection(Settings.ConnectionString))
{
    // Do your stuff here ;)
}
```

### Insert

Create a new record. Returns an `int` of the generated Id.

**Parameters:**

> **Table name**

> **Method Parameters:** An object (just like with Dapper parameters) that has only the fields and values to insert.

``` c#
int id = await connection.Insert("TestTable", new { TestString = "HI", TestInt = 5 });
```

### Update

Update an number of existing records.

**Method Parameters:**

> **Table name**

> **Predicate parameters:** Update only the records that have the specified column / value. In the example below, only records that have a value of `5` for the `TestTableID` column (which happens to be a primary key) will be updated.

> **Method Parameters:** An object that has only the fields and values to update.

``` c#
 await connection.Update("TestTable", new { TestTableID = 5 }, new { TestString = "UPDATED" });
```

### Delete

Delete a number of records.

**Method Parameters:**

> **Table name**

> **Predicate parameters:** Delete only the records that have the specified column / value. In the example below, only records that have a value of `5` for the `TestTableID` column will be deleted (one record since that's a primary key column).

``` c#
 await connection.Delete("TestTable", new { TestTableID = 5 });
```

**DeleteAll**

Delete all records in a database table.

**Method Parameters:**

> **Table name**

``` c#
 await connection.DeleteAll("TestTable");
```

## Potential Future Features?

- Ability to specify database (mysql, etc.) which will adjust how generated ids are returned from `Insert` etc.

- Version on `Insert` that returns GUID ids.