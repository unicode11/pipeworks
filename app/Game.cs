using System.Text.Json.Serialization;
using System.Windows.Media;

namespace pipeworks.app;

public class Game
{
    public string Name { get; set; }
    public string DisplayName { get; set; } 
    public string Playtime { get; set; } = "0 ч.";
    public string ExecutablePath { get; set; }
    [JsonIgnore] // игнорим иначе будет ошибки идите наху
    public ImageSource Icon { get; set; }
}