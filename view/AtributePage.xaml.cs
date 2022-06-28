using data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace view
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AtributePage : UserControl
    {
        public AtributePage()
        {
            InitializeComponent();
            AtributeXmlList.ItemsSource = Atribute.AllAtributes;
            CollectionViewSource.GetDefaultView(Atribute.AllAtributes).Refresh();
        }

        private void Atribute_Remove(object sender, RoutedEventArgs e)
        {
            sender = ((MenuItem)sender).CommandParameter as Atribute; ;
            Atribute.AllAtributes.Remove((Atribute)sender);
            AtributeXmlList.ItemsSource = Atribute.AllAtributes;
            CollectionViewSource.GetDefaultView(Atribute.AllAtributes).Refresh();
        }

        private void Add_Atribute(object sender, RoutedEventArgs e)
        {
            MainWindow window = Window.GetWindow(this) as MainWindow;
            window.CloseControl(this);
            AddAtributePage addAtributePage = new();
            window.OpenControl(addAtributePage);
        }

        private void Save_Atribute(object sender, RoutedEventArgs e)
        {
            AtributeJSON.WriteJSON();
        }

        private void Load_Atribute(object sender, RoutedEventArgs e)
        {
            Controller.DebugLogging1.CompareTo(true); // this is to force controller to load is a hack
            AtributeXmlList.ItemsSource = Atribute.AllAtributes;
           CollectionViewSource.GetDefaultView(Atribute.AllAtributes).Refresh();
        }
    }
}

// debug code
//  Atribute.AllAtributes.Add(new Atribute("test", "test", new MagicPath[0], 2, 2, 2, 2, 2, 214));
// CollectionViewSource.GetDefaultView(AtributeXmlList.ItemsSource).Refresh();