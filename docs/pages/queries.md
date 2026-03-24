\page queries Query System

DataSmith provides a strongly-typed, fluent query system for retrieving data from models.

Queries are generated automatically for each model and allow you to filter data using a readable builder pattern.

---
## 🧠 Overview

Queries operate on model data without modifying it.

Key characteristics:

- Strongly typed
- Fluent builder API
- Lazy execution
- Chainable conditions
- Zero reflection at runtime

---

## 🏗 Generated Query Classes

For every model, DataSmith generates a corresponding query class:

```
<ModelType> → <ModelType>Query
```

Example

```
InventoryItem → InventoryItemQuery
```

These classes are auto-generated and should not be edited manually.

___

## 🚀 Creating a Query

Queries are created from a model instance.

```csharp
var query =
    DataContext.Get<InventoryItemModel>()
        .Query();
```

---

## 🔗 Fluent Filtering

Query methods correspond to the public fields of the model.

```csharp
var items =
    DataContext.Get<InventoryItemModel>()
        .Query()
        .ValueGreaterThan(50)
        .Execute();
```

Multiple conditions can be chained:

```csharp
var result =
    inventory.Query()
        .NameContains("Sword")
        .ValueLessThanEqualTo(200)
        .Execute();
```

## ⚙️ Generated Condition Methods

DataSmith generates methods based on field types.

### 🔹 Equality (All Types)
```csharp
.IdEquals("sword_01")
```
### 🔹 Numeric Comparisons

For numeric fields:

- GreaterThan
- LessThan
- GreaterThanEqualTo
- LessThanEqualTo

```csharp
.ValueGreaterThan(100)
```
### 🔹 String Operations

For string fields:

- Contains:
```csharp
.NameContains("Potion")
```

## 🔗 Reference Queries

If a field references another model via the `Reference` attribute,
DataSmith generates helper methods that accept the referenced object.

```csharp
inventoryUsage.Query()
    .ItemEquals(itemObject)
    .Execute();
```
Internally this compares primary keys.

## 🧩 Custom Conditions

You can supply your own predicate using Where.

```csharp
var rareItems =
    inventory.Query()
        .Where(i => i.Value > 500 && i.Name.StartsWith("Epic"))
        .Execute();
```

## ⏳ Lazy Execution

Queries are evaluated only when enumerated.

Calling `Execute()` returns an enumerable result.

```csharp
foreach (var item in query.Execute())
{
    Debug.Log(item.Name);
}
```

Queries can also be used directly in LINQ:

```csharp
var firstItem = query.FirstOrDefault();
```

---

## 📦 Single vs List Models
- List models return multiple results
- Single models typically operate on a single value

Behavior depends on the generated model type.

---

## 🗄 Database-Backed Queries

For database models (`ModelValueType.DB`), queries may be translated into SQL and executed by the configured database provider.

Exact behavior depends on the active `IDatabase` implementation.

---

## ⚠️ Best Practices

 ✔ Prefer generated methods over manual predicates <br>
 ✔ Chain conditions for clarity <br>
 ✔ Avoid expensive logic inside Where <br>
 ✔ Treat queries as read-only operations <br>
