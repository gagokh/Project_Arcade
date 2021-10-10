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
        public HighScores()
        {
            InitializeComponent();

            //database connection
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Projectarcade\\ArcadeGameProject\\ArcadeGameProject\\Data\\Database1.mdf;Integrated Security=True";

            SqlConnection connection = new SqlConnection(connectionString);

            //query
            SqlCommand cmd = new SqlCommand("Select Player, Highscore from highscores_Table ", connection);

            connection.Open();
            DataTable dt = new DataTable(); //define data table
            dt.Load(cmd.ExecuteReader()); //load the result of the reader inside the data table
            connection.Close();

            //dtGrid.DataContext = dt; //bind loaded data with datagrid (xaml)
        }
      }
    }




