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
using System.Windows.Shapes;

namespace ArcadeGameProject
{
    /// <summary>
    /// Interaction logic for PauseWindow.xaml
    /// </summary>
    public partial class PauseWindow : Window
    {
        public GameWindow GW;
        public MainWindow MW;

        public string player1;
        public string player2;

        public PauseWindow()
        {
            InitializeComponent();
        }

        private void OnClickResume(object sender, RoutedEventArgs e)
        {
            GW.gameTimer.Start();
            GW.Visibility = Visibility.Visible;
            this.Close();
        }

        private void OnClickRestartGame(object sender, RoutedEventArgs e)
        {
            GW.Close();
            GameWindow gW = new GameWindow();
            gW.Visibility = Visibility.Visible;
            gW.Playername1 = player1;
            gW.Playername2 = player2;

            this.Close();
        }

        private void OnClickGoMM(object sender, RoutedEventArgs e)
        {
            MW.Visibility = Visibility.Visible;
            GW.Close();
            this.Close();
        }
    }
}
