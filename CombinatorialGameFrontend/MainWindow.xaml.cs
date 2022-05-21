using System.Windows;
using CombinatorialGameLibrary.GameManagement;

namespace CombinatorialGameFrontend {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private GameConfigPage configPage;
        public MainWindow() {
            InitializeComponent();
            configPage = new GameConfigPage(SwitchToGameScene);
            MainFrame.Navigate(configPage);
        }

        private void SwitchToGameScene((GameManager, GameConfigPage.GamePauseBehaviour[]) gameSettings) {
            MainFrame.Navigate(new GamePage(gameSettings, ReturnToConfigScene));
        }

        private void ReturnToConfigScene() {
            MainFrame.Navigate(configPage);
        }
    }
}
