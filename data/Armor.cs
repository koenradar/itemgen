using System.Collections.Generic;

namespace data
{
    public class Armor : IEquipment
    {
        public static int Id { get; set; } = 300;
        public int ThisId { get; set; } = Id;
        public string Name { get; set; } = "name not set @koenradar";
        public int Protection { get; private set; }
        public int DefenceBonus { get; private set; }
        public int Encumbrance { get; private set; }
        public Type ThisType { get; private set; }

        public Armor() // TODO change to weapon style
        {
            ThisId = Id;
            Id++;
        }

        public static Armor GenerateArmor(int points, Type type, out int cost)
        {
            Armor thisArmor = new();
            cost = 0;
            switch (type)
            {
                case Type.shield:
                    thisArmor.ThisType = Type.shield;
                    thisArmor.Protection = 14;
                    thisArmor.DefenceBonus = 4;
                    thisArmor.Encumbrance = 2;
                    cost = 3;
                    break;

                case Type.helmet:
                    thisArmor.ThisType = Type.helmet;
                    thisArmor.Protection = 14;
                    cost = 4;
                    break;

                case Type.crown:
                    thisArmor.ThisType = Type.crown;
                    thisArmor.Protection = 9;
                    cost = 2;
                    break;

                case Type.armor:
                    thisArmor.ThisType = Type.armor;
                    thisArmor.Protection = 13;
                    thisArmor.Encumbrance = 2;
                    thisArmor.DefenceBonus = -1;
                    cost = 3;
                    break;
            }

            if (type is Type.shield)
            {
                UpgradeShield(points, ref cost, thisArmor);
            }
            else if (type is Type.armor or Type.helmet)
            {
                UpgradeArmor(points, ref cost, thisArmor);
            }

            return thisArmor;
        }

        public static void UpgradeArmor(int points, ref int cost, Armor thisArmor)
        {
            double loopMultiplier = 1;
            while (cost < points)
            {
                // upgrade armor
                if (Controller.BoolFromRng(30) && cost + 6 < points)
                {
                    thisArmor.DefenceBonus++;
                    cost += (int) (6 * loopMultiplier);
                }
                else if (Controller.BoolFromRng(15) && thisArmor.Encumbrance > 0 && cost + 5 < points)
                {
                    thisArmor.Encumbrance -= 1;
                    cost += (int) (5 * loopMultiplier);
                }
                else
                {
                    int ChangingAmount = 1;

                    // check if armor cap
                    if (thisArmor.Protection + ChangingAmount >= 40)
                    {
                        thisArmor.Protection = 40;
                        cost += (int) (loopMultiplier * ChangingAmount * 3);
                        break;
                    }

                    thisArmor.Protection += ChangingAmount;
                    cost += (int) (loopMultiplier * ChangingAmount);
                }

                loopMultiplier += 0.06;
            }
        }

        public static void UpgradeShield(int points, ref int cost, Armor thisArmor)
        {
            double loopMultiplier = 1;
            while (cost < points)
            {
                if (Controller.BoolFromRng(35) && cost + 6 < points)
                {
                    thisArmor.DefenceBonus++;
                    cost += (int) (6 * loopMultiplier);
                }
                else if (Controller.BoolFromRng(10) && thisArmor.Encumbrance > 0 && cost + 4 < points)
                {
                    thisArmor.Encumbrance -= 1;
                    cost += (int) (4 * loopMultiplier);
                }
                else
                {
                    int ChangingAmount = 1;
                    if (thisArmor.Protection + ChangingAmount >= 40)
                    {
                        thisArmor.Protection = 40;
                        cost += (int) (loopMultiplier * ChangingAmount * 3);
                        break;
                    }

                    thisArmor.Protection += ChangingAmount;
                    cost += (int) (loopMultiplier * ChangingAmount);
                }

                loopMultiplier += 0.06;
            }
        }

        public List<string> ToStringList()
        {
            List<string> stringList = new()
            {
                $"#newarmor {ThisId}",
                $"#name \"{Name}\" ",
                $"#type {(int)ThisType}",
                $"#prot {Protection}",
                "#magicarmor",
            };

            if (DefenceBonus != 0)
            {
                stringList.Add($"#def {DefenceBonus}");
            }

            if (Encumbrance != 0)
            {
                stringList.Add($"#enc {Encumbrance}");
            }

            stringList.Add("#end");
            return stringList;
        }

        
    }
}