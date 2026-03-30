\page attributes Attributes

DataSmith uses C# attributes to control how models are generated, stored, and queried.

Attributes allow you to define behavior declaratively without writing boilerplate code.

---

## 🧠 Overview

Attributes are applied directly to model classes or fields.

They determine:

- How models are stored
- How queries are generated
- Relationships between models
- Persistence behavior
- Data integrity rules

---

## 🏗️ GameModel Attribute

Marks a class as a DataSmith model and specifies its storage type.

Syntax
```csharp
[GameModel(ModelValueType valueType)]
```

<b>Example</b>
```csharp
using Baruah.DataSmith;

[GameModel(ModelValueType.List)]
public class InventoryItem
{
    public string Id;
    public string Name;
    public int Value;
}
```

| Parameter               | Description                      |
| ----------------------- | -------------------------------- |
| `ModelValueType.Single` | Stores one instance of the model |
| `ModelValueType.List`   | Stores multiple items            |
| `ModelValueType.DB`     | Uses a database provider         |

🔑 PrimaryKey Attribute

Marks a field as the unique identifier for a model.

Syntax
```csharp
[PrimaryKey]
```
<b>Example</b>
```csharp
[PrimaryKey]
public string Id;
```
### Behavior

For list models:
- Enforces uniqueness
- Enables fast lookups
- Required for references
- Used in query generation
- Used for database mapping

Only one primary key should be defined per model.

---

## 🔗 Reference Attribute

Indicates that a field references another model.

The field typically stores the primary key of the target model.

Syntax
```csharp
[Reference(typeof(TargetModel))]
```
<b>Example</b>
```csharp
[Reference(typeof(InventoryItem))]
public string ItemId;
```
### Behavior

DataSmith generates helper methods that:

- Resolve the referenced object
- Enable object-based query filters
- Maintain loose coupling between models

<b>Example usage:</b>
```csharp
var item =
    DataContext.Get<InventoryUsageModel>()
    .GetInventoryItem();
```
---

🧩 Reference Query Support

Reference fields also generate query helpers that accept the referenced object.
```csharp
usageModel.Query()
    .ItemEquals(itemObject)
    .Execute();
```
Internally this compares primary keys.

---

## 🚧 Column Constraints
Used to enforce rules in database-backed models.

### NotNull

Prevent null entry for the selected field
```csharp
[Required]
public string Name;
```

---

### Unique

Prevent duplication of data
```csharp
[Unique]
public string Email;
```

---

### DefaultValue

If omitted, this value is written by default.

```csharp
[DefaultValue(0)]
public int Value;
```

### RangeAttribute

Make sure the value is between a range
```csharp
 [Range(0, 99)]
 public int Quantity;
```

---

### MaxLengthAttribute

For string variable, ensure the string length is max this amount

```csharp
[MaxLength(30)]
public string Name;
```

---

### Behavior

- Translates into SQL constraints
- Ensures data integrity
- Used during table creation

---

## 🧱 Attribute Scope

| Attribute    | Applies To |
|--------------|------------|
| GameModel    | Class      |
| PrimaryKey   | Field      |
| Reference    | Field      |
| Range        | Field      |
| Required     | Field      |
| Unique       | Field      |
| DefaultValue | Field      |
| MaxLength    | Field      |


---

## ⚠️ Constraints and Rules
### Primary Key Requirements
- Must be a public field
- Should be immutable after creation
- Must uniquely identify each item
- Required for references and DB models
---
###  Reference Requirements
- Target model must have a primary key
- Field type must match the primary key type
- Works across Memory and DB models
- Circular references are allowed but should be used carefully
---
### Database Constraints
- Only valid for ModelStorageType.DB
- Invalid usage in Memory/Asset models may be ignored or warned
- Must be compatible with selected database provider

---

## 🧠 Best Practices

✔ Keep model classes simple<br>
✔ Use attributes instead of manual wiring<br>
✔ Always define primary keys for List/DB models<br>
✔ Avoid business logic inside model classes<br>
✔ Use references instead of duplicating data<br>
✔ Use constraints to enforce correctness at the database level<br>

---

## 🧭 Example: Complete Model
```csharp
[GameModel(ModelValueType.List)]
public class InventoryUsage
{
    [PrimaryKey]
    public int Id;

    [Reference(typeof(InventoryItem))]
    public string ItemId;
    
    [NotNull]
    public int Quantity;
}
```
This model:
- Uses database storage
- Stores multiple records
- Has a primary key
- References another model
- Enforces data constraints

---
## 🔗 See Also

* \ref models "Models"
* \ref queries "Query System"
* \ref db_support "Database Support"
* \ref datacontext "Context"
