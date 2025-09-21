using System.Collections.Generic;
using System.IO;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Birds.src.containers.composite.blueprints;

namespace Birds.src.storage.implementations
{
  public class JsonBlueprintStorage : IBlueprintStorage
  {
    private readonly string _storagePath;

    public JsonBlueprintStorage()
    {
      _storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Birds", "blueprints");
      Directory.CreateDirectory(_storagePath);
    }

    public async Task SaveBlueprintAsync(CompositeBlueprint blueprint)
    {
      var fileName = GetSafeFileName(blueprint.Name) + ".json";
      var filePath = Path.Combine(_storagePath, fileName);

      var json = JsonSerializer.Serialize(blueprint, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<CompositeBlueprint> LoadBlueprintAsync(string name)
    {
      var fileName = GetSafeFileName(name) + ".json";
      var filePath = Path.Combine(_storagePath, fileName);

      if (!File.Exists(filePath))
        throw new FileNotFoundException($"Blueprint '{name}' not found");

      var json = await File.ReadAllTextAsync(filePath);
      return JsonSerializer.Deserialize<CompositeBlueprint>(json);
    }

    public async Task<List<string>> GetBlueprintNamesAsync()
    {
      return await Task.Run(() =>
      {
        if (!Directory.Exists(_storagePath))
          return new List<string>();

        return Directory.GetFiles(_storagePath, "*.json")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Select(RestoreOriginalName)
            .OrderBy(name => name)
            .ToList();
      });
    }

    public async Task DeleteBlueprintAsync(string name)
    {
      var fileName = GetSafeFileName(name) + ".json";
      var filePath = Path.Combine(_storagePath, fileName);

      await Task.Run(() =>
      {
        if (File.Exists(filePath))
          File.Delete(filePath);
      });
    }

    public async Task<bool> BlueprintExistsAsync(string name)
    {
      var fileName = GetSafeFileName(name) + ".json";
      var filePath = Path.Combine(_storagePath, fileName);
      return await Task.Run(() => File.Exists(filePath));
    }

    private string GetSafeFileName(string name)
    {
      var invalid = Path.GetInvalidFileNameChars();
      return invalid.Aggregate(name, (current, c) => current.Replace(c, '_'));
    }

    private string RestoreOriginalName(string safeFileName)
    {
      return safeFileName.Replace('_', ' ');
    }
  }
}
