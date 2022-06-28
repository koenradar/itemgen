using data;
using System;
using System.Windows;
using System.Windows.Controls;

namespace view
{
    /// <summary>
    /// Interaction logic for AddAtributePage.xaml
    /// </summary>
    public partial class AddAtributePage : UserControl
    {
        public Atribute Atribute { get; set; } = new Atribute("", "", new MagicPath[2], null, 0, 0, 0, 1, 0, 0, 0);

        public AddAtributePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Atribute.AllAtributes.Add(Atribute);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Atribute.Name = Name.Text;
            Atribute.DomminionsModTag = DomminionsModTag.Text;

            int.TryParse(BasePower.Text, out int basePower);
            Atribute.BasePower = basePower;

            int.TryParse(DefaultCost.Text, out int defaultPointCost);
            Atribute.DefaultPointCost = defaultPointCost;

            int.TryParse(Weight.Text, out int weight);
            Atribute.Weight = weight;

            int.TryParse(ScalingCost.Text, out int scalingCost);
            Atribute.ScalingCost = scalingCost;

            int.TryParse(PowerScaling.Text, out int powerScaling);
            Atribute.PowerScaling = powerScaling;

            Enum.TryParse<MagicPath>(PathAffinity.Text, out MagicPath result);
            MagicPath[] magicPaths = new MagicPath[1];
            magicPaths[0] = result;
            Atribute.PathAffinity = magicPaths;
        }
    }
}