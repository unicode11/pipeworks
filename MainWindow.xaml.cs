using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using pipeworks.app;
using System.Globalization;
using File = System.IO.File;
using Path = System.IO.Path;

namespace pipeworks;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string appDataPath;
    public Game currentGame;
    public List<Game> games = new();
    private readonly string jsonPath;

    public MainWindow()
    {
        appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pipeworks");
        jsonPath = Path.Combine(appDataPath, "games.json");
        if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
        InitializeComponent();
        LoadGames();
        DataContext = currentGame; // иди просто нахуй как ты работаешь я не понимаю блятьт
    }

    private void LoadGames()
    {
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            games = JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();
        }
        else
        {
            games = new List<Game>();
            SaveGames();
        }

        GameList.ItemsSource = null;
        GameList.ItemsSource = games;
    }

    // private string ResolveShortCut(string shortcutPath) // тупые блять микрософты вы не могли сделать так, чтобы оно сразу возвращало путь к оригиналу нахуя мне эту функцию делать
    // {
    //     if (!File.Exists(shortcutPath) || Path.GetExtension(shortcutPath).ToLower() != ".lnk")
    //         return shortcutPath;
    //
    //     Type shellType = Type.GetTypeFromProgID("WScript.Shell");
    //     dynamic shell = Activator.CreateInstance(shellType);
    //     dynamic shortcut = shell.CreateShortcut(shortcutPath);
    //
    //     return shortcut.TargetPath;
    // }

    private void SaveGames()
    {
        var json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json);
    }

    private void SecretButton(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("ᗜ˰ᗜ", "ᗜ˰ᗜ");
    }

    private void Refresh(object sender, RoutedEventArgs e)
    {
        Reload();
    }

    private void Reload()
    {
        SaveGames();
        LoadGames();
    }

    private void DeleteGame(object sender, RoutedEventArgs e)
    {
        if (GameList.SelectedItem == null) return;
        if (GameList.SelectedItem is Game game)
        {
            games.Remove(game);
            Reload();
        }
    }

    private void AddGame(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Выберите исполняемый файл игры",
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == false) return;

        var filePath = openFileDialog.FileName;
        if (games.Any(g => g.ExecutablePath == filePath))
        {
            MessageBox.Show(Strings.DuplicateDesc, Strings.DuplicateTitle);
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var name = fileName; // на случай если нет названия
        var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
        Console.WriteLine(versionInfo.FileDescription);
        if (!string.IsNullOrWhiteSpace(versionInfo.FileDescription))
            name = versionInfo.FileDescription;
        else if (name == fileName)
            name = "";

        var newGame = new Game
        {
            Name = fileName,
            DisplayName = name,
            ExecutablePath = filePath,
            Playtime = "0 ч."
        };

        games.Add(newGame);
        SaveGames();
        LoadGames();
    }

    private void PlayGame(object sender, RoutedEventArgs e)
    {
        currentGame = (Game)GameList.SelectedItem;
        if (currentGame == null) return;
        var path = currentGame.ExecutablePath;
        var psi = new ProcessStartInfo
        {
            FileName = $@"{path}",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void BrowseFiles(object sender, RoutedEventArgs e)
    {
        currentGame = (Game)GameList.SelectedItem;
        if (currentGame == null) return;
        var folder = Path.GetDirectoryName(currentGame.ExecutablePath);
        Process.Start("explorer.exe", $@"{folder}");
    }

    private void GameClicked(object sender, MouseButtonEventArgs e)
    {
        currentGame = (Game)GameList.SelectedItem;
        if (currentGame == null) return;

        RightDescription.Text = currentGame.Name;
        RightTitle.Text = currentGame.DisplayName;
        RightInfo.Text = currentGame.ExecutablePath;
        HoursSpent.Text = currentGame.Playtime;
    }

    private void EditInstance(object sender, RoutedEventArgs e)
    {
        currentGame = (Game)GameList.SelectedItem;
        if (currentGame == null) return;

        var button = sender as Button;
        if (currentGame != null)
        {
            var editWindow = new EditWindow(currentGame);
            editWindow.Owner = this;
            editWindow.ShowDialog();
            GameList.Items.Refresh();
        }
    }

    private void GetDescription(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ChangeLanguage(object sender, RoutedEventArgs e)
    {
        var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

        string newCulture = currentCulture == "ru" ? "en" : "ru";

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(newCulture);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(newCulture);

        var newWindow = new MainWindow();
        Application.Current.MainWindow = newWindow;
        newWindow.Show();
        this.Close();
    }
}