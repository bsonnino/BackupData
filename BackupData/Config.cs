using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BackupData
{
    public class Config
    {
        public Config(string fileName)
        {
            if (!File.Exists(fileName))
                return;
            var doc = XDocument.Load(fileName);
            if (doc.Root == null)
                return;
            IncludePaths = doc.Root.Element("IncludePaths")?.Value.Split(';');
            ExcludeFiles = doc.Root.Element("ExcludeFiles")?.Value.Split(';') ?? new string[0] ;
            ExcludePaths = doc.Root.Element("ExcludePaths")?.Value.Split(';') ?? new string[0];
            BackupFile = $"{doc.Root.Element("BackupFile")?.Value}{DateTime.Now:yyyyMMddhhmmss}.zip";
            ExcludeFilesRegex =
                new Regex(string.Join("|", string.Join("|", ExcludeFiles), string.Join("|", ExcludePaths)));
        }

        public Regex ExcludeFilesRegex { get; }
        public IEnumerable<string> IncludePaths { get; }
        public IEnumerable<string> ExcludeFiles { get; }
        public IEnumerable<string> ExcludePaths { get; }
        public string BackupFile { get; }
    }
}
