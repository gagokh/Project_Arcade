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

namespace ArcadeGameProject
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#region Variabelen
        String Name1;
        String Name2;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClickPlay(object sender, RoutedEventArgs e)
        {
            //wanneer 1 van de spelernaam velden leeg is wordt er een messagebox weergegeven
            if (NamePlayer1.Text == string.Empty || NamePlayer2.Text == string.Empty)
            {
                MessageBox.Show("Voer een naam voor beide spelers in!");
            }

            else if (NamePlayer1.Text.Length > 10 || NamePlayer2.Text.Length > 10)
            {
                MessageBox.Show("Playername mag niet langer dan 10 tekens zijn");
            }
            else
            {
                //opent de Gamewindow
                GameWindow GW = new GameWindow();
                GW.Visibility = Visibility.Visible;
                String Name1 = NamePlayer1.Text;
                String Name2 = NamePlayer2.Text;
                GW.Playername1 = Name1;
                GW.Playername2 = Name2;
            }

   
        }

        private void OnClickExit(object sender, RoutedEventArgs e)
        {
            //sluit de applicatie 
            Application.Current.Shutdown();
        }

        private void Highscorebutton_Click(object sender, RoutedEventArgs e)
        {
            //opent highscores 
            HighScores HS = new HighScores();
            HS.Visibility = Visibility.Visible;
            HS.MW = this;
            this.Visibility = Visibility.Hidden;
        }
      }
    }
