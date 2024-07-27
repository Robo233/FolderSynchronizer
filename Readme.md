## Requirements

- .NET SDK (version 5.0 or later)

## Usage

**Build the project**

```sh
dotnet build
```

**Running the executable**

```sh
./bin/Debug/net8.0/FolderSynchronizer.exe <sourcePath> <replicaPath> <logFilePath> <syncInterval>
```

- `sourcePath`: The path of the source folder.
- `replicaPath`: The path of the replica folder.
- `logFilePath`: The path of the log file.
- `syncInterval`: The synchronization interval in seconds.

For example:

```sh
./bin/Debug/net8.0/FolderSynchronizer.exe C:\Veeam\SourceFolder C:\Veeam\ReplicaFolder C:\Veeam\sync.log 5
```

Note: It's unnecessary to create all the directories before running the program. The program will create them except for the source directory.
