using System;
using System.IO;
using System.Text;

namespace HereticalSolutions.Persistence.IO
{
    /// <summary>
    /// Provides methods for working with streams.
    /// </summary>
    public static class StreamIO
    {
        /// <summary>
        /// Opens a file stream for reading.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="dataStream">The opened file stream.</param>
        /// <returns>True if the stream was successfully opened, otherwise false.</returns>
        public static bool OpenReadStream(
            FilePathSettings settings,
            out FileStream dataStream)
        {
            string savePath = settings.FullPath;

            dataStream = default(FileStream);

            if (!FileExists(settings.FullPath))
                return false;

            dataStream = new FileStream(savePath, FileMode.Open);

            return true;
        }
        
        /// <summary>
        /// Opens a stream reader for reading.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="streamReader">The opened stream reader.</param>
        /// <returns>True if the stream was successfully opened, otherwise false.</returns>
        public static bool OpenReadStream(
            FilePathSettings settings,
            out StreamReader streamReader)
        {
            string savePath = settings.FullPath;

            streamReader = default(StreamReader);

            if (!FileExists(settings.FullPath))
                return false;

            streamReader = new StreamReader(savePath, Encoding.UTF8);

            return true;
        }
        
        /// <summary>
        /// Opens a file stream for writing.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="dataStream">The opened file stream.</param>
        /// <returns>True if the stream was successfully opened, otherwise false.</returns>
        public static bool OpenWriteStream(
            FilePathSettings settings,
            out FileStream dataStream)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);
            
            dataStream = new FileStream(savePath, FileMode.Create);

            return true;
        }
        
        /// <summary>
        /// Opens a stream writer for writing.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="streamWriter">The opened stream writer.</param>
        /// <returns>True if the stream was successfully opened, otherwise false.</returns>
        public static bool OpenWriteStream(
            FilePathSettings settings,
            out StreamWriter streamWriter)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);
            
            streamWriter = new StreamWriter(savePath, false, Encoding.UTF8);

            return true;
        }

        /// <summary>
        /// Closes a file stream.
        /// </summary>
        /// <param name="dataStream">The file stream to close.</param>
        public static void CloseStream(FileStream dataStream)
        {
            dataStream.Close();
        }
        
        /// <summary>
        /// Closes a stream reader.
        /// </summary>
        /// <param name="streamReader">The stream reader to close.</param>
        public static void CloseStream(StreamReader streamReader)
        {
            streamReader.Close();
        }
        
        /// <summary>
        /// Closes a stream writer.
        /// </summary>
        /// <param name="streamWriter">The stream writer to close.</param>
        public static void CloseStream(StreamWriter streamWriter)
        {
            streamWriter.Close();
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        public static void Erase(FilePathSettings settings)
        {
            string savePath = settings.FullPath;

            if (File.Exists(savePath))
                File.Delete(savePath);
        }
        
        /// <summary>
        /// Checks whether the file at the specified path exists and ensures the directory path exists.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        private static bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[UnityStreamIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[UnityStreamIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            return File.Exists(path);
        }
        
        /// <summary>
        /// Ensures that the directory path exists.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        private static void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[TextFileIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[TextFileIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}