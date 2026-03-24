\page datacontext DataContext

The **DataContext** is the central entry point for all database operations
within the DataSmith framework.

It acts as a unit-of-work that manages database connections,
provides access to generated models and queries, and coordinates
data operations in a consistent and efficient manner.

---

## Overview

A DataContext instance represents an active session with the database.
All reads, writes, and transactions should be performed through this object.

The Generator Tool produces strongly-typed accessors that attach to
the DataContext, allowing you to interact with your database using
compile-time safe APIs instead of raw SQL.

---

## Responsibilities

The DataContext is responsible for:

* Establishing and managing database connections
* Providing access to generated repositories and query objects
* Executing commands and queries
* Managing transactions
* Ensuring proper resource cleanup
* Coordinating data operations across models

---

## Lifecycle

Typically, a DataContext is created when database access begins
and disposed when the operation completes.

Depending on your application architecture, it may be:

* Short-lived (per operation)
* Scoped (per system or service)
* Long-lived (application-wide)

---

## Basic Usage

```csharp
using (var context = new GameDataContext())
{
    var player = context.Players.GetById(1);
    player.Name = "Commander";

    context.SaveChanges();
}
```

---

## Generated Access Points

The Generator Tool creates properties on the DataContext that expose
database entities and queries.

Example:

* `context.Players` — Access player data
* `context.Items` — Access item data
* `context.Regions` — Access region data

These properties provide type-safe operations such as:

* Retrieval by primary key
* Filtered queries
* Insert, update, and delete operations
* Custom generated queries

---

## Transactions

The DataContext can coordinate multiple operations within a single
database transaction to ensure atomicity.

```csharp
using (var context = new GameDataContext())
{
    using (var tx = context.BeginTransaction())
    {
        context.Players.Add(newPlayer);
        context.Items.Add(newItem);

        tx.Commit();
    }
}
```

If a failure occurs before commit, all changes are rolled back.

---

## Connection Management

The DataContext handles opening and closing the database connection
automatically.

Manual connection handling is typically unnecessary and discouraged,
as improper usage can lead to resource leaks or inconsistent state.

---

## Thread Safety

DataContext instances are generally **not thread-safe**.

Each thread or task should use its own instance unless explicitly
documented otherwise.

---

## Best Practices

* Create DataContext instances only when needed
* Dispose contexts promptly
* Avoid sharing a single instance across unrelated systems
* Use transactions for multi-step operations
* Prefer generated APIs over raw SQL when possible

---

## When to Create a New Context

Create a new DataContext when:

* Starting a new logical operation
* Performing background processing
* Running independent tasks
* Isolation between operations is required

---

## See Also
 
* \ref models "Models"
* \ref queries "Query System"
* \ref db_support "Database Support"