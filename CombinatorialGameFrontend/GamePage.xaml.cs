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

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page {
        public GamePage() {
            InitializeComponent();
            InitializeBoard(10);
        }

        private void InitializeBoard(int n) {
            for(int i = 0; i < n; i++) {
                GameBoard.Children.Add(new Button() { Content = i.ToString(), Style = Resources["GameTile"] as Style });
            }
        }
    }
}
