using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zip;

namespace BackupData
{
    class Backup
    {
        private readonly FileFinder _fileFinder = new FileFinder();

        public async Task<int> DoBackup(Config config, bool incremental)
        {
            var files = await _fileFinder.GetFiles(config.IncludePaths.ToArray(), config.ExcludeFilesRegex, incremental);
            using (ZipFile zip = new ZipFile())
            {
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                foreach (var path in files)
                    zip.AddFiles(path.Value, false, path.Key);
                zip.Save(config.BackupFile);
            }
            foreach (var file in files.SelectMany(f => f.Value))
                ResetArchiveAttribute(file);
            return 0;
        }

        public void ResetArchiveAttribute(string fileName)
        {
            var attributes = File.GetAttributes(fileName);
            File.SetAttributes(fileName, attributes & ~FileAttributes.Archive);
        }
    }
}
