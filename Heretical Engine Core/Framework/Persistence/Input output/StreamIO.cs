using System.Text;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.IO
{
    public static class StreamIO
    {
        public static bool OpenReadStream(
            FilePathSettings settings,
            out FileStream dataStream,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            dataStream = default(FileStream);

            if (!FileExists(
                settings.FullPath,
                logger))
                return false;

            dataStream = new FileStream(savePath, FileMode.Open);

            return true;
        }
        
        public static bool OpenReadStream(
            FilePathSettings settings,
            out StreamReader streamReader,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            streamReader = default(StreamReader);

            if (!FileExists(
                settings.FullPath,
                logger))
                return false;

            streamReader = new StreamReader(savePath, Encoding.UTF8);

            return true;
        }
        
        public static bool OpenWriteStream(
            FilePathSettings settings,
            out FileStream dataStream,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(
                savePath,
                logger);
            
            dataStream = new FileStream(savePath, FileMode.Create);

            return true;
        }
        
        public static bool OpenWriteStream(
            FilePathSettings settings,
            out StreamWriter streamWriter,
            ILogger logger = null)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(
                savePath,
                logger);
            
            streamWriter = new StreamWriter(savePath, false, Encoding.UTF8);

            return true;
        }

        public static void CloseStream(FileStream dataStream)
        {
            dataStream.Close();
        }
        
        public static void CloseStream(StreamReader streamReader)
        {
            streamReader.Close();
        }
        
        public static void CloseStream(StreamWriter streamWriter)
        {
            streamWriter.Close();
        }

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
                    logger.TryFormat("INVALID PATH"));
			
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