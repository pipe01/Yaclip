# Yaclip

Library with a fluent interface for parsing command line arguments in C#.

## Usage

```cs
class NewOptions
{
    public string Name { get; set; }
    public int Size { get; set; }

    public string Extension { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var app = YaclipApp.New()
            .Name("My example application")
            .ExecutableName("myapp") // If not specified it will be guessed from the current process name
            .GenerateHelpCommand(true) // Default is true
            .Command<NewOptions>("new", c => c
                .Description("Creates a new file")
                .Callback(o => Console.WriteLine(o.Name)) // The callback will be called when app.Run is called
                .Argument(o => o.Name, a => a
                    .Name("name")
                    .Required(true)) // Default is false
                .Argument(o => o.Size, a => a
                    .Name("size"))
                .Option(o => o.Extension, o => o
                    .ShortName('e')
                    .LongName("extension")
                    .Description("The extension of the new file")
                    .ValueName("ext"))); // Only used for generating help message

        app.Build().Run(args);
    }
}
```

### Output

`myapp help`:
```
My example application.

Usage:
  myapp new <name> [size] [-e=<ext> | --extension=<ext>]
  myapp help [command]
```

`myapp help new`:
```
My example application.

Creates a new file

Usage: myapp new <name> [size] [-e=<ext> | --extension=<ext>]

Options:
  -e=<ext>, --extension=<ext>  The extension of the new file
```
