using System.Text.Json;

namespace WorkingWithJson;

class Program
{
    static async Task Main(string[] args)
    {
        Person person = new Person
        {
            Age = 16,
            FirstName = "Hari",
            LastName = "Hari bol"
        };

        // write csharp object as Json data to file
        using FileStream createWrite = File.Create(path: "dir\\write-json-data-to-file.json");
        await JsonSerializer.SerializeAsync(createWrite, person);
        await createWrite.DisposeAsync();

        // read Json data and write to console
        using (FileStream openRead = File.OpenRead("dir\\read-json-data-to-obj.json"))
        {
            var readResult = await JsonSerializer.DeserializeAsync<Person[]>(openRead);
            if (readResult != null)
            {
                foreach (var item in readResult)
                {
                    Console.WriteLine($"Age of {item.LastName} {item.FirstName} is {item.Age}");
                }
            }
        }

        Console.WriteLine("Hare Krishna");

        var jsonResult = JsonSerializer.Serialize(person);
        Console.WriteLine(jsonResult);
    }
}

class Person
{
    private int _age;
    public int Age
    {
        get { return _age; }
        set
        {
            if (value < 0 || value > 17)
            {
                throw new ArgumentException("Allowed Age limit is upto 16");
            }
            _age = value;
        }
    }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}