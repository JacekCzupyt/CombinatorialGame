using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page {
        public GameManager Manager { get; }

        private readonly List<Button> GameTiles = new();
        public GamePage(GameManager manager) {
            Manager = manager;
            InitializeComponent();
            InitializeBoard(Manager.GameState.N);

            manager.MoveComplete += (_, _) => UpdateBoard();
            manager.GameComplete += UpdateBoard;
            
            Manager.PlayGame();
        }

        private void InitializeBoard(int n) {
            for(int i = 0; i < n; i++) {
                var button = new Button() { Content = (i + 1).ToString(), Style = Resources["GameTile"] as Style, Tag = i};
                button.Background = TileColors[0];
                button.Click += GameTileClick;
                GameTiles.Add(button);
                GameBoard.Children.Add(button);
            }
        }

        private static void GameTileClick(object sender, RoutedEventArgs e) {
            if ((sender as Button)?.Tag is not int index)
                throw new InvalidOperationException();
            UserGamePlayer.Instance.MakeMove(index);
        }

        private Dictionary<int, Brush> TileColors = new Dictionary<int, Brush>() {
            { 0, new SolidColorBrush(Colors.Silver) },
            { 1, new SolidColorBrush(Colors.Cyan)},
            { -1, new SolidColorBrush(Colors.PaleVioletRed)}
        };

        private void UpdateBoard() {
            var state = Manager.GameState.GameList;
            for (int i = 0; i < GameTiles.Count; i++) {
                GameTiles[i].Background = TileColors[state[i]];
            }
        }
        
        private void UpdateBoard(VictoryState victory) {
            if (victory.GameEnded && victory.Winner != 0) {
                for (int i = 0; i < Manager.GameState.K; i++) {
                    int tile = victory.SequenceStart.Value + victory.SequenceJump.Value * i;
                    GameTiles[tile].BorderThickness = new Thickness(10);
                    GameTiles[tile].BorderBrush = new SolidColorBrush(Colors.Yellow);
                }
            }
        }
    }
}
