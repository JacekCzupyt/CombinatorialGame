using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private GameManager Manager { get; }

        private readonly List<Button> GameTiles = new();
        private GameConfigPage.GamePauseBehaviour[] PauseBehaviour { get; }

        private Task<VictoryState> _gameTask;
        public GamePage((GameManager, GameConfigPage.GamePauseBehaviour[]) gameSettings) {
            Manager = gameSettings.Item1;
            PauseBehaviour = gameSettings.Item2;
            InitializeComponent();
            InitializeBoard(Manager.GameState.N);

            Manager.MoveComplete += (player, _) => HandleMove(player);
            Manager.GameComplete += UpdateBoard;
            
            PlayGame();
        }

        private async void PlayGame() {
            VictoryText.Text = null;
            
            _gameTask = Manager.PlayGame();
            // CLR/System.ArgumentOutOfRangeException when debbuging
            var victoryState = await _gameTask;

            if (!victoryState.GameEnded)
                return;
            
            if (victoryState.Winner == 1)
                VictoryText.Text = "Player 1 has won";
            else if(victoryState.Winner == -1)
                VictoryText.Text = "Player 2 has won";
            else {
                VictoryText.Text = "The game ended in a tie";
            }
        }

        private void InitializeBoard(int n) {
            for (int i = 0; i < n; i++) {
                var button = new Button() { Content = (i + 1).ToString(), Style = Resources["GameTile"] as Style, Tag = i };
                button.Background = new SolidColorBrush(TileColors[0]);
                button.Click += GameTileClick;
                button.IsHitTestVisible = false;
                GameTiles.Add(button);
                GameBoard.Children.Add(button);
            }
            UserGamePlayer.Instance.NotifyMove += EnableButtons;
        }

        private void EnableButtons(MoveRequest request) {
            for (int i = 0; i < GameTiles.Count; i++) {
                if (request.GameState.GameList[i] == 0) {
                    GameTiles[i].IsHitTestVisible = true;
                }
            }
        }

        private void GameTileClick(object sender, RoutedEventArgs e) {
            if ((sender as Button)?.Tag is not int index)
                throw new InvalidOperationException();

            try {
                UserGamePlayer.Instance.MakeMove(index);
                foreach(var tile in GameTiles) {
                    tile.IsHitTestVisible = false;
                }
            }
            catch {
                // ignored
            }
        }

        private Dictionary<int, Color> TileColors = new() {
            { 0, Colors.Silver },
            { 1, Colors.Cyan },
            { -1, Colors.PaleVioletRed }
        };

        private void HandleMove(int player) {
            UpdateBoard();
            HandlePause(player);
        }
        
        private void UpdateBoard() {
            var state = Manager.GameState.GameList;
            for (int i = 0; i < GameTiles.Count; i++) {
                GameTiles[i].Background = new SolidColorBrush(TileColors[state[i]]);
            }
        }

        private void HandlePause(int player) {
            if (Manager.GamePaused) {
                var behaviour = PauseBehaviour[player == 1 ?
                    1 :
                    0];

                switch (behaviour) {
                    case GameConfigPage.GamePauseBehaviour.Pause:
                        break;
                    case GameConfigPage.GamePauseBehaviour.Resume:
                        Manager.ResumeGame();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            ResumeButton.IsEnabled = Manager.GamePaused;
        }

        private void UpdateBoard(VictoryState victory) {
            if (victory.GameEnded && victory.Winner != 0) {
                for (int i = 0; i < Manager.GameState.K; i++) {
                    int tileIndex = victory.SequenceStart.Value + victory.SequenceJump.Value * i;
                    var tile = GameTiles[tileIndex];
                    tile.Background = GenerateStripedBrush(TileColors[-victory.Winner.Value], Colors.Yellow);
                }
            }
        }

        private static Brush GenerateStripedBrush(Color color1, Color color2) {
            LinearGradientBrush brush = new LinearGradientBrush();

            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(1, 1);
            brush.SpreadMethod = GradientSpreadMethod.Repeat;

            brush.GradientStops = new GradientStopCollection(
                new[] {
                    new GradientStop(color1, 0),
                    new GradientStop(color1, 0.6),
                    new GradientStop(color2, 0.6),
                    new GradientStop(color2, 1),
                }
            );

            brush.RelativeTransform = new ScaleTransform(0.1, 0.1);
            return brush;
        }
        
        private void NextButton_OnClick(object sender, RoutedEventArgs e) {
            if(Manager.GamePaused)
                Manager.ResumeGame();
        }
        private async void RestartButton_OnClick(object sender, RoutedEventArgs e) {
            if (Manager.GameInProgress) {
                Manager.CancelGame();
                await _gameTask;
            }
            Manager.RestartGame();
            PlayGame();
            UpdateBoard();
        }
    }
}
