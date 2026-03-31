\page getting_started Getting Started

This guide will help you quickly set up and use the MathsEngine in your Unity project.

---

# Installation

1. Open your Unity project.
2. Open **Window → Package Manager**.
3. Click the **+** button in the top-left corner.
4. Select **Add package from git URL**.
5. Enter:
```
https://github.com/arijeetbaruah/DataSmith.git
```
Unity will download and install the package automatically.

___

# How it works

## Creating a Data Model

First create a new C# class and mark it with the `GameModel` attribute.

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
---
### Key Points

- Models are **plain C# classes**
- No inheritance required
- Fields define stored data
- Attributes control behavior

---

## Generate Model Code
Open the generator window:
```csharp
Tools → Game Model Generator
```
Then Click `Generate All`

DataSmith will create:
- `InventoryItemModel.cs`
- `InventoryItemQuery.cs`
- `DataContext.cs`

These files appear in the configured output folder.

## Initialize DataContext
Before using any models, initialize the system.

```csharp
using Baruah.DataSmith;

void Awake()
{
     DataContext.Initialize();
}
```
This registers all generated models and prepares them for use.

## Access your model through the DataContext:

```csharp
var model = DataContext.Get<InventoryItemModel>();

model.Add(new InventoryItem
{
     Id = "sword_01",
     Name = "Iron Sword",
     Value = 100
});
```

## Query Data

If you are using `ModelValueType.List` then you can use the generated query system to filter data.

```csharp
var expensiveItems =
      DataContext.Get<InventoryItemModel>()
          .Query()
          .ValueGreaterThan(50)
          .Execute();
```
Queries use a fluent builder pattern and execute only when enumerated.

## Save and Load Data

Serialize all models:

```csharp
string json = DataContext.SerializeAll();
```

Restore data later:

```csharp
DataContext.DeserializeAll(json);
```

DataSmith uses Newtonsoft.Json for serialization.
___

## See Also

* \ref models "Models"
* \ref queries "Query System"
* \ref db_support "Database Support"
* \ref datacontext "DataContext"
