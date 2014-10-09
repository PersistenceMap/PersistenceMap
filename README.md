![wickedflame persistancemap](assets/wickedflame persistancemap - black.png)

PersistanceMap
==============

PersistanceMap is a small, extremely lightweight and intuitive ORM Framework for .NET. It uses a Fluent API to define Queries that translate to SQL. The synthax is easy to use and resemles the SQL or LINQ synthax .
Queries are executed against the database using ADO.NET and the result data is automaticaly mapped into Dataobjects.

## Installation
------------------------------
PersistanceMap can be installed from [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) through the package manager console: 

PM > Install-Package PersistanceMap

# Examples
------------------------------
## Context
The databaseConnection opens a IDatabaseContext. IDatabaseContext containes Extensionmethods that can be used to select, insert, update or delete data from a Database.
```csharp
var dbConnection = new DatabaseConnection(new SqlContextProvider(connectionString));
using (var context = dbConnection.Open())
{

}
```
## Select
Simple select from a ***single*** Table:
```csharp
context.Select<Order>(o => o.ID == 1)
```
Join ***multiple*** Tables together into one Dataobject:
```csharp
context.From<Order>()
.Join<OrderDetail>((od, o) => od.OrderID == o.ID)
.Select<OrderWithDetail>()
```
Create a anonym object that ***defines*** the fields for the select:
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
## Insert
## Update
## Delete
## Stored Procedures




PersistanceMap is developed by [wickedflame](http://wicked-flame.blogspot.ch/) under the [Ms-PL License](License.txt).