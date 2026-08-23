using System.Text.Json;
using Confluent.Kafka;
using Producer.Models;
using Producer.Services;

var dataLoader = new LoadDataFromJson();

var objList = dataLoader.LoadData("Input/field_reports.json");


var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
};

using (var producer = new ProducerBuilder<Null, string>(config).Build())
{
    foreach (var obj in objList)
    {
         var jsonObj = JsonSerializer.Serialize(obj);
         var topic = obj.AssetType;
         
         var message = new Message<Null, string>
         {
             Value = jsonObj,
         };

         var result = await producer.ProduceAsync(topic, message);
         Console.WriteLine($"produce {jsonObj}");
    }
}