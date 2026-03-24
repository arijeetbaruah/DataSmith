\page db_support Database Support

DataSmith supports external databases through a pluggable provider system.

Instead of hardcoding a specific database engine, DataSmith communicates with databases through the `IDatabase` interface. This allows you to switch between storage backends without changing your model code.

---
## 🧠 Overview

Database-backed models use `ModelValueType.DB`.

These models:

- Persist data outside of memory
- Can execute queries through a database provider
- Support large datasets
- Enable server-backed or persistent storage
- Are independent of a specific SQL implementation

---

🏗️ Enabling Database Models

Mark your model with ModelValueType.DB.

```csharp
using Baruah.DataSmith;

[GameModel(ModelValueType.DB)]
public class AccountData
{
[PrimaryKey]
public int AccountId;

    public string Username;
    public int Coins;
}
```

Generated models will use the active database provider for storage and queries.

---

## 🔌 Database Provider System

DataSmith communicates with databases using the IDatabase interface.

A provider implements this interface and executes commands against a specific backend.

<b>Example Interface</b>
```csharp
public interface IDatabase
{
    IEnumerable<T> Query<T>(string commandText);
    int Execute(string commandText);
}
```

The exact interface may vary depending on the provider implementation.

### ⚙️ Configuring a Provider

The active database provider is configured through the DataContext.

```csharp
DataContext.Database = new SqliteDatabase(connectionString);
```


Once configured, all DB models will use this provider automatically.

---

### 🧪 Built-In SQLite Support

DataSmith can work with Mono.Data.Sqlite when available.

SQLite is ideal for:

- Local save data
- Offline applications
- Prototyping
- Small to medium datasets

Requirements
- `Mono.Data.Sqlite.dll` present in the project
- Compatible Unity scripting backend

### Custom Database Providers

You can implement support for any database system by creating your own provider.

<b>Example: Custom Provider</b>

```csharp
public class MyDatabase : IDatabase
{
    public IEnumerable<T> Query<T>(string commandText)
    {
    // Execute query and map results
    }

    public int Execute(string commandText)
    {
        // Execute non-query command
    }
}
```

Then register it:

```csharp
DataContext.Database = new MyDatabase();
```

DataContext.Database = new MyDatabase();

🔎 Database Queries

For DB models, generated queries may be translated into database commands instead of in-memory filtering.

Example usage:

```csharp
var richPlayers =
    DataContext.Get<AccountDataModel>()
        .Query()
        .CoinsGreaterThan(1000)
        .Execute();
```
The provider determines how this query is executed.

---

💾 Persistence Behavior

Unlike in-memory models:
- Data is stored externally
- Changes may be committed immediately or batched
- Behavior depends on the provider implementation
