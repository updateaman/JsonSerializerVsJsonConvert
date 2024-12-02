
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Bogus;
using JsonCompare;

[MemoryDiagnoser(false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class JsonSerializerVsJsonConvert
{
    [Params(10000)]
    public int Count { get; set; }
    private List<Customer> customers = [];
    private string customersJson = "";

    private List<string> customerJson = [];

    [GlobalSetup]
    public void GlobalSetup()
    {
        var testAddress = new Faker<Address>()
        .RuleFor(a => a.City, f => f.Address.City())
        .RuleFor(a => a.Street, f => f.Address.StreetName())
        .RuleFor(a => a.Longitude, f => f.Address.Longitude())
        .RuleFor(a => a.Latitude, f => f.Address.Latitude());

        var faker = new Faker<Customer>()
        .RuleFor(c => c.Name, f => f.Name.FullName())
        .RuleFor(c => c.Orders, f => f.Random.Int(1, 100))
        .RuleFor(c => c.DateOfBirth, f => f.Date.Recent())
        .RuleFor(c => c.EmailAddress, f => f.Internet.Email())
        .RuleFor(c => c.IpAddress, f => f.Internet.Ip())
        .RuleFor(c => c.Addresses, f => testAddress.Generate(f.Random.Int(1, 10)).ToArray());

        customers = faker.Generate(Count);
        customersJson = System.Text.Json.JsonSerializer.Serialize(customers);

        customerJson = customers.Select(c => System.Text.Json.JsonSerializer.Serialize(c)).ToList();
    }

    [BenchmarkCategory("DeserializeBigData"), Benchmark]
    public void JsonSerializer_Deserialize_BigData()
    {
        _ = System.Text.Json.JsonSerializer.Deserialize<List<Customer>>(customersJson);
    }

    [BenchmarkCategory("DeserializeBigData"), Benchmark(Baseline = true)]
    public void JsonConvert_Deserialize_BigData()
    {
        _ = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer>>(customersJson);
    }

    [BenchmarkCategory("DeserializeSmallData"), Benchmark]
    public void JsonSerializer_Deserialize_SmallData()
    {
        foreach (var customer in customerJson)
        {
            _ = System.Text.Json.JsonSerializer.Deserialize<Customer>(customer);
        }
    }

    [BenchmarkCategory("DeserializeSmallData"), Benchmark(Baseline = true)]
    public void JsonConvert_Deserialize_SmallData()
    {
        foreach (var customer in customerJson)
        {
            _ = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(customer);
        }
    }

    [BenchmarkCategory("SerializeBigData"), Benchmark]
    public void JsonSerializer_Serialize_BigData()
    {
        _ = System.Text.Json.JsonSerializer.Serialize(customers);
    }

    [BenchmarkCategory("SerializeBigData"), Benchmark(Baseline = true)]
    public void JsonConvert_Serialize_BigData()
    {
        _ = Newtonsoft.Json.JsonConvert.SerializeObject(customers);
    }

    [BenchmarkCategory("SerializeSmallData"), Benchmark]
    public void JsonSerializer_Serialize_SmallData()
    {
        foreach (var customer in customers)
        {
            _ = System.Text.Json.JsonSerializer.Serialize(customer);
        }
    }

    [BenchmarkCategory("SerializeSmallData"), Benchmark(Baseline = true)]
    public void JsonConvert_Serialize_SmallData()
    {
        foreach (var customer in customers)
        {
            _ = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<JsonSerializerVsJsonConvert>();
    }
}