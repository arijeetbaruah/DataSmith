![Cover Image](https://github.com/arijeetbaruah/DataSmith/blob/main/docs/Image/Cover.jpg)


<h1 align="center">DataSmith</h1>

<p align="center">
Serializable Math Formula System for Unity
</p>

<p align="center">

<img src="https://img.shields.io/github/stars/arijeetbaruah/DataSmith?style=for-the-badge">
<img src="https://img.shields.io/github/license/arijeetbaruah/DataSmith?style=for-the-badge">
<img src="https://img.shields.io/github/issues/arijeetbaruah/DataSmith?style=for-the-badge">

</p>

---

## ✨ Overview

**DataSmith** is an attribute-driven data modeling framework for Unity that automatically generates strongly-typed models and fluent query APIs.

It delivers an ORM-like developer experience for in-memory gameplay data, enabling scalable, maintainable systems without boilerplate or runtime reflection.

---

## 🚀 Features

• Attribute-driven model definitions  
• Automatic code generation (Models + Queries)  
• Strongly-typed accessors and events  
• Fluent, lazy query system  
• ORM-like developer experience  
• Centralized data architecture  
• Zero runtime reflection  
• Designer-friendly workflow  
• Scales to large projects

## 🚀 Why DataSmith?

Managing gameplay data in Unity often leads to:

❌ Boilerplate getters/setters  
❌ Scattered data access logic  
❌ Fragile string-based queries  
❌ Hard-to-maintain systems  
❌ Overuse of ScriptableObjects

**DataSmith solves this** by turning plain C# classes into full data systems automatically.

---

## 🏗️ Core Concept

Define your data once:

```csharp
[GameModel(ModelValueType.List)]
public class InventoryItem
{
    public string Id;
    public int Quantity;
    public bool IsEquipped;
}
```
Generate models with the editor tool:
```
Tools → Game Model Generator
```
DataSmith creates:

- A model wrapper
- Accessors
- Events
- Query builder
- Utility methods

---

## 📦 Installation

### Unity Package Manager (Git URL)

1. Open **Unity Package Manager**
2. Click **Add package from git URL**
3. Paste:

```
https://github.com/arijeetbaruah/DataSmith.git
```

Unity will install the package automatically.

---

### Manual Installation

Clone the repository:

```
git clone https://github.com/arijeetbaruah/DataSmith.git
```

Copy the runtime folder into your Unity project:

```
Assets/
 └── DataSmith/
```

Unity will compile the scripts automatically.

---

## 📖 Documentation

Full documentation is available here:

https://arijeetbaruah.github.io/DataSmith/

Getting started guide:

https://arijeetbaruah.github.io/DataSmith/getting_started.html

---

## 🧩 Generated Model Example
```csharp
inventoryModel.Add(new InventoryItem
{
Id = "potion",
Quantity = 5
});
```
Access data safely:
```csharp
var potions = inventoryModel.FindById("potion");
````
---
## ⚙️ Model Types
### List Model
Represents a collection of entities.
```csharp
[GameModel(ModelValueType.List)]
public class EnemyData { ... }
```
### Single Model

Represents a single state object.
```csharp
[GameModel(ModelValueType.Single)]
public class PlayerStats { ... }
```
---
## 🛠️ Generator Tool

Open:
```
Tools → Game Model Generator
```
Features:

- Generate all models at once
- Generate per-script
- Custom output folder
- Include / Exclude filters
- Wildcard support (*, **, *.cs)
- Template-based generation
- Safe incremental updates
---
## ⚡ Performance

DataSmith is optimized for gameplay use:

✔ No runtime reflection<br>
✔ No dynamic dispatch<br>
✔ Lazy query execution<br>
✔ Single-pass filtering<br>
✔ IL2CPP safe<br>
✔ Allocation-minimal

---
## 🎯 Use Cases

- Gameplay state management
- Inventory systems
- Character stats
- Quest systems
- Simulation data
- Strategy games
- MMO-style data layers
- Designer-driven content
---

## 🤝 Contributing

Contributions are welcome!

Please read:

```
CONTRIBUTING.md
```

before submitting a pull request.

Pull requests should follow the repository template.

---

## 🌟 Contributors

Thanks to all contributors who help improve this project.

<a href="https://github.com/arijeetbaruah/DataSmith/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=arijeetbaruah/DataSmith" />
</a>

---

## 🛡 License

This project is licensed under the **MIT License**.

---

## ⭐ Support the Project

If you find this project useful:

* ⭐ Star the repository
* 🐛 Report issues
* 🧩 Contribute new nodes
* 📢 Share it with other Unity developers
