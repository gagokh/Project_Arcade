using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public static string checkpassword;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            ResetHigscoreButton.Visibility = Visibility.Hidden;
        }

        private void OnClickPlay(object sender, RoutedEventArgs e)
        {
            //wanneer 1 van de spelernaam velden leeg is wordt er een messagebox weergegeven
            if (NamePlayer1.Text == string.Empty || NamePlayer2.Text == string.Empty)
            {
                MessageBox.Show("Voer een naam voor beide spelers in!");
            }

            else if (NamePlayer1.Text.Contains(" ") || (NamePlayer2.Text.Contains(" ")))
            {
                MessageBox.Show("Naam mag geen spaties bevatten");
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
                GW.MW = this;
                this.Visibility = Visibility.Hidden;
                //this.Close();
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

        private void Controls_Click(object sender, RoutedEventArgs e)
        {
            ControlsWindow CW = new ControlsWindow();
            CW.Visibility = Visibility.Visible;
        }

        private void OnClickAdmin(object sender, RoutedEventArgs e)
        {

            Window1 pop = new Window1();
            pop.Visibility = Visibility.Visible;
            pop.ShowDialog();
            if (checkpassword == "admin")
            {
                ResetHigscoreButton.Visibility = Visibility.Visible;
            }
            else if(checkpassword != "admin")
            {
                MessageBoxResult Wrongpassword = MessageBox.Show("Wachtwoord onjuist");
            }

        }

        private void OnClickReset(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Reset = MessageBox.Show("Weet je zeker dat je de highscores wil resetten?", "Reset Highscore", MessageBoxButton.YesNo);
            switch (Reset)
            {
                case MessageBoxResult.Yes:

                    string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = D:\\GitProjectArcade\\ArcadeGameProject\\ArcadeGameProject\\Data\\Database1.mdf; Integrated Security = True";

                    string query = "TRUNCATE TABLE [Highscores]";

                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand();

                    try
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        command.Connection = connection;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        ResetHigscoreButton.Visibility = Visibility.Hidden;
                    }
                    catch (Exception)
                    {
                        connection.Close();
                    }

                    break;
                case MessageBoxResult.No:
                    ResetHigscoreButton.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }



        }
    }
   }
