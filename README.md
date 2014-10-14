![wickedflame persistancemap](assets/wickedflame persistancemap - black.png)

PersistanceMap
==============

PersistanceMap is a small, extremely lightweight and intuitive ORM Framework for .NET. It uses a Fluent API to define Queries that translate to SQL. The Queries are executed against a RDBMS using ADO.NET. The resulting data is automaticaly mapped into Dataobjects/POCO's.
The synthax is simple and easy to use and resemles the SQL or LINQ synthax.
PersistanceMap is extremely lightweight and leaves no traces in the client code.
The current versions only support MSSql Servers.

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
Insert with the fields defined in a anonym object.
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
Update with the fields defined in a anonym object
```csharp
context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
```
Compiles to:
```sql
UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1)
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
Delete with all key elements defined in a anonym object.
```csharp
context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
```
Compiles to:
```sql
DELETE from Employee where (Employee.EmployeeID = 1) and (Employee.LastName = 'Lastname') and (Employee.FirstName = 'Firstname')
```
## Stored Procedures




PersistanceMap is developed by [wickedflame](http://wicked-flame.blogspot.ch/) under the [Ms-PL License](License.txt).