using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for GameConfigPage.xaml
    /// </summary>
    public partial class GameConfigPage : Page {
        private Action<(GameManager, GamePauseBehaviour[])> StartGame { get; }

        private const int MaxN = 100;
        private const int MaxK = 100;

        public enum GamePauseBehaviour {
            Pause,
            Resume
        }

        private class PlayerInitializer {
            public string Name { get; set; }
            public Func<IGamePlayer> PlayerFactory { get; set; }
            public GamePauseBehaviour GamePauseBehaviour { get; set; }
        }

        private readonly List<PlayerInitializer> AvailablePlayers = new() {
            new PlayerInitializer() {
                Name = "Human Player",
                PlayerFactory = (() => UserGamePlayer.Instance),
                GamePauseBehaviour = GamePauseBehaviour.Resume
            },
            new PlayerInitializer {
                Name = "Simple MinMax",
                PlayerFactory = (() => new MinMaxAiPlayer()),
                GamePauseBehaviour = GamePauseBehaviour.Pause
            },
            new PlayerInitializer {
                Name = "Analytical MinMax",
                PlayerFactory = (() => new AnalyticalGamePlayer()),
                GamePauseBehaviour = GamePauseBehaviour.Pause
            },
            new PlayerInitializer {
                Name = "MonteCarlo V1",
                PlayerFactory = (() => new MonteCarloAiPlayer()),
                GamePauseBehaviour = GamePauseBehaviour.Pause
            },
            new PlayerInitializer {
                Name = "MonteCarlo V2",
                PlayerFactory = (() => new MonteCarloMinMaxPlayer()),
                GamePauseBehaviour = GamePauseBehaviour.Pause
            }
        };

        public GameConfigPage(Action<(GameManager, GamePauseBehaviour[])> startGame) {
            StartGame = startGame;
            InitializeComponent();
            InitializePlayerSelection();
        }

        private void InitializePlayerSelection() {
            Player1Box.ItemsSource = AvailablePlayers;
            Player1Box.DisplayMemberPath = "Name";
            Player1Box.SelectedIndex = 0;

            Player2Box.ItemsSource = AvailablePlayers;
            Player2Box.DisplayMemberPath = "Name";
            Player2Box.SelectedIndex = 0;
        }

        //source: https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text) {
            return !_regex.IsMatch(text);
        }

        // Use the PreviewTextInputHandler to respond to key presses 
        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e) {
            e.Handled = !IsTextAllowed(e.Text);
        }

        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e) {
            if (e.DataObject.GetDataPresent(typeof(String))) {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) {
                    e.CancelCommand();
                }
            }
            else {
                e.CancelCommand();
            }
        }

        private void StartDemo_OnClick(object sender, RoutedEventArgs e) {
            ErrorText.Text = null;
            try {
                StartGame(ValidateInputs());
            }
            catch (ArgumentException err) {
                ErrorText.Text = err.Message;
            }
        }

        private (GameManager, GamePauseBehaviour[]) ValidateInputs(bool demoMode = true) {
            int n;
            try {
                n = int.Parse(NTextbox.Text);
            }
            catch {
                throw new ArgumentException("N is not a valid integer");
            }
            if (n is <= 0 or > MaxN)
                throw new ArgumentException($"N must be between 1 and {MaxN}");

            int k;
            try {
                k = int.Parse(KTextbox.Text);
            }
            catch {
                throw new ArgumentException("K is not a valid integer");
            }
            if (k is <= 0 or > MaxK)
                throw new ArgumentException($"K must be between 1 and {MaxK}");
            if (k > n)
                throw new ArgumentException("k must not be greater then n");


            var player1Factory = Player1Box.SelectedItem as PlayerInitializer;
            var player2Factory = Player2Box.SelectedItem as PlayerInitializer;

            var player1 = player1Factory!.PlayerFactory();
            var player2 = player2Factory!.PlayerFactory();

            if ((player1 is MinMaxAiPlayer || player2 is MinMaxAiPlayer) && n > 11)
                throw new ArgumentException(
                    "Simple MinMax is the simplest AI, which literally solves the provided game. " +
                    "Using it on N greater then 11 would result in extremely long loading times."
                );

            if (!demoMode && (player1 is UserGamePlayer || player2 is UserGamePlayer))
                throw new ArgumentException("Human player is not allowed in test mode.");

            return (new GameManager(
                player1,
                player2,
                new SimpleGameController(n, k),
                demoMode
            ), new[] { player1Factory!.GamePauseBehaviour, player2Factory!.GamePauseBehaviour });
        }

        private int GetNumberOfGames() {
            int g;
            try {
                g = int.Parse(NGamesBox.Text);
            }
            catch {
                throw new ArgumentException("Number of games is not a valid integer");
            }
            if (g <= 0)
                throw new ArgumentException("Number of games must be greater then zero");

            return g;
        }


        private async void StartTest_OnClick(object sender, RoutedEventArgs e) {
            ErrorText.Text = null;
            try {
                var manager = ValidateInputs(false).Item1;
                var gameCount = GetNumberOfGames();
                
                await InitializeTestGames(manager, gameCount);
            }
            catch (ArgumentException err) {
                ErrorText.Text = err.Message;
            }
        }

        private CancellationTokenSource testCancellationTokenSource;

        private async Task InitializeTestGames(GameManager manager, int gameCount) {
            StartTestButton.IsEnabled = false;
            StartDemoButton.IsEnabled = false;
            ProgressBarGrid.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            ProgressBarText.Text = $"0 / {gameCount}";
            CancelButton.Visibility = Visibility.Visible;
            
            testCancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<(int, int, int, int)>();
            progress.ProgressChanged += (sender, data) => {
                var (_games, _wins, _losses, _ties) = data;
                ProgressBar.Value = 100f * _games / gameCount;
                ProgressBarText.Text = $"{_games} / {gameCount}";
                ResultText.Text = $"Player 1 wins: {_wins}\nPlayer 2 wins: {_losses}\nTies: {_ties}";
            };

            var (wins, losses, ties) = await TestGames(manager, gameCount, testCancellationTokenSource.Token, progress);
            
            StartTestButton.IsEnabled = true;
            StartDemoButton.IsEnabled = true;
            ProgressBarGrid.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            
            ResultText.Text = $"Player 1 wins: {wins}\nPlayer 2 wins: {losses}\nTies: {ties}";
        }

        private async Task<(int, int, int)> TestGames(GameManager manager, int gameCount, CancellationToken token, IProgress<(int, int, int, int)> progress) {
            int wins = 0, losses = 0, ties = 0;

            token.Register(
                () => {
                    try {
                        manager.CancelGame();
                    }
                    catch (InvalidOperationException) {
                    }
                }
            );

            for (int i = 0; i < gameCount && !token.IsCancellationRequested; i++) {
                var victory = await manager.PlayGame();
                switch (victory.Winner) {
                    case 1:
                        wins++;
                        break;
                    case -1:
                        losses++;
                        break;
                    case 0:
                        ties++;
                        break;
                }
                manager.RestartGame();
                progress.Report((i + 1, wins, losses, ties));
            }

            return (wins, losses, ties);
        }
        private void CancelButton_OnClick(object sender, RoutedEventArgs e) {
            testCancellationTokenSource?.Cancel();
        }
    }
}
