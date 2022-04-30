using System.Windows;
using CombinatorialGameLibrary.GameManagement;

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            MainFrame.Navigate(new GameConfigPage(SwitchToGameScene));
        }

        private void SwitchToGameScene((GameManager, GameConfigPage.GamePauseBehaviour[]) gameSettings) {
            MainFrame.Navigate(new GamePage(gameSettings));
        }
    }
}
