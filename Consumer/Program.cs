using System.Text.Json;
using Confluent.Kafka;
using Consumer.Data;
using Consumer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer.Models;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

var connectionString =
    configuration.GetConnectionString("DefaultConnection");

services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

var serviceProvider = services.BuildServiceProvider();

var uavTopics = configuration["Kafka:Topics:UAV"];
var perimeterSensorTopics = configuration["Kafka:Topics:PerimeterSensor"];
var groupId = configuration["Kafka:GroupId"];
var bootstrapServers = configuration["Kafka:BootstrapServer"];

var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
{
    consumer.Subscribe([perimeterSensorTopics, uavTopics]);

    var nullCount = 0;

    while (nullCount < 5)
    {
        try
        {
            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

            if (consumeResult == null)
            {
                nullCount++;
                continue;
            }

            nullCount = 0;

            var jsonObj = consumeResult.Message.Value;

            Console.WriteLine($"Consume{jsonObj}");

            var obj = JsonSerializer.Deserialize<AssetLiveReport>(jsonObj);

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var assetProcessor = new AssetProcess(dbContext);

                var objSaved = await assetProcessor.Process(obj);

                if (objSaved)
                {
                    Console.WriteLine($"Object {obj.AssetId} is saved to the database.");
                }
                else
                {
                    Console.WriteLine($"Object {obj.AssetId} was not saved to the database.");
                }
            }
            consumer.Commit(consumeResult);
        }
        catch (JsonException)
        {
            Console.WriteLine("Json Exception");
        }
        catch (KafkaException e)
        {
            Console.WriteLine(e);
        }
    }
}