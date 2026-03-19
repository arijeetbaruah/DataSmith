


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

---

## 📦 Installation

### Unity Package Manager (Git URL)

1. Open **Unity Package Manager**
2. Click **Add package from git URL**
3. Paste:

```
https://github.com/arijeetbaruah/DataSmith.git?path=Packages/com.arijeet.DataSmith/DataSmith
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
 └── MathsEngine/
```

Unity will compile the scripts automatically.

---

### Verify Installation

1. Right-click in the Project window
2. Navigate to:

```
Create → Baruah → DataSmith → Maths Formula
```

3. Create a **Math Formula asset**

If the asset appears, installation succeeded.

---

## 📖 Documentation

Full documentation is available here:

https://arijeetbaruah.github.io/DataSmith/

Getting started guide:

https://arijeetbaruah.github.io/DataSmith/getting_started.html

---

## 🏗 Architecture

DataSmith is built around modular **Math Nodes**.

```
BaseMathNode
 ├── Arithmetic Nodes
 ├── Comparison Nodes
 ├── Logical Nodes
 └── Custom Nodes
```

Each node can evaluate itself and generate a readable equation.

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
