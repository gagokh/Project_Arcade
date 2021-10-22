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
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ArcadeGameProject
{
    /// <summary>
    /// Interaction logic for HighScores.xaml
    /// </summary>
    public partial class HighScores : Window
    {
        public MainWindow MW;

        public HighScores()
        {
            InitializeComponent();

            //database connection
            string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = D:\\GitProjectArcade\\ArcadeGameProject\\ArcadeGameProject\\Data\\Database1.mdf; Integrated Security = True";
            SqlConnection connection = new SqlConnection(connectionString);

            try //om een exception te vermijden doen we try catch. Error ontstaat alleen als db leeg is.
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 CAST(Highscore AS INT) AS TheMax, Player, Id FROM Highscores ORDER BY TheMax DESC"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    connection.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {

                        for (int i = 0; i < 5; i++)
                        {
                            sdr.Read();
                            txtId.Text += "\r 0" + sdr["Id"].ToString();
                            txtPlayerName.Text += "\r" + sdr["Player"].ToString();
                            txtScore.Text += "\r" + sdr["TheMax"].ToString();
                        }
                    }
                    connection.Close();
                }
            }
            catch
            {  

            }
          }


        private void OnClickClose(object sender, RoutedEventArgs e)
        {
            MW.Visibility = Visibility.Visible;
            this.Close();
        }


        ////query
        //SqlCommand cmd = new SqlCommand("Select Player, Highscore from highscores_Table ", connection);

        //connection.Open();
        //DataTable dt = new DataTable(); //define data table
        //dt.Load(cmd.ExecuteReader()); //load the result of the reader inside the data table
        //connection.Close();

        ////dtGrid.DataContext = dt; //bind loaded data with datagrid (xaml)
        ///
        //int count = sdr.FieldCount;
        //            do
        //            {
        //                sdr.Read();
        //                txtId.Text += "\r 0" + sdr["Id"].ToString();
        //txtPlayerName.Text += "\r" + sdr["Player"].ToString();
        //txtScore.Text += "\r" + sdr["Highscore"].ToString();
        //count++;
        //            } while (count< 5);

    }
}




