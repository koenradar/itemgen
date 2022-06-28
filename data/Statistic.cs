using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
    public class Statistic
    {
        private static readonly int AmountOfItems = Controller.AmountOfItems;

        private List<Item> Items = new List<Item>();

        public int[] AmountsPerLevel { get; set; } = new int[5];

        public double[] MainPathPerLevel { get; set; } = new double[5];

        public double[] CrossPathPerLevel { get; set; } = new double[5];

        public double[] AttributesPerLevel { get; set; } = new double[5];

        public double AverageMainPathLevel = 0;

        public double AverageCrossPathLevel = 0;

        public Statistic(int amountOfGenerations)
        {
            for (int i = 0; i < amountOfGenerations * AmountOfItems; i++)
            {
                Items.Add(Item.GenerateItem());
            }

            for (int i = 0; i < AmountsPerLevel.Length; i++) // bugged inaccurate stats
            {
                AmountsPerLevel[i] = Items.Count(x => x.ConstLevel == (i * 2)) / amountOfGenerations;
                MainPathPerLevel[i] = Items.Where(x => x.ConstLevel == (i * 2)).Average(y => y.MainPathLevel);
                CrossPathPerLevel[i] = (double)Items.Where(x => x.ConstLevel == (i * 2)).Average(y => y.CrossPathLevel);
                AttributesPerLevel[i] = Items.Where(x => x.ConstLevel == (i * 2)).Average(z => z.AtributeList.Count(y => y.DomminionsModTag != "#itemcost" && y.DomminionsModTag != "#itemcost2" && y.DomminionsModTag != "#weapon" && y.DomminionsModTag != "#armor"));
            }
            AverageMainPathLevel = Items.Average(x => x.MainPathLevel);
            AverageCrossPathLevel = (double)Items.Average(x => x.CrossPathLevel);
        }
    }
}
