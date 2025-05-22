using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using pipeworks.app;
using Path = System.IO.Path;

namespace pipeworks;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly StorageHandler _storage;
    private Game _currentGame;
    private List<Game> _games = new();

    public MainWindow()
    {
        _storage = new StorageHandler();

        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pipeworks");
        if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
        InitializeComponent();
        _storage.LoadGames();
        DataContext = _currentGame; // иди просто нахуй как ты работаешь я не понимаю блятьт
        Reload();
    }


    private void SecretButton(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("ᗜ˰ᗜ", "ᗜ˰ᗜ");
    }

    private void LoadGames()
    {
        _games = _storage.LoadGames();
        GameList.ItemsSource = null;
        GameList.ItemsSource = _games;
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

        _games.Remove(game);
        _storage.DeleteGame(game);
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
        if (_games.Any(g => g.ExecutablePath == filePath))
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

        _games.Add(newGame);
        _storage.SaveGame(newGame);
        LoadGames();
    }

    private void PlayGame(object sender, RoutedEventArgs e)
    {
        _currentGame = (Game)GameList.SelectedItem;
        if (_currentGame == null) return;

        var psi = new ProcessStartInfo
        {
            FileName = _currentGame.ExecutablePath,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void BrowseFiles(object sender, RoutedEventArgs e)
    {
        _currentGame = (Game)GameList.SelectedItem;
        var folder = Path.GetDirectoryName(_currentGame.ExecutablePath);
        
        if (folder == null) return;

        Process.Start("explorer.exe", folder);
    }

    private void GameClicked(object sender, MouseButtonEventArgs e)
    {
        _currentGame = (Game)GameList.SelectedItem;
        if (_currentGame == null) return;

        RightDescription.Text = _currentGame.Name;
        RightTitle.Text = _currentGame.DisplayName;
        RightInfo.Text = _currentGame.ExecutablePath;
        HoursSpent.Text = _currentGame.Playtime;
    }

    private void EditInstance(object sender, RoutedEventArgs e)
    {
        _currentGame = (Game)GameList.SelectedItem;
        if (_currentGame == null) return;

        var editWindow = new EditWindow(_currentGame);
        editWindow.Owner = this;

        if (editWindow.ShowDialog() == true)
        {
            _storage.SaveGame(_currentGame);
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

        var newCulture = currentCulture == "ru" ? "en" : "ru";

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(newCulture);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(newCulture);

        var newWindow = new MainWindow();
        Application.Current.MainWindow = newWindow;
        newWindow.Show();
        Close();
    }

    private void GetJson(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}