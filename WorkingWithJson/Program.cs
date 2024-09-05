using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace WorkingWithJson;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Person person = new Person
            {
                Age = 16,
                FirstName = "Hari",
                LastName = "Hari bol"
            };

            // write csharp object as Json data to file
            using FileStream createWrite = File.Create(path: @"..\\..\\..\\dir\\write-json-data-to-file.json");
            await JsonSerializer.SerializeAsync(createWrite, person);
            await createWrite.DisposeAsync();

            // read Json data and write to console
            using (FileStream openRead = File.OpenRead(@"..\\..\\..\\dir\\read-json-data-to-obj.json"))
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

            Console.WriteLine("\nHare Krishna");

            var jsonResult = JsonSerializer.Serialize(person);
            Console.WriteLine($"\n {jsonResult}");

            var jsonOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
                WriteIndented = true,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };

            // read Json data from file, de-serialize to csharp object with JsonSerializerOptions
            using (FileStream fileContent = File.OpenRead(@"..\\..\\..\\dir\\sample-json-data.json"))
            {
                var result = await JsonSerializer.DeserializeAsync<CarDetails>(fileContent, jsonOptions);

                if (result != null)
                {
                    Console.Write("\n");

                    Console.WriteLine($"{result.CarYear}, {result.CarMake} - {result.CarModel} :: {result.CarVIN} " +
                                      $"is {result.CarCondition} car with {result.CarColor} exterior color, has {result.CarMileage} miles " +
                                      $"from {result.CarLocationCity} in {result.CarLocationCountry}, is selled at ${result.CarPrice} by {result.CarOwner}");
                }
            }

            List<Quotes>? quotesList;

            HttpClient httpClient = new HttpClient();
            var webResult = await httpClient.GetAsync("https://api.quotable.io/quotes/random?limit=108");

            if (webResult.IsSuccessStatusCode)
            {
                quotesList = await webResult.Content.ReadFromJsonAsync<List<Quotes>>();

                // quotesList = await webResult.Content.ReadFromJsonAsync(QuotesContext.Default.ListQuotes);

                Console.WriteLine("\n");

                foreach (var quoteDetails in quotesList)
                {
                    Console.WriteLine($"\t\"{quoteDetails.Content}\" from \"{quoteDetails.Author}\" is {quoteDetails.Length} long" +
                        $" added on {quoteDetails.DateAdded} by {quoteDetails.AuthorSlug} and modified on {quoteDetails.DateModified}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(ex.ToString());
        }
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

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
class CarDetails
{
    private string? carMake;
    private string? carModel;
    private int carYear;
    private string carVin = string.Empty;
    private string? carColor;
    private string? carCondition;

    public string? CarMake
    {
        get { return carMake; }
        set { carMake = value; }
    }
    public string? CarModel
    {
        get => carModel;
        set
        {
            carModel = value;
        }
    }
    public int CarYear
    {
        get { return carYear; }
        set { carYear = value; }
    }
    public string CarVIN
    {
        get => carVin; set
        {
            if (value == null || value == string.Empty)
            {
                throw new ArgumentNullException("Vehicle Identification Number is required");
            }
            carVin = value;
        }
    }
    public string? CarColor
    {
        get { return carColor; }
        set { carColor = value; }
    }
    public double CarPrice { get; set; }
    public int CarMileage { get; set; }
    public string? CarCondition { get => carCondition; set => carCondition = value; }
    public string CarOwner = string.Empty;
    public string CarLocationCity { get; set; } = string.Empty;
    public string? CarLocationCountry { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CarSummary { get; set; }
    public short CarBatteryLifeReadOnly { get; private set; } = 15;
    public short CarTiresLife { get; set; } = default;
}

class Quotes
{
    private string? _id;
    private string? _author;
    private string? _content;
    private string[]? _tags;

    [JsonRequired]
    public string? _Id
    {
        get { return _id; }
        set { _id = value; }
    }
    public string? Author
    {
        get { return _author; }
        set { _author = value; }
    }
    public string? Content
    {
        get { return _content; }
        set { _content = value; }
    }
    public string[]? Tags
    {
        get { return _tags; }
        set { _tags = value; }
    }
    public string? AuthorSlug { get; set; }

    [JsonInclude]
    public int? Length;
    public string? DateAdded { get; set; }
    public string? DateModified { get; set; }
}

[JsonSerializable(typeof(List<Quotes>))]
internal sealed partial class QuotesContext : JsonSerializerContext
{

}