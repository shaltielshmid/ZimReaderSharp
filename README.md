# ZimReaderSharp - A Robust Library for reading ZIM files

`ZimReaderSharp` provides a streamlined interface to efficiently read and handle ZIM files. With support for multiple compression types—including BZip2, LZMA, and ZStd—this library prioritizes performance, loading only necessary data on-the-fly for rapid startup.

## Installation

To incorporate ZimReaderSharp into your project, choose one of the following methods:

### .NET CLI
```bash
dotnet add package ZimReaderSharp
```

### NuGet Package Manager
```powershell
Install-Package ZimReaderSharp
```

For detailed package information, visit [ZimReaderSharp on NuGet](https://www.nuget.org/packages/ZimReaderSharp/).

## Key Usages

The main class `ZimFile` offers an interface for accessing all the entries in a given ZIM file. You can either iterate over them, retrieve them by URL or by retrieve them by index. 

Given a returned Entry, it has the following properties:

- `Format`: An object containing all the metadata for a given entry: `Namespace`, `Revision`, `Url`, `Title`, `Parameter`.

- `Index`: The index of the entry in the file.

- `IsRedirect`: If the user requested to not follow redirects, then this flag will be true and data will be null.

- `Data`: A byte[] containing the data of the entry

- `Mime`: A string representation of the mime type. 


## Sample usage

Here is an example to read all the HTML entries in a given ZIM file: 

```cs
string filename = "wikipedia_en_all_nopic_2023-06.zim";

using var zimFile = new ZimFile(filename);

foreach (var (index, mime) in zimFile.IterateArticles()) {
    if (mime == "text/html") {
        var entry = zimFile.GetArticleByIndex(index, fFollowRedirect: false);
        if (entry.IsRedirect) continue;

        string html = Encoding.UTF8.GetString(entry.Data);
    }
}
```

Or, here is an example to get an entry by a URL:

```cs
string filename = "wikipedia_en_all_nopic_2023-06.zim";

using var zimFile = new ZimFile(filename);

var entry = zimFile.GetEntryByUrl('A', "some_url");
Console.WriteLine(entry.Format.Title);
```
