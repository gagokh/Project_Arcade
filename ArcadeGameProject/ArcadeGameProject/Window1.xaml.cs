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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }


        private void OnClickGoMM(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Visibility = Visibility.Visible;
        }

        private void OnClickResume(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Visibility = Visibility.Visible;
        }
        private void OnClickRestartGame(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Visibility = Visibility.Visible;
        }
    }
}
