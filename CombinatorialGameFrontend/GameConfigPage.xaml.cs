using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for GameConfigPage.xaml
    /// </summary>
    public partial class GameConfigPage : Page {
        
        private const int MaxN = 10;
        private const int MaxK = 5;

        private struct PlayerInitializer {
            public string Name { get; set; }
            public Func<IGamePlayer> PlayerFactory { get; set; }
        }

        private readonly List<PlayerInitializer> AvailablePlayers = new() {
            new PlayerInitializer {Name = "Player", PlayerFactory = (() => null)},
            new PlayerInitializer {Name = "MinMax", PlayerFactory = (() => new MinMaxAiPlayer())}
        };

        public GameConfigPage() {
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
        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e) 
        { 
            e.Handled = !IsTextAllowed(e.Text); 
        } 
        
        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void SubmitButton_OnClick(object sender, RoutedEventArgs e) {
            try {
                ValidateInputs();
            }
            catch (Exception err) {
                ErrorText.Text = err.Message;
            }
        }

        private GameManager ValidateInputs() {
            int n;
            try {
                n = int.Parse(NTextbox.Text);
            }
            catch {
                throw new Exception("N is not a valid integer");
            }
            if (n is <= 0 or > MaxN)
                throw new Exception($"N must be between 1 and {MaxN}");
            
            int k;
            try {
                k = int.Parse(KTextbox.Text);
            }
            catch {
                throw new Exception("K is not a valid integer");
            }
            if (k is <= 0 or > MaxK)
                throw new Exception($"K must be between 1 and {MaxK}");

            return null;
        }
    }
}
