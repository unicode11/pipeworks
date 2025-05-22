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
    private readonly StorageHandler storage;

    public MainWindow()
    {
        storage = new StorageHandler();
        
        appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pipeworks");
        if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
        InitializeComponent();
        storage.LoadGames();
        DataContext = currentGame; // иди просто нахуй как ты работаешь я не понимаю блятьт
    }

    
    private void SecretButton(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("ᗜ˰ᗜ", "ᗜ˰ᗜ");
    }

    private void LoadGames()
    {
        games = storage.LoadGames();
        GameList.ItemsSource = null;
        GameList.ItemsSource = games;
    }
    private void Refresh(object sender, RoutedEventArgs e)
    {
        Reload();
    }

    private void Reload()
    {
        LoadGames();
    }

    private void DeleteGame(object sender, RoutedEventArgs e)
    {
        if (GameList.SelectedItem is not Game game) return;
        
        games.Remove(game);
        storage.DeleteGame(game);
        LoadGames();
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
        var name = fileName;
        var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
        if (!string.IsNullOrWhiteSpace(versionInfo.FileDescription))
            name = versionInfo.FileDescription;

        var newGame = new Game
        {
            Name = fileName,
            DisplayName = name,
            ExecutablePath = filePath,
            Playtime = "0"
        };

        games.Add(newGame);
        storage.SaveGame(newGame);
        LoadGames();
    }

    private void PlayGame(object sender, RoutedEventArgs e)
    {
        currentGame = (Game)GameList.SelectedItem;
        if (currentGame == null) return;

        var psi = new ProcessStartInfo
        {
            FileName = currentGame.ExecutablePath,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void BrowseFiles(object sender, RoutedEventArgs e)
    {
        currentGame = (Game)GameList.SelectedItem;
        if (currentGame == null) return;

        var folder = Path.GetDirectoryName(currentGame.ExecutablePath);
        Process.Start("explorer.exe", folder);
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

        var editWindow = new EditWindow(currentGame);
        editWindow.Owner = this;

        if (editWindow.ShowDialog() == true)
        {
            storage.SaveGame(currentGame);
            LoadGames();
        }
    }

    private void GetDescription(object sender, RoutedEventArgs e)
    {
        // todo
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