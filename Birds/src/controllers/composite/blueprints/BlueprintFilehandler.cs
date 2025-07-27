using System.Collections.Generic;
using System.IO;
using System;

namespace Birds.src.controllers.composite.blueprints;

public class BlueprintFileHandler
{
  public void SaveBlueprint(Blueprint blueprint)
  {
    string vertexFileName = $"{blueprint.Name}_vertices.txt";
    string edgeFileName = $"{blueprint.Name}_edges.txt";

    SaveVertices(vertexFileName, blueprint.Vertices);
    SaveEdges(edgeFileName, blueprint.Edges);
  }

  public Blueprint LoadBlueprint(string blueprintName)
  {
    string vertexFileName = $"{blueprintName}_vertices.txt";
    string edgeFileName = $"{blueprintName}_edges.txt";

    var vertices = LoadVertices(vertexFileName);
    var edges = LoadEdges(edgeFileName);

    return new Blueprint(blueprintName, vertices, edges);
  }

  private void SaveVertices(string fileName, List<BlueprintVertex> vertices)
  {
    using (var writer = new StreamWriter(fileName))
    {
      writer.WriteLine("# Vertices: Id,X,Y,Rotation,EntityType");
      foreach (var vertex in vertices)
      {
        writer.WriteLine($"{vertex.Id},{vertex.Position.X},{vertex.Position.Y},{vertex.Rotation},{vertex.EntityType}");
      }
    }
  }

  private void SaveEdges(string fileName, List<BlueprintEdge> edges)
  {
    using (var writer = new StreamWriter(fileName))
    {
      writer.WriteLine("# Edges: VertexId1,VertexId2,LinkIndex1,LinkIndex2");
      foreach (var edge in edges)
      {
        writer.WriteLine($"{edge.VertexId1},{edge.VertexId2},{edge.LinkIndex1},{edge.LinkIndex2}");
      }
    }
  }

  private List<BlueprintVertex> LoadVertices(string fileName)
  {
    var vertices = new List<BlueprintVertex>();

    if (!File.Exists(fileName))
      return vertices;

    var lines = File.ReadAllLines(fileName);
    foreach (var line in lines)
    {
      if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
        continue;

      var parts = line.Split(',');
      if (parts.Length >= 5)
      {
        var vertex = new BlueprintVertex
        {
          Id = int.Parse(parts[0]),
          Position = new Vector2(float.Parse(parts[1]), float.Parse(parts[2])),
          Rotation = float.Parse(parts[3]),
          EntityType = (ID_ENTITY)Enum.Parse(typeof(ID_ENTITY), parts[4])
        };
        vertices.Add(vertex);
      }
    }

    return vertices;
  }

  private List<BlueprintEdge> LoadEdges(string fileName)
  {
    var edges = new List<BlueprintEdge>();

    if (!File.Exists(fileName))
      return edges;

    var lines = File.ReadAllLines(fileName);
    foreach (var line in lines)
    {
      if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
        continue;

      var parts = line.Split(',');
      if (parts.Length >= 4)
      {
        var edge = new BlueprintEdge
        {
          VertexId1 = int.Parse(parts[0]),
          VertexId2 = int.Parse(parts[1]),
          LinkIndex1 = int.Parse(parts[2]),
          LinkIndex2 = int.Parse(parts[3])
        };
        edges.Add(edge);
      }
    }

    return edges;
  }
}