using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static data.Weapon;

namespace data
{
    public class SecondaryEffect
    {
        public string Name { get; set; }

        public int Aoe { get; set; }

        public long Damage { get; set; }

        public int Cost { get; set; }
        public int DamageScalingCost { get; set; }
        
        private List<DamageType> DamageType;

        public string DamageTypes
        {
            get => string.Join(",", DamageType);
            set {
                List<DamageType> temp = new();
                foreach (string type in Enum.GetNames(typeof(DamageType)))
                {
                    if (value.Contains(type))
                    {
                        DamageType x = (DamageType)Enum.Parse(typeof(DamageType), type);
                        temp.Add(x);
                    }
                }
                DamageType = temp;
            }
        }

        public List<AttributeType> ThisType { get; set; }

        public string Buckets
        {
            get => string.Join(",", ThisType);
            set
            {
                List<AttributeType> temp = new();
                foreach (string type in Enum.GetNames(typeof(AttributeType)))
                {
                    if (value.Contains(type))
                    {
                        AttributeType x = (AttributeType)Enum.Parse(typeof(AttributeType), type);
                        temp.Add(x);
                    }
                }
                ThisType = temp;
            }
        }

        public bool Always;

        public string SecondaryEffectAlways
        {
            get => Always.ToString();
            set {
                value = value.ToLower();
                if (value.Contains("yes") || value.Contains("true"))
                {
                    Always = true;
                } else
                {
                    Always = false;
                }
            }
        }

        public int Weight { get; set; }

        public bool Tertiary;

        public string TertiaryEffect
        {
            get => Tertiary.ToString();
            set
            {
                value = value.ToLower();
                if (value.Contains("yes") || value.Contains("true"))
                {
                    Tertiary = true;
                }
                else
                {
                    Tertiary = false;
                }
            }
        }

        public int TertiaryEffectCost { get; set; }

        public MagicPath[] PathAffinityies;

        public string PathAffinity
        {
            get => string.Join(",", PathAffinityies);
            set
            {
                List<MagicPath> temp = new();
                foreach (string path in Enum.GetNames(typeof(MagicPath)))
                {
                    if (value.Contains(path))
                    {
                        MagicPath x = (MagicPath)Enum.Parse(typeof(MagicPath), path);
                        temp.Add(x);
                    }
                }
                PathAffinityies = temp.ToArray();
            }
        }

        public SecondaryEffect ThisTetiaryEffect { get; set; }

        public int ExplosionSprite { get; set; }

        public string Additional { get; set; }

        public int CopyWeapon { get; set; }

        public int ThisId { get; set; } = 0;

        public List<string> ToStringList()
        {
            List<string> stringList = new()
            {
                $"#newweapon {ThisId}",
                $"#name \"{Name}\"",
                $"#dmg {Damage}",
                "#magic"
            };

            if (ThisTetiaryEffect != null && ThisTetiaryEffect.Always)
            {
                stringList.Add($"#secondaryeffectalways {ThisTetiaryEffect.ThisId}"); // TODO
            }
            else if (ThisTetiaryEffect != null)
            {
                stringList.Add($"#secondaryeffect {ThisTetiaryEffect.ThisId}");
            }

            if (CopyWeapon != 0)
            {
                stringList.Insert(1, $"#copyweapon {CopyWeapon}");
            }

            if (ExplosionSprite != 0)
            {
                stringList.Add($"#explspr {ExplosionSprite}");
            }

            if (Aoe != 0)
            {
                stringList.Add($"#aoe {Aoe}");
                stringList.Add("#sound 89");
            }

            DamageTypesToInputedList(stringList, DamageType);

            stringList.Add($"#end");

            return stringList;
        }

        public SecondaryEffect()
        {
        }

        public SecondaryEffect(SecondaryEffect copy)
        {
            ThisId = Id;
            Id++;
            Name = copy.Name;
            Aoe = copy.Aoe;
            Damage = copy.Damage;
            Cost = copy.Cost;
            DamageType = copy.DamageType;
            Always = copy.Always;
            Weight = copy.Weight;
            Tertiary = copy.Tertiary;
            PathAffinityies = copy.PathAffinityies;
            ExplosionSprite = copy.ExplosionSprite;
            Additional = copy.Additional;
            CopyWeapon = copy.CopyWeapon;
        }

    }
}
