using System;
using System.IO;

namespace HereticalSolutions.Persistence.IO
{
    /// <summary>
    /// Provides methods for reading and writing text files.
    /// </summary>
    public static class TextFileIO
    {
        /// <summary>
        /// Writes the specified contents to the file specified by the given <see cref="FilePathSettings"/>.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="contents">The contents to write.</param>
        /// <returns>true if the write operation is successful; otherwise, false.</returns>
        public static bool Write(
            FilePathSettings settings,
            string contents)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);

            File.WriteAllText(savePath, contents);

            return true;
        }
        
        /// <summary>
        /// Writes the specified byte array to the file specified by the given <see cref="FilePathSettings"/>.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="contents">The byte array to write.</param>
        /// <returns>true if the write operation is successful; otherwise, false.</returns>
        public static bool Write(
            FilePathSettings settings,
            byte[] contents)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);

            File.WriteAllBytes(savePath, contents);

            return true;
        }

        /// <summary>
        /// Reads the contents of the file specified by the given <see cref="FilePathSettings"/> into a string.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="contents">When this method returns, contains the contents of the file, or an empty string if the file does not exist.</param>
        /// <returns>true if the read operation is successful; otherwise, false.</returns>
        public static bool Read(
            FilePathSettings settings,
            out string contents)
        {
            string savePath = settings.FullPath;

            contents = string.Empty;
			
            if (!FileExists(savePath))
                return false;
			
            contents = File.ReadAllText(savePath);

            return true;
        }
        
        /// <summary>
        /// Reads the contents of the file specified by the given <see cref="FilePathSettings"/> into a byte array.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        /// <param name="contents">When this method returns, contains the contents of the file, or null if the file does not exist.</param>
        /// <returns>true if the read operation is successful; otherwise, false.</returns>
        public static bool Read(
            FilePathSettings settings,
            out byte[] contents)
        {
            string savePath = settings.FullPath;

            contents = null;
			
            if (!FileExists(savePath))
                return false;
			
            contents = File.ReadAllBytes(savePath);

            return true;
        }

        /// <summary>
        /// Deletes the file specified by the given <see cref="FilePathSettings"/>, if it exists.
        /// </summary>
        /// <param name="settings">The file system settings.</param>
        public static void Erase(FilePathSettings settings)
        {
            string savePath = settings.FullPath;

            if (File.Exists(savePath))
                File.Delete(savePath);
        }

        /// <summary>
        /// Checks whether the file at the specified path exists.
        /// Also makes sure the folder directory specified in the path actually exists anyway.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>true if the file exists; otherwise, false.</returns>
        private static bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[TextFileIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[TextFileIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            return File.Exists(path);
        }

        /// <summary>
        /// Ensures that the directory specified in the path exists.
        /// </summary>
        /// <param name="path">The path to the file.</param>
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