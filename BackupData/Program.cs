using System;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace BackupData
{
    class Options
    {
        [Option(DefaultValue = "config.xml",
            HelpText = "Configuration file for the backup.")]
        public string ConfigFile { get; set; }

        [Option('i', "incremental", DefaultValue = false,
          HelpText = "Does an increamental backap.")]
        public bool Incremental { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);
            if (string.IsNullOrWhiteSpace(options.ConfigFile))
                return;
            if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(options.ConfigFile)))
            {
                var currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!string.IsNullOrWhiteSpace(currentDir))
                    options.ConfigFile = Path.Combine(currentDir, options.ConfigFile);
            }
            var config = new Config(options.ConfigFile);
            var backup = new Backup();
            var result = backup.DoBackup(config, options.Incremental).Result;

        }
    }
}
