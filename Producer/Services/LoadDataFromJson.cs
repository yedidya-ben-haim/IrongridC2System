using System.Text.Json;
using Producer.Models;

namespace Producer.Services;

public class LoadDataFromJson
{
    public List<AssetLiveReport>? LoadData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }
        var jsonText = File.ReadAllText(filePath);

        var objList = JsonSerializer.Deserialize<List<AssetLiveReport>>(jsonText);

        return objList;
    }
}