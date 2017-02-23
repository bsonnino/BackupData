using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackupData
{
    class FileFinder
    {
        public async Task<ConcurrentDictionary<string, List<string>>> GetFiles(string[] paths, 
            Regex excludeFilesRegex, Regex excludePathsRegex, bool incremental)
        {
            var files = new ConcurrentDictionary<string, List<string>>();
            var tasks = paths.Select(path =>
                Task.Factory.StartNew(() =>
                {
                    var rootDir = "";
                    var drive = Path.GetPathRoot(path);
                    if (!string.IsNullOrWhiteSpace(drive))
                    {
                        rootDir = drive[0] + "_drive";
                        rootDir = rootDir + path.Substring(2);
                    }
                    else
                        rootDir = path;
                    var selectedFiles = GetFilesInDirectory(path, excludeFilesRegex, excludePathsRegex, incremental);
                    files.AddOrUpdate(rootDir, selectedFiles.ToList(), (a, b) => b);
                }));
            await Task.WhenAll(tasks);
            return files;
        }

        private List<string> GetFilesInDirectory(string directory, Regex excludeFilesRegex, 
            Regex excludePathsRegex,bool incremental)
        {
            var files = new List<string>();
            try
            {
                var directories = Directory.GetDirectories(directory);
                try
                {
                    var selectedFiles = Directory.EnumerateFiles(directory).Where(f => !excludeFilesRegex.IsMatch(f.ToLower()));
                    if (incremental)
                        selectedFiles = selectedFiles.Where(f => (File.GetAttributes(f) & FileAttributes.Archive) != 0);
                    files.AddRange(selectedFiles);
                }
                catch
                {
                }
                foreach (var dir in directories.Where(d => !excludePathsRegex.IsMatch(d.ToLower())))
                {
                    files.AddRange(GetFilesInDirectory(Path.Combine(directory, dir), excludeFilesRegex, excludePathsRegex, incremental));
                }
            }
            catch
            {
            }

            return files;
        }
    }
}