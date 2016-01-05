![wickedflame persistencemap](assets/wickedflame persistencemap - black.png)

PersistenceMap
==============
[![Build status](https://ci.appveyor.com/api/projects/status/i43jveqowctku03x/branch/master?svg=true)](https://ci.appveyor.com/project/chriswalpen/persistencemap/branch/master)
[![NuGet Version](https://img.shields.io/nuget/v/persistencemap.svg?style=flat)](https://www.nuget.org/packages/PersistenceMap/)

PersistenceMap is a small, extremely lightweight and intuitive code fist, convention based Micro ORM Framework for .NET. It uses a Fluent API to define Queries that translate to SQL. The SQL is executed against the RDBMS using ADO.NET and the result is automaticaly mapped to Typed POCO's.
The simple, straight forward, intuitive and easy to use API helps to quickly setup a connction to a Database.
PersistenceMap is extremely lightweight and leaves no traces in the client code.

PersistenceMap currently supports MSSql and SQLite RDMBS Servers.

## Important Notice
PersistenceMap was renamed from PersistanceMap to PersistenceMap (note the change of **a** to **e**) in Source and on Nuget.
The Current Version of **PersistanceMap 0.4.0** is no longer supported! 

## Installation
------------------------------
PersistenceMap can be installed from [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) through the package manager console.  
MSSql Server:  
```
PM > Install-Package PersistenceMap
```
SQLite: 
``` 
PM > Install-Package PersistenceMap.Sqlite
```
# Examples
------------------------------
## Context
The ContextProvider opens a IDatabaseContext for a Databaseconnection. IDatabaseContext containes methods that can be used to select, insert, update or delete data from a Database and to create or alter Database, Tables and Fields.
```csharp
// Sql Database
var provider = new SqlContextProvider(connectionString);
using (var context = provider.Open())
{
	// use context to create Queries
}

// Sqlite Database
var provider = new SqliteContextProvider(connectionString);
using (var context = provider.Open())
{
	// use context to create Queries
}
```
## Reading Data from the Database
### Select
Simple select from a ***single*** Table:
```csharp
context.Select<Order>(o => o.ID == 1)
```
### Select with Join
Join ***multiple*** Tables together into one Dataobject:
```csharp
context.From<Order>()
	.Join<OrderDetail>((od, o) => od.OrderID == o.ID)
	.Select<OrderWithDetail>()
```
Create a anonymous object that ***defines*** the fields for the select:
```csharp
context.From<Order>()
	.Join<OrderDetail>((od, o) => od.OrderID == o.ID)
	.For(() => new { ProductID = 0, OrderID = 0 })
	.Select(a => new OrderDetail { ProductID = a.ProductID, OrderID = a.OrderID })
```
Compiles to:
```sql
select ProductID, OrderID from Order join OrderDetail on (OrderDetail.OrderID = Order.ID)
```
## Editing Data
For the Examples to edit Data we use the following Class that also represents a Table in the Database
```csharp
public class Warrior
{
    public int ID { get; set; }
    public int WeaponID { get; set; }
    public string Race { get; set; }
    public string SpecialSkill { get; set; }
}
```
### Insert
```csharp
context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" });
```
Compiles to:
```sql
INSERT INTO Warrior (ID, WeaponID, Race, SpecialSkill) VALUES (1, 0, 'Dwarf', NULL)
```
Insert with the fields defined in a anonymous object. Only the fields defined in the anonymous object get set.
```csharp
context.Insert<Warrior>(() => new { ID = 1, Race = "Dwarf" });
```
Compiles to:
```sql
INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')
```
### Update
```csharp
context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 });
```
Compiles to:
```sql
UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1)
```
Update with the fields defined in a anonymous object. Only the fields defined in the anonymous object get updated.
```csharp
context.Update(() => new { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
```
Compiles to:
```sql
UPDATE Warrior SET WeaponID = 2, Race = 'Elf' where (Warrior.ID = 1)
```
### Delete
Delete statement with keys defined in a expression.
```csharp
context.Delete<Employee>(e => e.EmployeeID == 1);
```
Compiles to:
```sql
DELETE from Employee where (Employee.EmployeeID = 1)
```
Delete with all key elements defined in a anonymous object.
```csharp
context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
```
Compiles to:
```sql
DELETE from Employee where (Employee.EmployeeID = 1) and (Employee.LastName = 'Lastname') and (Employee.FirstName = 'Firstname')
```
## Code first
PersistenceMap supports a code first approach. Databases, tables and fields can be created, altered or deleted at runtime.
### Create Database
The following commands create a Database and two tables. The name of the Database is defined in the Connectionstring. The structure of the tables are defined with the object types.
```csharp
var provider = new SqlContextProvider(ConnectionString);
using (var context = provider.Open())
{
	// create the database
    context.Database.Create();
	
	// create a table with a key
	context.Database.Table<Weapon>().Key(wpn => wpn.ID).Create();
	
	// table with a foreign key
	context.Database.Table<Warrior>().Key(wrir => wrir.ID).ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID).Create();
	
    context.Commit();
}
```
## Stored Procedures
The Stored Procedure feature is only avaliable.
### Stored Procedure without result
```csharp
context.Procedure("UpdateSalesByYear")
    .AddParameter("DateFrom", () => new DateTime(1970, 1, 1))
    .AddParameter(() => DateTime.Today)
	.AddParameter("@value", () => somevalue)
    .Execute();
```
### Stored Procedure with result
```csharp
var sales = context.Procedure("SalesByYear")
    .AddParameter(() => new DateTime(1970, 1, 1))
    .AddParameter(() => DateTime.Today)
    .Execute<SalesByYear>();
```
Map fields if the field names of the resultset and the data object don't match:
```csharp
var sales = context.Procedure("SalesByYear")
    .AddParameter(() => new DateTime(1970, 1, 1))
    .AddParameter(() => DateTime.Today)
    .For<SalesByYear>()
	.Map("OrdersID", sby => sby.ID)
	.Execute();
```
Convert a value from the resultset to a value in the data object:
```csharp
var sales = context.Procedure("SalesByYear")
    .AddParameter(() => new DateTime(1970, 1, 1))
    .AddParameter(() => DateTime.Today)
    .For<SalesByYear>()
	// converts the datetime provided by the result to a string
	.Map("Date", sby => sby.StringDate, value => ((DateTime)value).ToShortDateString())
	.Execute();
```
### Stored Procedure with output parameter
```csharp
int id = 0;
var proc = context.Procedure("SetSale")
    // adds a output parameter and executes the delegate after the procedure was executed
	.AddParameter("ID", () => id, ret => id = ret)
    .AddParameter("value", () => somevalue)
    .Execute<SalesByYear>();
```


PersistenceMap is developed by [wickedflame](http://wickedflame.github.com/) under the [Ms-PL License](License.txt).