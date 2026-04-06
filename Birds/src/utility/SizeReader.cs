using System;
using System.Buffers.Binary;
using System.IO;

namespace Birds.src.utility;

public static class SizeReader
{
  public static (int Width, int Height) GetPngSize(string path)
  {
    using var fs = File.OpenRead(path);
    fs.Position = 16;
    byte[] buffer = new byte[8];
    fs.Read(buffer, 0, 8);

    int width = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan(0, 4));
    int height = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan(4, 4));
    return (width, height);
  }

}

