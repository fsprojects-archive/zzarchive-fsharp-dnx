using System;
using System.Collections.Generic;
using System.IO;

namespace YoloDev.Dnx.FSharp
{
  class TempFiles : IDisposable
  {
    readonly List<string> _files;
    readonly List<string> _dirs;

    public TempFiles()
    {
      _files = new List<string>();
      _dirs = new List<string>();
    }

    public Stream CreateStream(out string path)
    {
      path = Path.GetTempFileName();
      _files.Add(path);
      return File.Open(path, FileMode.Truncate, FileAccess.Write);
    }

    public string CreateDir()
    {
      var path = Path.GetTempFileName();
      File.Delete(path);
      Directory.CreateDirectory(path);
      _dirs.Add(path);
      return path;
    }

    public void Dispose()
    {
      foreach (var file in _files)
        if (File.Exists(file))
          File.Delete(file);

      foreach (var dir in _dirs)
        if (Directory.Exists(dir))
          Directory.Delete(dir, true);
    }
  }
}
