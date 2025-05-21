using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using Microsoft.Win32;
using pipeworks.app;
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
                SaveGames(); // ой блять какая же хуйня этот ваш c#
            }
            
            GameList.ItemsSource = null;
            GameList.ItemsSource = games;
        }

        private void SaveGames()
        {
            var json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json);
        }
        


        private void SecretButton(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            LoadGames();
        }

        private void DeleteGame(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddGame(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите исполняемый файл игры",
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
            };

            if (!(bool)openFileDialog.ShowDialog()) return; 
            
            var filePath = openFileDialog.FileName;
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            var newGame = new Game
            {
                Name = fileName,
                IconPath = "Assets/default_icon.png", // пока похуй
                ExecutablePath = filePath,
                Playtime = "0 ч."
            };

            games.Add(newGame);
            SaveGames();
            LoadGames();
        }
    }
}