/**
using Microsoft.Data.Sqlite;
using Birds.src.controllers.composite.blueprints;
using Birds.src.controllers.composite.blueprints.parts;
using Birds.src.utility;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Birds.src.storage;

public class SqliteBlueprintStorage : IBlueprintStorage
{
  private readonly string _connectionString;

  public SqliteBlueprintStorage()
  {
    var dbPath = GetPlatformDbPath();
    _connectionString = $"Data Source={dbPath}";
    InitializeDatabase();
  }

  private void InitializeDatabase()
  {
    using var connection = new SqliteConnection(_connectionString);
    connection.Open();

    var createScript = @"
                CREATE TABLE IF NOT EXISTS Blueprints (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT UNIQUE NOT NULL
                );

                CREATE TABLE IF NOT EXISTS EntityPlacements (
                    BlueprintId INTEGER NOT NULL,
                    EntityId INTEGER NOT NULL,
                    EntityType INTEGER NOT NULL,
                    FOREIGN KEY(BlueprintId) REFERENCES Blueprints(Id)
                );

                CREATE TABLE IF NOT EXISTS Connections (
                    BlueprintId INTEGER NOT NULL,
                    EntityId1 INTEGER NOT NULL,
                    EntityId2 INTEGER NOT NULL,
                    LinkIndex1 INTEGER NOT NULL,
                    LinkIndex2 INTEGER NOT NULL,
                    FOREIGN KEY(BlueprintId) REFERENCES Blueprints(Id)
                );";

    using var command = new SqliteCommand(createScript, connection);
    command.ExecuteNonQuery();
  }

  public async Task SaveBlueprintAsync(CompositeBlueprint blueprint)
  {
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    // Get or create blueprint ID
    var getBlueprintId = new SqliteCommand("SELECT Id FROM Blueprints WHERE Name = @name", connection);
    getBlueprintId.Parameters.AddWithValue("@name", blueprint.Name);
    var existingId = await getBlueprintId.ExecuteScalarAsync();

    long blueprintId;
    if (existingId != null)
    {
      blueprintId = (long)existingId;
      // Clear existing data
      var clearEntities = new SqliteCommand("DELETE FROM EntityPlacements WHERE BlueprintId = @id", connection);
      clearEntities.Parameters.AddWithValue("@id", blueprintId);
      await clearEntities.ExecuteNonQueryAsync();

      var clearConnections = new SqliteCommand("DELETE FROM Connections WHERE BlueprintId = @id", connection);
      clearConnections.Parameters.AddWithValue("@id", blueprintId);
      await clearConnections.ExecuteNonQueryAsync();
    }
    else
    {
      // Insert new blueprint
      var insertBlueprint = new SqliteCommand("INSERT INTO Blueprints (Name) VALUES (@name); SELECT last_insert_rowid();", connection);
      insertBlueprint.Parameters.AddWithValue("@name", blueprint.Name);
      blueprintId = (long)await insertBlueprint.ExecuteScalarAsync();
    }

    // Insert entities
    foreach (var entity in blueprint.Entities)
    {
      var insertEntity = new SqliteCommand("INSERT INTO EntityPlacements (BlueprintId, EntityId, EntityType) VALUES (@blueprintId, @entityId, @entityType)", connection);
      insertEntity.Parameters.AddWithValue("@blueprintId", blueprintId);
      insertEntity.Parameters.AddWithValue("@entityId", entity.Id);
      insertEntity.Parameters.AddWithValue("@entityType", (int)entity.EntityType);
      await insertEntity.ExecuteNonQueryAsync();
    }

    // Insert connections
    foreach (var conn in blueprint.Connections)
    {
      var insertConnection = new SqliteCommand("INSERT INTO Connections (BlueprintId, EntityId1, EntityId2, LinkIndex1, LinkIndex2) VALUES (@blueprintId, @entityId1, @entityId2, @linkIndex1, @linkIndex2)", connection);
      insertConnection.Parameters.AddWithValue("@blueprintId", blueprintId);
      insertConnection.Parameters.AddWithValue("@entityId1", conn.EntityId1);
      insertConnection.Parameters.AddWithValue("@entityId2", conn.EntityId2);
      insertConnection.Parameters.AddWithValue("@linkIndex1", conn.LinkIndex1);
      insertConnection.Parameters.AddWithValue("@linkIndex2", conn.LinkIndex2);
      await insertConnection.ExecuteNonQueryAsync();
    }
  }

  public async Task<CompositeBlueprint> LoadBlueprintAsync(string name)
  {
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    // Get blueprint ID
    var getBlueprintId = new SqliteCommand("SELECT Id FROM Blueprints WHERE Name = @name", connection);
    getBlueprintId.Parameters.AddWithValue("@name", name);
    var blueprintIdObj = await getBlueprintId.ExecuteScalarAsync();

    if (blueprintIdObj == null)
      throw new FileNotFoundException($"Blueprint '{name}' not found");

    var blueprintId = (long)blueprintIdObj;

    // Load entities
    var entities = new List<EntityPlacement>();
    var getEntities = new SqliteCommand("SELECT EntityId, EntityType FROM EntityPlacements WHERE BlueprintId = @id ORDER BY EntityId", connection);
    getEntities.Parameters.AddWithValue("@id", blueprintId);
    using var entitiesReader = await getEntities.ExecuteReaderAsync();
    while (await entitiesReader.ReadAsync())
    {
      entities.Add(new EntityPlacement
      {
        Id = entitiesReader.GetInt32(0),
        EntityType = (ID_ENTITY)entitiesReader.GetInt32(1)
      });
    }

    // Load connections
    var connections = new List<Connection>();
    var getConnections = new SqliteCommand("SELECT EntityId1, EntityId2, LinkIndex1, LinkIndex2 FROM Connections WHERE BlueprintId = @id", connection);
    getConnections.Parameters.AddWithValue("@id", blueprintId);
    using var connectionsReader = await getConnections.ExecuteReaderAsync();
    while (await connectionsReader.ReadAsync())
    {
      connections.Add(new Connection
      {
        EntityId1 = connectionsReader.GetInt32(0),
        EntityId2 = connectionsReader.GetInt32(1),
        LinkIndex1 = connectionsReader.GetInt32(2),
        LinkIndex2 = connectionsReader.GetInt32(3)
      });
    }

    return new CompositeBlueprint
    {
      Name = name,
      Entities = entities,
      Connections = connections
    };
  }

  public async Task<List<string>> GetBlueprintNamesAsync()
  {
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    var getNames = new SqliteCommand("SELECT Name FROM Blueprints ORDER BY Name", connection);
    var names = new List<string>();
    using var reader = await getNames.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      names.Add(reader.GetString(0));
    }
    return names;
  }

  public async Task DeleteBlueprintAsync(string name)
  {
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    var delete = new SqliteCommand("DELETE FROM Blueprints WHERE Name = @name", connection);
    delete.Parameters.AddWithValue("@name", name);
    await delete.ExecuteNonQueryAsync();
  }

  public async Task<bool> BlueprintExistsAsync(string name)
  {
    using var connection = new SqliteConnection(_connectionString);
    await connection.OpenAsync();

    var check = new SqliteCommand("SELECT COUNT(*) FROM Blueprints WHERE Name = @name", connection);
    check.Parameters.AddWithValue("@name", name);
    var count = (long)await check.ExecuteScalarAsync();
    return count > 0;
  }

  private string GetPlatformDbPath()
  {
#if ANDROID
          return Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "birds_game.db");
#elif IOS
          var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
          return Path.Combine(documents, "birds_game.db");
#elif WINDOWS || LINUX || MACOS
          var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          var appFolder = Path.Combine(appData, "Birds");
          Directory.CreateDirectory(appFolder);
          return Path.Combine(appFolder, "birds_game.db");
#else
    return Path.Combine(Environment.CurrentDirectory, "birds_game.db");
#endif
  }
}
*/