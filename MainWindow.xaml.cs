using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Windows.Interop;
using Microsoft.Win32;
using pipeworks.app;
using IWshRuntimeLibrary;
using File = System.IO.File;
using Path = System.IO.Path;

namespace pipeworks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string jsonPath;
        private string appDataPath;
        private List<Game> games = new();
        
        public MainWindow()
        {
            
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"pipeworks");
            jsonPath = Path.Combine(appDataPath, "games.json");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
            
            
            InitializeComponent();
            LoadGames();
        }

        private void LoadGames()
        {
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                games = JsonSerializer.Deserialize<List<Game>>(json) ?? new();
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

        private ImageSource GetIcon(string path)
        {
            try
            {
                string exePath = path;

                if (Path.GetExtension(path).ToLower() == ".lnk")
                {
                    var shell = new WshShell();
                    var shortcut = (IWshShortcut)shell.CreateShortcut(path);
                    exePath = shortcut.TargetPath;
                }

                var icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                if (icon != null)
                {
                    using (var bmp = icon.ToBitmap())
                    {
                        var stream = new MemoryStream();
                        bmp.Save(stream, ImageFormat.Png);
                        stream.Seek(0, SeekOrigin.Begin);

                        var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        return decoder.Frames[0];
                    }
                }
            }
            catch { }

            return null;
        }
        
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

            if (openFileDialog.ShowDialog() == true)
            {

                var filePath = openFileDialog.FileName;
                if (games.Any(g => g.ExecutablePath == filePath))
                {
                    MessageBox.Show("Уже есть такое, пропускаю.", "Дубликат");
                    return;
                }

                var fileName = Path.GetFileNameWithoutExtension(filePath);

                var newGame = new Game
                {
                    Name = fileName,
                    DisplayName = "penis",
                    ExecutablePath = filePath,
                    Playtime = "0 ч.",
                    Icon = GetIcon(filePath)
                };

                games.Add(newGame);
                SaveGames();
                LoadGames();
            }
        }
    }
}