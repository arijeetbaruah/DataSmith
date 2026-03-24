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

## 🧱 Attribute Scope

| Attribute  | Applies To |
| ---------- | ---------- |
| GameModel  | Class      |
| PrimaryKey | Field      |
| Reference  | Field      |

---

## ⚠️ Constraints and Rules
### Primary Key Requirements
- Must be a public field
- Should be immutable after creation
- Should uniquely identify each item
- Required for references
---
###  Reference Requirements
- Target model must have a primary key
- Field type must match the primary key type
- Circular references are allowed but should be used carefully

---

## 🧠 Best Practices

✔ Keep model classes simple<br>
✔ Use attributes instead of manual wiring<br>
✔ Define primary keys for list and DB models<br>
✔ Avoid business logic inside model classes<br>
✔ Use references instead of duplicating data<br>

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

    public int Quantity;
}
```
This model:

- Stores multiple usage records
- Has a unique identifier
- References an inventory item
- Supports generated queries and lookups

---
## 🔗 See Also

* \ref models "Models"
* \ref queries "Query System"
* \ref db_support "Database Support"
* \ref datacontext "Context"
