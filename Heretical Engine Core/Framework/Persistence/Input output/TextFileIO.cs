using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.IO
{
    public static class TextFileIO
    {
        public static bool Write(
            FilePathSettings settings,
            string contents,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(
                savePath,
                logger);

            File.WriteAllText(savePath, contents);

            return true;
        }
        
        public static bool Write(
            FilePathSettings settings,
            byte[] contents,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(
                savePath,
                logger);

            File.WriteAllBytes(savePath, contents);

            return true;
        }

        public static bool Read(
            FilePathSettings settings,
            out string contents,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            contents = string.Empty;
			
            if (!FileExists(
                savePath,
                logger))
                return false;
			
            contents = File.ReadAllText(savePath);

            return true;
        }
        
        public static bool Read(
            FilePathSettings settings,
            out byte[] contents,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            contents = null;
			
            if (!FileExists(
                savePath,
                logger))
                return false;
			
            contents = File.ReadAllBytes(savePath);

            return true;
        }

        /// <summary>
        /// Deletes the file specified by the given <see cref="FilePathSettings"/>, if it exists
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        public static void Erase(FilePathSettings settings)
        {
            string savePath = settings.FullPath;

            if (File.Exists(savePath))
                File.Delete(savePath);
        }

        private static bool FileExists(
            string path,
            ILogger logger = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception(
                    logger.TryFormat("INVALID PATH"));
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception(
                    logger.TryFormat("INVALID DIRECTORY PATH"));
			
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            return File.Exists(path);
        }

        private static void EnsureDirectoryExists(
            string path,
            ILogger logger = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception(
                    logger.TryFormat($"INVALID PATH"));
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception(
                    logger.TryFormat("INVALID DIRECTORY PATH"));
			
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}