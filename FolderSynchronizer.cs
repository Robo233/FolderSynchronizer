using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Linq;

class FolderSynchronizer
{
    private static string? sourcePath;
    private static string? replicaPath;
    private static string? logFilePath;
    
    private static int syncInterval;

    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSynchronizer <sourcePath> <replicaPath> <logFilePath> <syncInterval>");
            return;
        }

        sourcePath = args[0];
        replicaPath = args[1];
        logFilePath = args[2];
        syncInterval = int.Parse(args[3]);

        if (sourcePath == null || replicaPath == null || logFilePath == null)
        {
            Console.WriteLine("Invalid paths provided.");
            return;
        }

        if (!Directory.Exists(replicaPath))
        {
            Directory.CreateDirectory(replicaPath);
            Log($"Replica folder created: {replicaPath}");
        }

        Log("Synchronization started.");

        while (true)
        {
            try
            {
                SynchronizeFolders();
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
            Thread.Sleep(syncInterval * 1000);
        }
    }

    private static void SynchronizeFolders()
    {
        if (sourcePath == null || replicaPath == null)
        {
            Log("Source or replica path is null.");
            return;
        }

        foreach (var directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            var relativePath = directory.Substring(sourcePath.Length + 1);
            var replicaDir = Path.Combine(replicaPath, relativePath);

            if (replicaDir != null && !Directory.Exists(replicaDir))
            {
                Directory.CreateDirectory(replicaDir);
                Log($"Directory created: {replicaDir}");
            }
        }

        foreach (var file in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
        {
            var relativePath = file.Substring(sourcePath.Length + 1);
            var replicaFile = Path.Combine(replicaPath, relativePath);

            if (!File.Exists(replicaFile) || !FilesAreEqual(file, replicaFile))
            {
                File.Copy(file, replicaFile, true);
                Log($"File copied: {file} -> {replicaFile}");
            }
        }

        foreach (var file in Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories))
        {
            var relativePath = file.Substring(replicaPath.Length + 1);
            var sourceFile = Path.Combine(sourcePath, relativePath);

            if (!File.Exists(sourceFile))
            {
                File.Delete(file);
                Log($"File deleted: {file}");
            }
        }

        foreach (var directory in Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories))
        {
            var relativePath = directory.Substring(replicaPath.Length + 1);
            var sourceDir = Path.Combine(sourcePath, relativePath);

            if (!Directory.Exists(sourceDir))
            {
                Directory.Delete(directory, true);
                Log($"Directory deleted: {directory}");
            }
        }
    }

    private static bool FilesAreEqual(string file1, string file2)
    {
        using (var hashAlg = MD5.Create())
        {
            var hash1 = hashAlg.ComputeHash(File.ReadAllBytes(file1));
            var hash2 = hashAlg.ComputeHash(File.ReadAllBytes(file2));
            return hash1.SequenceEqual(hash2);
        }
    }

    private static void Log(string message)
    {
        if (logFilePath == null)
        {
            Console.WriteLine("Log file path is null.");
            return;
        }

        var logMessage = $"{DateTime.Now}: {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}
