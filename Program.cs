
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Bogus;
using Person = JsonCompare.Person;

[MemoryDiagnoser(false)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class JsonSerializerVsJsonConvert
{
    [Params(10000)]
    public int Count { get; set; }
    private List<Person> persons = [];
    private string personsJson = "";

    private List<string> personListJson = [];

    [GlobalSetup]
    public void GlobalSetup()
    {
        var faker = new Faker<Person>()
        .RuleFor(c => c.Name, f => f.Name.FullName())
        .RuleFor(c => c.Age, f => f.Random.Int(1, 100))
        .RuleFor(c => c.DateOfBirth, f => f.Date.Recent())
        .RuleFor(c => c.IsMarried, f => f.Random.Bool())
        .RuleFor(c => c.Children, f => f.Lorem.Words(f.Random.Int(1, 5)));

        persons = faker.Generate(Count);
        personsJson = System.Text.Json.JsonSerializer.Serialize(persons);

        personListJson = persons.Select(c => System.Text.Json.JsonSerializer.Serialize(c)).ToList();
    }

    [BenchmarkCategory("DeserializeBigData"), Benchmark]
    public void JsonSerializer_Deserialize_BigData()
    {
        _ = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
    }

    [BenchmarkCategory("DeserializeBigData"), Benchmark(Baseline = true)]
    public void JsonConvert_Deserialize_BigData()
    {
        _ = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Person>>(personsJson);
    }

        [BenchmarkCategory("DeserializeSmallData"), Benchmark]
        public void JsonSerializer_Deserialize_SmallData()
        {
            foreach (var person in personListJson)
            {
                _ = System.Text.Json.JsonSerializer.Deserialize<Person>(person);
            }
        }

        [BenchmarkCategory("DeserializeSmallData"), Benchmark(Baseline = true)]
        public void JsonConvert_Deserialize_SmallData()
        {
            foreach (var person in personListJson)
            {
                _ = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(person);
            }
        }

        [BenchmarkCategory("SerializeBigData"), Benchmark]
        public void JsonSerializer_Serialize_BigData()
        {
            _ = System.Text.Json.JsonSerializer.Serialize(persons);
        }

        [BenchmarkCategory("SerializeBigData"), Benchmark(Baseline = true)]
        public void JsonConvert_Serialize_BigData()
        {
            _ = Newtonsoft.Json.JsonConvert.SerializeObject(persons);
        }

        [BenchmarkCategory("SerializeSmallData"), Benchmark]
        public void JsonSerializer_Serialize_SmallData()
        {
            foreach (var person in persons)
            {
                _ = System.Text.Json.JsonSerializer.Serialize(person);
            }
        }

        [BenchmarkCategory("SerializeSmallData"), Benchmark(Baseline = true)]
        public void JsonConvert_Serialize_SmallData()
        {
            foreach (var person in persons)
            {
                _ = Newtonsoft.Json.JsonConvert.SerializeObject(person);
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