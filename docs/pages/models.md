\page models Model System

DataSmith uses a code-first model system.

You define plain C# classes, and DataSmith generates
runtime model containers, queries, and persistence logic.

---

## 🧠 What Is a Model?

A model describes a type of data stored and managed by DataSmith.

Models are:

- Plain C# classes (POCOs)
- Attribute-driven
- Serializable
- Engine-agnostic

DataSmith generates companion classes that provide:

- Storage
- Queries
- Change tracking
- Persistence

---

## 🏗 Creating a Model

Mark a class with the `GameModel` attribute.

```csharp
using Baruah.DataSmith;

 [GameModel(ModelValueType.List)]
 public class InventoryItem
 {
     [PrimaryKey]
     public string Id;

     public string Name;
     public int Value;
 }
```

Run the generator to create the runtime model classes.

---

## 📦 Model Types

The `ModelValueType` determines how data is stored.

### 🔹 Single Model

Stores exactly one instance of the data type.

```csharp
 [GameModel(ModelValueType.Single)]
 public class PlayerStats
 {
     public int Level;
     public int Experience;
 }
```

Generated model:

- Holds a single value
- Provides getters/setters for each field
- Emits change events

---

### 🔹 List Model

Stores multiple items of the data type.

```csharp
 [GameModel(ModelValueType.List)]
 public class InventoryItem
 {
     [PrimaryKey]
     public string Id;

     public string Name;
     public int Value;
 }
```

Generated model:

- Backed by a list
- Supports Add / Remove
- Supports queries
- Can enforce uniqueness via primary keys

---

### 🔹 Database Model (DB)

Uses an external database provider instead of in-memory storage.

```csharp
[GameModel(ModelValueType.DB)]
public class AccountData
{
     [PrimaryKey]
     public int AccountId;
    
     public string Username;
     public int Coins;
}
```
Behavior depends on the configured database provider.

---

## 🏷 Attributes

Attributes modify how models behave.

---

### 🔑 PrimaryKey

Marks a field as the unique identifier for the model.

```csharp
[PrimaryKey]
public string Id;
```

For list models:

- Enforces uniqueness
- Used for lookups
- Required for references

---

### 🔗 Reference

Indicates that a field refers to another model.

Typically stores the target model’s primary key.

```csharp
[Reference(typeof(InventoryItem))]
public string ItemId;
```

Generated code provides helper methods to retrieve the referenced object.

---

## 🔄 Generated Companion Classes

For each model type, DataSmith generates:

- `<TypeName>Model`
- `<TypeName>Query`

Example:

```
 InventoryItem → InventoryItemModel + InventoryItemQuery
```

These classes handle runtime behavior and should not be edited manually.

---

## 🧩 Accessing Models

Models are retrieved through the DataContext.

```csharp
var inventory = DataContext.Get<InventoryItemModel>();
```

---

## ⚠️ Best Practices

✔ Use simple public fields
✔ Always define a primary key for list models
✔ Avoid heavy logic inside model classes
✔ Treat models as data containers only

---

## 🔗 See Also

- \ref queries
- \ref datacontext
- \ref attributes
- \ref persistence
