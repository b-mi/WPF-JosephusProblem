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

namespace JosephusProblem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Josephus js;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Run_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            js = new Josephus(this.canvas, (int)slSolds.Value);
        }



        private void slSolds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
                tbsl.Text = e.NewValue.ToString();
        }

        private void canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (js != null)
            {
                js.ScanOne();
            }
        }
    }
}
