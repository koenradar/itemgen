using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace data
{
    /// <summary>
    /// Atributes are additions to items think fireres 5 TODO no more int? ? bad
    /// </summary>
    ///
    public class Atribute
    {
        public string Name { get; set; }
        public string DomminionsModTag { get; set; }

        [JsonIgnore]
        public MagicPath[] PathAffinity { get; set; }

        public string PathAffinityJson
        {
            get
            {
                string temp = "";
                if (PathAffinity == null) return temp;

                foreach (MagicPath path in PathAffinity)
                {
                    temp += path.ToString();
                    temp += " ";
                }
                return temp;
            }
            set
            {
                List<MagicPath> temp = new();
                foreach (string path in Enum.GetNames(typeof(MagicPath)))
                {
                    if (!value.Contains(path)) continue;

                    MagicPath x = (MagicPath)Enum.Parse(typeof(MagicPath), path);
                    temp.Add(x);
                }
                PathAffinity = temp.ToArray();
            }
        }

        [JsonIgnore]
        public AttributeAffinity Affinity { get; set; }

        [JsonIgnore]
        public AttributeType[] Types { get; set; }

        public string TypesJson
        {
            get
            {
                string temp = "";
                if (Types != null)
                {
                    foreach (AttributeType type in Types)
                    {
                        temp += type.ToString();
                        temp += ", ";
                    }
                }
                return temp;
            }
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
                Types = temp.ToArray();
            }
        }

        public string Scaling { get; set; }

        [JsonIgnore]
        public int Power { get; set; }

        public int DefaultPointCost { get; set; }

        public int MinimumConstLevel { get; set; }

        public int BasePower { get; set; }

        public int PowerScaling { get; set; }

        public int ScalingCost { get; set; }

        public int ScaledCost { get; set; }

        public int Weight { get; set; }
        public int MaxValue { get; set; }

        public string NameForItem { get; set; }

        /// <summary>
        /// Constructor for Atribute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domminionsModTag"></param>
        /// <param name="pathAffinity"></param>
        /// <param name="defaultPointCost"></param>
        /// <param name="minimumConstLevel"></param>
        /// <param name="basePower"></param>
        /// <param name="powerScaling"></param>
        /// <param name="scalingCost"></param>
        /// <param name="weight"></param>
        [JsonConstructor]
        public Atribute(string name, string domminionsModTag, MagicPath[] pathAffinity, AttributeType[] types, int defaultPointCost,
                        int minimumConstLevel, int power, int basePower, int powerScaling, int scalingCost, int weight)
        {
            Name = name;
            DomminionsModTag = domminionsModTag;
            PathAffinity = pathAffinity;
            DefaultPointCost = defaultPointCost;
            MinimumConstLevel = minimumConstLevel;
            BasePower = basePower;
            PowerScaling = powerScaling;
            ScalingCost = scalingCost;
            Weight = weight;
            Types = types;
            Power = power;

            switch (defaultPointCost)
            {
                case <= 0:
                    Affinity = AttributeAffinity.negative;
                    break;
                case >= 10:
                    Affinity = AttributeAffinity.standalone;
                    break;
                default:
                    Affinity = AttributeAffinity.filler;
                    break;
            }
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="atribute"></param>
        public Atribute(Atribute atribute)
        {
            Name = atribute.Name;
            DomminionsModTag = atribute.DomminionsModTag;
            PathAffinity = atribute.PathAffinity;
            DefaultPointCost = atribute.DefaultPointCost;
            MinimumConstLevel = atribute.MinimumConstLevel;
            BasePower = atribute.BasePower;
            PowerScaling = atribute.PowerScaling;
            ScalingCost = atribute.ScalingCost;
            Weight = atribute.Weight;
            Types = atribute.Types;
            Power = 0;
            MaxValue = atribute.MaxValue;

            ScaledCost += atribute.DefaultPointCost;

            if (DefaultPointCost <= 0)
            {
                Affinity = AttributeAffinity.negative;
            }
            else if (DefaultPointCost >= 10)
            {
                Affinity = AttributeAffinity.standalone;
            }
            else
            {
                Affinity = AttributeAffinity.filler;
            }
        }

        public static List<Atribute> AllAtributes { get; set; } = new();

        // Affinity
        public static List<Atribute> FillerAtributes { get; set; } = new();

        public static List<Atribute> StandaloneAtributes { get; set; } = new();
        public static List<Atribute> NegativeAtributes { get; set; } = new();

        //types
        public static List<List<Atribute>> AtributesTypeList { get; set; } = new();

        public static void UpdateAtributeLists(List<Atribute> tempList)
        {
            // check valid
            if (Controller.DebugLogging1)
            {
                foreach (Atribute item in tempList)
                {
                    if (item.DomminionsModTag == null)
                    {
                        Debug.WriteLine("no Tag : " + item.Name);
                    }
                }
            }

            AllAtributes = tempList.FindAll(att => att.DomminionsModTag != null && att.BasePower != 0);

            // Affinity
            FillerAtributes = AllAtributes.Where(att => att.Affinity == AttributeAffinity.filler).ToList();
            StandaloneAtributes = AllAtributes.Where(att => att.Affinity == AttributeAffinity.standalone).ToList();
            NegativeAtributes = AllAtributes.Where(att => att.Affinity == AttributeAffinity.negative).ToList();

            // types
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
                AtributesTypeList.Add(AllAtributes.Where(att => AtributeContains(type, att)).ToList());
            }
        }

        public static bool AtributeContains(AttributeType type, Atribute atribute)
        {
            if (atribute.Types == null)
            {
                return false;
            }

            if (atribute.Types.Contains(type))
            {
                return true;
            }

            return false;
        }

        public static Atribute GetAttribute()
        {
            return GetAtributeFrom(AllAtributes);
        }

        public static Atribute GetAttribute(AttributeType type)
        {
            return GetAtributeFrom(AtributesTypeList[(int)type]);
        }

        public static Atribute GetAttribute(AttributeAffinity affinity)
        {
            return affinity switch
            {
                AttributeAffinity.filler => GetAtributeFrom(FillerAtributes),
                AttributeAffinity.standalone => GetAtributeFrom(StandaloneAtributes),
                AttributeAffinity.negative => GetAtributeFrom(NegativeAtributes),
                _ => null,
            };
        }

        public static Atribute GetAtributeFrom(List<Atribute> list)
        {
            int r = Controller.Rng.Next(0, list.Select(x => x.Weight).Sum());
            int accumulatedWeight = 0;
            foreach (var atribute in list)
            {
                accumulatedWeight += atribute.Weight;
                if (r <= accumulatedWeight)
                {
                    Atribute passByValue = new(atribute);
                    return passByValue;
                }
            }
            return null;
        }

        // TODO make this call small function
        public static List<Atribute> HandleAtribute(Atribute atribute, List<Atribute> atributeListToCheck)
        {
            List<Atribute> localHandleList = new();

            if (atribute.DomminionsModTag == "#poisoncloud" && atributeListToCheck.All(x => x.DomminionsModTag != "#poisonres") && AllAtributes.Find(x => x.DomminionsModTag == "#poisonres") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#poisonres"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#diseasecloud" && atributeListToCheck.All(x => x.DomminionsModTag != "#diseaseres") && AllAtributes.Find(x => x.DomminionsModTag == "#diseaseres") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#diseaseres"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#uwheat" && atributeListToCheck.All(x => x.DomminionsModTag != "#fireres") && AllAtributes.Find(x => x.DomminionsModTag == "#fireres") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#fireres"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#mindslime" && atributeListToCheck.All(x => x.DomminionsModTag != "#voidsanity") && AllAtributes.Find(x => x.DomminionsModTag == "#voidsanity") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#voidsanity"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#overcharged" && atributeListToCheck.All(x => x.DomminionsModTag != "#shockres") && AllAtributes.Find(x => x.DomminionsModTag == "#shockres") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#shockres"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#cold" && atributeListToCheck.All(x => x.DomminionsModTag != "#coldres") && AllAtributes.Find(x => x.DomminionsModTag == "#coldres") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#coldres"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#heat" && atributeListToCheck.All(x => x.DomminionsModTag != "#fireres") && AllAtributes.Find(x => x.DomminionsModTag == "#fireres") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#fireres"));
                copiedAtribute.Power = atribute.Power;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#incorporate" && atributeListToCheck.All(x => x.DomminionsModTag != "#trample") && AllAtributes.Find(x => x.DomminionsModTag == "#trample") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trample"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
                copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trampswallow"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#digest" && atributeListToCheck.All(x => x.DomminionsModTag != "#trample") && AllAtributes.Find(x => x.DomminionsModTag == "#trample") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trample"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
                copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trampswallow"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#aciddigest" && atributeListToCheck.All(x => x.DomminionsModTag != "#trample") && AllAtributes.Find(x => x.DomminionsModTag == "#trample") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trample"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
                copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trampswallow"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#trampswallow" && atributeListToCheck.All(x => x.DomminionsModTag != "#trample") && AllAtributes.Find(x => x.DomminionsModTag == "#trample") != null)
            {
                Atribute copiedAtribute = new(AllAtributes.Find(x => x.DomminionsModTag == "#trample"));
                copiedAtribute.Power = 1;
                localHandleList.Add(copiedAtribute);
            }
            else if (atribute.DomminionsModTag == "#champprize")
            {
                Atribute autoCompete = new("AutoCompete", "#autocompete", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 8, 1, 1, 0, 0, 0);
                Atribute cursed = new(AllAtributes.Find(x => x.DomminionsModTag == "#cursed"));
                localHandleList.Add(autoCompete);
                localHandleList.Add(cursed);
            }
            else if (atribute.DomminionsModTag == "#autospellrepeat")
            {
                Atribute autoSpell = new(atribute.Name, "#autospell", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, 0, 0, 0, 0,0);
                localHandleList.Add(autoSpell);
            }

            return localHandleList;
        }

        private static readonly Dictionary<string, string[]> OneToManyExclusiveAtributes = new()
        {
            {
                "#feeblemind",
                new [] {"#mastersmith", "#masterrit", "#fixforgebonus", "#forgebonus" , "#randomspell", "#comslave", "#commaster",
                    "#bonusspells", "#spellsinger", "#pen", "#fastcast", "#combatcaster",
                    "#darkvision", "#spiritsight", "#tmpairgems", "#tmpastralgems", "#tmpdeathgems", "#tmpearthgems", "#tmpfiregems",
                    "#tmpnaturegems", "#tmpwatergems", "#crossbreeder" , "#researchbonus" , "#douse", "#slothresearch", "#drainimmune",
                    "#magicboost", "#mute", "#elegist", "#alchemy", "#adeptsacr", "#lamialord", "#corpselord", "#ivylord", "#dragonlord",
                    "#allrange", "#elementrange", "#sorceryrange", "#airrange", "#astralrange", "#bloodrange", "#deathrange", "#earthrange",
                    "#firerange", "#naturerange", "#waterrange",
                }
            },
            {
                "#bers",
                new [] { "#randomspell", "#comslave", "#commaster", "#bonusspells", "#spellsinger", "#pen", "#fastcast", "#combatcaster",
                    "#darkvision", "#spiritsight", "#tmpairgems", "#tmpastralgems", "#tmpdeathgems", "#tmpearthgems", "#tmpfiregems",
                    "#tmpnaturegems", "#tmpwatergems", "#magicboost", "#prec"}
            },
        };

        private static readonly List<List<string>> MutuallyExclusiveAtributes = new()
        {
            new() { "#commaster", "#comslave" },
            new() { "#springpower", "#fallpower" },
            new() { "#summerpower", "#winterpower" },
            new() { "#maxsize", "#minsize" },
            new() { "#onlyinanim", "#noinanim" },
            new() { "#onlyinanim", "#onlycoldblood" },
            new() { "#onlyimmobile", "#noimmobile" },
            new() { "#onlyimmobile", "#unteleportable" },
            new() { "#nodemon", "#onlydemon" },
            new() { "#deathpower" },
            new() { "#falsearmy" },
            new() { "#autospellrepeat", "#spell", "#autospell" },
            new() { "#reform" },
            new() { "#firepower", "#coldpower" },
            new() { "#invulnerable", "#barkskin", "#ironskin", "#stoneskin" },
            new() { "#seduce", "#beckon", "#succubus" }, // Needs Assasin when added
            new() { "#homesick", "#uwregen", "#regeneration", "#limitedregen", "#undregen", "#onlyundead" },
            new() { "#sunawe", "#animalawe", "#awe", "#haltheretic" },
            new() { "#bonusspells", "#comslave" },
            new() { "#bless", "#autobless", "#spreaddom" },
        };

        /// <summary>
        /// True equals there is a error -> false no error
        /// </summary>
        /// <param name="atributeName"></param>
        /// <param name="atributesToCheck"></param>
        /// <returns></returns>S
        public static bool CheckForAtributeConflicts(string atributeName, List<Atribute> atributesToCheck) // Very slow function FIX
        {
            //  Mutually exclusive -> dont add
            foreach (List<string> mutuallyExclusiveAtributes in MutuallyExclusiveAtributes)
            {
                if (mutuallyExclusiveAtributes.Any(x => x == atributeName))
                {
                    foreach (Atribute atribute in atributesToCheck)
                    {
                        if (mutuallyExclusiveAtributes.Any(x => x == atribute.DomminionsModTag))
                        {
                            return true;
                        }
                    }
                }
            }

            // Dict check string -> dont add
            foreach (Atribute atribute in atributesToCheck)
            {
                if (OneToManyExclusiveAtributes.Keys.Any(x => x == atributeName))
                {
                    if (OneToManyExclusiveAtributes[atributeName].Any(modTag => modTag == atribute.DomminionsModTag))
                    {
                        return true;
                    }
                }

                foreach (string check in OneToManyExclusiveAtributes.Keys)
                {
                    if (atributesToCheck.Any(att => att.DomminionsModTag == check))
                    {
                        if (OneToManyExclusiveAtributes[check].Any(x => x == atributeName))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool Scalable(int points)
        {
            if (points < DefaultPointCost + ScalingCost)
            {
                return false;
            } 
            else
            {
                return true;
            }
        }
    }

    public enum AttributeAffinity
    {
        filler,
        standalone,
        negative
    }

    public enum AttributeType
    {
        Thugging,
        Counterthug,
        Magic,
        Stealth,
        Commander,
        Other
    }
}