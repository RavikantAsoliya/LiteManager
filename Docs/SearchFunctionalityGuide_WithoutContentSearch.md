## Search Functionality Guide (Without Content Search)

The search functionality in the LiteManager application allows users to search for files and folders based on a search query. The search is performed recursively within the current directory and its subdirectories.

### Usage

To perform a search in the LiteManager application, follow these steps:

1. Enter the search query in the search box.
2. Press the Enter key to initiate the search.
3. The application will clear the existing items in the file list view.
4. The search will be performed recursively within the current directory and its subdirectories.
5. Any files or folders matching the search query will be added to the file list view.
6. The file list view will display the search results, including the name, icon, type, size, and last modified date of each file or folder.

### Error Handling

The search functionality includes error handling to handle exceptions that may occur during the search process. If an error occurs while processing a file or folder, the application will log the error message to the console. Additionally, if an error occurs while searching a directory, the application will log the error message and continue searching other directories.

### Limitations

Please note the following limitations of the search functionality:

- The search is performed based on the file and folder names only. It does not search within the file contents.
- The search is case-sensitive, meaning it will match the search query exactly as entered.
- The search may take longer to complete for large directories or when searching for a common term that appears in multiple files or folders.

## Code Explanation

Let's go through the code in detail to understand how the search functionality is implemented.

### Search Method

The `Search` method is the entry point for performing a search. It takes two parameters: `directoryPath` (the path of the directory to search in) and `searchQuery` (the query to search for). Here's the code:

```csharp
private void Search(string directoryPath, string searchQuery)
{
    fileListView.BeginInvoke(new Action(() =>
    {
        fileListView.Items.Clear();
    }));
    
    Task.Run(() =>
    {
        try
        {
            SearchDirectory(directoryPath, searchQuery);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during the search: " + ex.Message);
        }
    });
}
```

The method starts by clearing the existing items in the file list view using `fileListView.Items.Clear()`. Then, it creates a new `Task` and runs the search operation inside it. The `SearchDirectory` method is called to perform the actual search. Any exceptions that occur during the search are caught and logged to the console.

### SearchDirectory Method

The `SearchDirectory` method is responsible for searching a directory and its subdirectories recursively. It takes two parameters: `directoryPath` (the path of the directory to search in) and `searchQuery` (the query to search for). Here's the code:

```csharp
private void SearchDirectory(string directoryPath, string searchQuery)
{
    try
    {
        var directory = new DirectoryInfo(directoryPath);
        var files = directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
        var folders = directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            try
            {
                if (file.Name.ToLower().Contains(searchQuery.ToLower()))
                {
                    AddItemToListView(file.FullName, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing file '{file.FullName}': {ex.Message}");
            }
        }

        Parallel.ForEach(folders, folder =>
        {
            try
            {
                if (folder.Name.ToLower().Contains(searchQuery.ToLower()))
                {
                    AddItemToListView(folder.FullName, false);
                }
                SearchDirectory(folder.FullName, searchQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing folder '{folder.FullName}': {ex.Message}");
            }
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while searching directory '{directoryPath}': {ex.Message}");
    }
}
```

Inside the `SearchDirectory` method, the directory specified by `directoryPath` is accessed using `new DirectoryInfo(directoryPath)`. The `EnumerateFiles` and `EnumerateDirectories` methods are used to get the files and folders within the directory, respectively.

In the `SearchDirectory` method, the search query and file/folder names are converted to lowercase using the `ToLower()` method. This ensures that the search is case-insensitive, as both the search query and file/folder names are compared in lowercase.

The code then iterates over each file and checks if its name contains the `searchQuery`. If it does, the `AddItemToListView` method is called to add the file to the file list view. Any exceptions that occur during this process are caught and logged.

Next, the code uses `Parallel.ForEach` to iterate over each folder in parallel. This allows for concurrent searching of multiple folders, improving the search performance. Similar to files, if a folder's name contains the `searchQuery`, it is added to the file list view. Additionally, the `SearchDirectory` method is called recursively to search within the subdirectories of the current folder. Any exceptions during this process are caught and logged.

If any exceptions occur while accessing the directory itself, such as permission issues or invalid paths, they are caught and logged as well.

### SearchToolStripComboBox_KeyDown Method

The `SearchToolStripComboBox_KeyDown` method handles the keydown event of the search text box. It triggers the search operation when the Enter key is pressed. Here's the code:

```csharp
private void SearchToolStripComboBox_KeyDown(object sender, KeyEventArgs e)
{
    string searchQuery = searchToolStripComboBox.Text;
    string directoryPath = currentDirectory;
    if (e.KeyCode == Keys.Enter)
    {
        if (!string.IsNullOrEmpty(searchQuery))
        {
            Task.Run(() =>
            {
                Search(directoryPath, searchQuery);
            });
        }
    }
}
```

The method checks if the Enter key is pressed and ensures that the search query is not empty. If both conditions are met, it creates a new `Task` and runs the `Search` method to perform the search operation.

### AddItemToListView Method

The `AddItemToListView` method is responsible for adding a file or folder to the file list view. It takes two parameters: `fullPath` (the full path of the file or folder) and `isFile` (a flag indicating whether the item is a file or folder). Here's the code:

```csharp
private void AddItemToListView(string fullPath, bool isFile)
{
    Task.Run(() =>
    {
        if (isFile)
        {
            fileListView.Invoke(new Action(() =>
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                ListViewItem item = fileListView.Items.Add(fileInfo.Name);
                if (fileInfo.Extension == ".exe" || fileInfo.Extension == "")
                {
                    Icon fileIcon = IconProvider.GetIconByFileName(fileInfo.FullName);
                    imageList.Images.Add(fileInfo.Name, fileIcon);
                    item.ImageKey = fileInfo.Name;
                }
                else
                {
                    if (!imageList.Images.ContainsKey(fileInfo.Extension))
                    {
                        Icon fileIcon = IconProvider.GetIconByFileName(fileInfo.FullName);
                        imageList.Images.Add(fileInfo.Extension, fileIcon);
                    }
                    item.ImageKey = fileInfo.Extension;
                }
                item.Tag = new Dictionary<string, string>
                {
                    {"FullName", fileInfo.FullName},
                    {"Type", "File" }
                };
                item.SubItems.Add(FileTypeChecker.GetFileTypeByExtension(fileInfo.Extension));
                item.SubItems.Add(SizeManager.FormatSize(fileInfo.Length));
                item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                countToolStripStatusLabel.Text = $"{fileListView.Items.Count} items";
            }));
        }
        else
        {
            fileListView.Invoke(new Action(() =>
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fullPath);
                ListViewItem item = fileListView.Items.Add(dirInfo.Name, (int)IconIndex.Folder);
                item.Tag = new Dictionary<string, string>
                {
                    {"FullName", dirInfo.FullName},
                    {"Type", "Folder" }
                };
                item.SubItems.Add("Folder");
                item.SubItems.Add("");
                item.SubItems.Add(dirInfo.LastWriteTime.ToString());
                countToolStripStatusLabel.Text = $"{fileListView.Items.Count} items";
            }));
        }
    });
}

```

Inside the method, a new `Task` is created to run the UI-related code asynchronously. If the item is a file (`isFile` is true), the code is executed within the UI thread using `fileListView.Invoke`. Similarly, if the item is a folder, the code to add the folder to the list view is executed within the UI thread.

## Conclusion

You now have a comprehensive guide and documentation for the search functionality in the LiteManager application. The provided code snippets and explanations should help you implement and understand the search functionality in detail. Feel free to refer back to this documentation whenever you need to work with or modify the search feature.