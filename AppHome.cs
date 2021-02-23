using System;
using System.IO;
using JetBrains.Annotations;
using Tomlyn.Syntax;
using static System.Environment;

namespace AppUtils {
    
    [PublicAPI]
    public class AppHome {
        
        public string Ns { get; }
        public string ShareDir { get; }
        public string NsDir { get; }
        public string ConfigPath { get; }

        public AppHome(string ns) {
            Ns = ns;
            
            ShareDir = GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.Create);
            NsDir = Path.Combine(ShareDir, ns);

            if (!Directory.Exists(NsDir)) {
                Directory.CreateDirectory(NsDir);
            }

            ConfigPath = Path.Combine(NsDir, "config.toml");
        }

        public void CreateConfig(DocumentSyntax initialConfig = null) {
            File.WriteAllText(ConfigPath, (initialConfig?.ToString() ?? "") + "\n");
        }

        public bool ConfigPresent() => File.Exists(ConfigPath);

        [CanBeNull]
        public AppConfig GetConfig() {
            var result = AppConfig.FromConfigPath(ConfigPath);

            return result.Match(cfg => cfg, 
                                bag => { 
                                    foreach (var diag in bag) { 
                                        Console.Error.WriteLine(diag.ToString());
                                    }

                                    return null;
                                });
        }
        
    }
}
