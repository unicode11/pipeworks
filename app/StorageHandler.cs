using System.IO;
using System.Text.Json;

namespace pipeworks.app;

public class StorageHandler
{
    private readonly string storagePath;

    public StorageHandler()
    {
        storagePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "pipeworks", "games"
        );

        if (!Directory.Exists(storagePath))
            Directory.CreateDirectory(storagePath);
    }

    public List<Game> LoadGames()
    {
        var games = new List<Game>();
        foreach (var file in Directory.GetFiles(storagePath, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var game = JsonSerializer.Deserialize<Game>(json);
                if (game != null) games.Add(game);
            }
            catch
            {
                // TODO: логи чтобы хуйнуть
            }
        }

        return games;
    }

    public void SaveGame(Game game)
    {
        if (game == null) return;

        string filename = GetFileName(game);
        string path = Path.Combine(storagePath, filename);
        string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(path, json);
    }

    public void DeleteGame(Game game)
    {
        string filename = GetFileName(game);
        string path = Path.Combine(storagePath, filename);

        if (File.Exists(path))
            File.Delete(path);
    }

    private string GetFileName(Game game)
    {
        // микрософт сосните хуйца со своими файлами без точек с запятой без хуйни
        var safeName = string.Join("_", game.Name.Split(Path.GetInvalidFileNameChars()));
        return $"{safeName}.json";
    }
}