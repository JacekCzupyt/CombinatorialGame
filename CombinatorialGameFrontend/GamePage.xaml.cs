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

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page {
        public GameManager Manager { get; }
        public GamePage(GameManager manager) {
            Manager = manager;
            InitializeComponent();
            InitializeBoard(Manager.GameState.N);
        }

        private void InitializeBoard(int n) {
            for(int i = 0; i < n; i++) {
                GameBoard.Children.Add(new Button() { Content = i.ToString(), Style = Resources["GameTile"] as Style });
            }
        }
    }
}
