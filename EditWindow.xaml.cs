using System.IO;
using System.Windows;
using pipeworks.app;

namespace pipeworks;

public partial class EditWindow : Window
{
    private readonly string appDataPath;
    public Game currentGame;
    private string jsonPath;

    public EditWindow(Game game)
    {
        appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pipeworks");
        jsonPath = Path.Combine(appDataPath, "games.json");

        InitializeComponent();
        game = currentGame;
        DataContext = game;
    }


    private void Save_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}