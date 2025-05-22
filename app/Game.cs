using System.Text.Json.Serialization;
using System.Windows.Media;

namespace pipeworks.app;

public class Game
{
    public string Name { get; set; } = "No name!";
    public string DisplayName { get; set; } = "No display name!";
    public string Playtime { get; set; } = "0";
    public string ExecutablePath { get; set; } = "No executable path!";
    public string JsonPath { get; set; } = "No json path!";
    // [JsonIgnore] // игнорим иначе будет ошибки идите наху
    // public ImageSource Icon { get; set; }
}