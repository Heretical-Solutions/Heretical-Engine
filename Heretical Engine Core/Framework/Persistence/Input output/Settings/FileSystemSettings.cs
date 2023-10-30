using System;
using System.IO;

namespace HereticalSolutions.Persistence.IO
{
    [Serializable]
    public class FilePathSettings
    {
        public string RelativePath;

        public string ApplicationDataFolder;

        public string FullPath
        {
            get
            {
                return Path
                    .Combine(
                        ApplicationDataFolder,
                        RelativePath)
                    .SanitizePath();
            }
            set
            {
                if (!string.IsNullOrEmpty(ApplicationDataFolder))
                {
                    RelativePath = value
                        .SanitizePath()
                        .Replace(
                            ApplicationDataFolder,
                            "");
                }
                else
                {
                    RelativePath = value
                        .SanitizePath();
                }
            }
        }
    }
}