using data;
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

namespace view
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage : UserControl
    {
        public Statistic statistic;
        public StatsPage()
        {
            InitializeComponent();
            Statistic statisticToDispay = new(100);
            statistic = statisticToDispay;
            StatList.DataContext = statistic;
            AverageMain.Content = "Average Main Path level: " + statistic.AverageMainPathLevel;
            AverageCross.Content = "Average Cross Path level: " + statistic.AverageCrossPathLevel;

        }

        private void StatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
