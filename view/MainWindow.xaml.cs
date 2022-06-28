using System;
using System.Diagnostics;
using data;
using System.Windows;
using System.Windows.Controls;

namespace view
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OpenControl(UserControl control)
        {
            grControls.Children.Add(control);
        }

        public void CloseControl(UserControl control)
        {
            grControls.Children.Remove(control);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AmountOfItems.Text, out int amountResult) && int.TryParse(ScalingIncrement.Text, out int scalingResult) && int.TryParse(DefaultPoints.Text, out int defaultResult) && int.TryParse(PointMultiplier.Text, out int multiplierResult) && MagicGen.IsChecked.HasValue && Debugging.IsChecked.HasValue)
            {
                try
                {
                    
                    bool magicGenIsChecked = MagicGen.IsChecked.Value;
                    bool debuggingIsChecked = Debugging.IsChecked.Value;
                    Controller.Settings(amountResult, magicGenIsChecked, debuggingIsChecked, scalingResult, MagicGenName.Text, defaultResult, multiplierResult);
                    Controller.GenerateMod();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
            else
            {
                try
                {
                    Controller.Settings();
                    Controller.GenerateMod();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        private void ShowHomescreen(object sender, RoutedEventArgs e)
        {
            grControls.Children.Clear();
            Visibility = Visibility.Hidden;
            new MainWindow().Show();
        }

        private void Atributes_Click(object sender, RoutedEventArgs e)
        {
            grControls.Children.Clear();
            AtributePage atributePage = new();
            OpenControl(atributePage);
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            grControls.Children.Clear();
            StatsPage statsPage = new();
            OpenControl(statsPage);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(MagicGen))
            {
                MagicGenName.IsEnabled = !MagicGenName.IsEnabled;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }
    }
}