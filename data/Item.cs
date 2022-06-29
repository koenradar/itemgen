using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace data
{
    //TODO
    // 112 x 2 = 224 National items (2 per nation (0, 1, 2, 3 should be selectable)) these should be more powerful than expected for their construction level (toggleable?)
    // no artifacts mode needs to be included (toggle)
    // Blending sprites

    // 499 - 224 = 275 Non national items

    public class Item
    {
        public static int ItemCurrentId { get; set; } = 1;
        private int ThisId { get; set; }

        private IEquipment Equipment;
        public Type ItemType { get; set; }

        private AttributeType ThisArcheType { get; set; }

        public int ConstLevel { get; set; }

        public MagicPath MainPath { get; set; }

        public int MainPathLevel { get; set; }

        public MagicPath CrossPath { get; set; }

        public int? CrossPathLevel { get; set; }

        private string Name { get; set; } //?

        public string Description { get; set; }
        public string SpriteOld { get; set; }
        private Sprite Sprite { get; set; }
        public static int DefaultPoints = 10;
        public static double PointMultiplier = 0.7;
        private List<string> DebugInfo { get; set; }

        public List<Atribute> AtributeList { get; private set; }

        /// <summary>
        /// Constructor for a item has AutoIncrement ID
        /// </summary>
        /// <param name="itemtype"></param>
        /// <param name="constlevel"></param>
        /// <param name="mainpath"></param>
        /// <param name="mainpathlevel"></param>
        /// <param name="secondpath"></param>
        /// <param name="secondpathlevel"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="atributeList"></param>
        public Item(Type itemtype, AttributeType archeType, int constlevel, MagicPath mainpath, int mainpathlevel, MagicPath secondpath,
                    int? secondpathlevel, string name, string description, List<Atribute> atributeList, List<string> debugInfo)
        {
            while (Controller.NessesaryVanillaItems.Contains(ItemCurrentId))
            {
                ItemCurrentId += 1;
            }

            ThisId = ItemCurrentId;
            ItemCurrentId++;
            ItemType = itemtype;
            ConstLevel = constlevel;
            MainPath = mainpath;
            MainPathLevel = mainpathlevel;
            CrossPath = secondpath;
            CrossPathLevel = secondpathlevel;
            Name = name;
            Description = description;
            AtributeList = atributeList;
            ThisArcheType = archeType;
            DebugInfo = debugInfo;
        }

        public static int EvaluateAtributes(List<Atribute> atributeListLocal)
        {
            int value = 0;

            foreach (Atribute a in atributeListLocal) // Manage scaled scaling cost
            {
                value += a.ScaledCost;
            }

            return value;
        }

        /// <summary>
        /// If possible upgrade the power of a attribute of the item
        /// </summary>
        /// <param name="points"></param>
        private static void IncreaseRandomAttributes(int points, List<Atribute> atributeList)
        {
            while (atributeList.Any(att => att.PowerScaling > 0 && att.ScalingCost > 0 && EvaluateAtributes(atributeList) + att.ScalingCost <= points && att.Power + att.PowerScaling <= att.MaxValue))
            {
                Atribute att = atributeList.Find(att => att.PowerScaling > 0 && att.ScalingCost > 0 && EvaluateAtributes(atributeList) + att.ScalingCost <= points && att.Power + att.PowerScaling <= att.MaxValue);
                att.Power += att.PowerScaling;

                att.ScaledCost += att.ScalingCost;
            }
        }

        /// <summary>
        /// Creates a DM version of the item
        /// </summary>
        /// <returns>List to be writen to the file</returns>
        public List<string> ToStringList()
        {
            int ItemTypeToPrint = (int)ItemType;
            if (ItemTypeToPrint == 3)
            {
                Weapon weapon = Equipment as Weapon;
                if (weapon.ThisWeaponType is Weapon.WeaponType.Ranged_Sling)
                {
                    ItemTypeToPrint = 1;
                }
                else
                {
                    ItemTypeToPrint = 2;
                }
            }

            List<string> stringList;
            if (AtributeList.Any(att => att.DomminionsModTag == "#copyitem"))
            {
                stringList = new()
                {
                    $"#selectitem {ThisId}",
                    $"#name \"{Name}\"",
                    $"#descr \"{Description}\"",
                    $"#constlevel {(AtributeList.Any(x => x.DomminionsModTag == "#champprize") ? 12 : ConstLevel)}",
                    $"#type {ItemTypeToPrint}",
                    $"#mainpath {(int)MainPath}",
                    $"#mainlevel {MainPathLevel}"
                };
            }
            else
            {
                stringList = new()
                {
                    $"#selectitem {ThisId}",
                    "#clear",
                    $"#name \"{Name}\"",
                    $"#descr \"{Description}\"",
                    $"#constlevel {(AtributeList.Any(x => x.DomminionsModTag == "#champprize") ? 12 : ConstLevel)}",
                    $"#type {ItemTypeToPrint}",
                    $"#mainpath {(int)MainPath}",
                    $"#mainlevel {MainPathLevel}"
                };
            }
            

            if (CrossPath != MagicPath.empty && (CrossPathLevel != 0 && CrossPathLevel != null))
            {
                stringList.Add($"#secondarypath {(int)CrossPath}");
                stringList.Add($"#secondarylevel {CrossPathLevel}");
            }
            else
            {
                stringList.Add("#secondarypath -1");
                stringList.Add("#secondarylevel -1");
            }

            if (Sprite.AllSprites.Count(x => x.SpriteType == ItemType) != 0 && Sprite != null && Sprite.FilePath != null && Sprite != new Sprite())
            {
                stringList.Add($"#spr \"ItemGen_{Controller.GenNumber}/{Sprite.FilePath}\"");
            }
            else
            {
                stringList.Add($"#spr \"ItemGen_{Controller.GenNumber}/Sprites/{SpriteOld}\"");
            }

            if (AtributeList != null)
            {
                foreach (Atribute atribute in AtributeList) // TODO
                {
                    if (Controller.DebugLogging1)
                    {
                        Debug.WriteLine(Name + " : " + atribute.Name + " -> " + atribute.Power);
                    }

                    if (atribute.Power is < 1 or > 1 || atribute.PowerScaling != 0 && atribute.ScalingCost != 0)
                    {

                        if (atribute.DomminionsModTag == "#magicboost")
                        {
                            // Handle boosters
                            if (atribute.Power == 1)
                            {
                                stringList.Add($"{atribute.DomminionsModTag} {(int)MainPath} 1");
                            }
                            else if (Controller.BoolFromRng(70) && CrossPath != MagicPath.empty)
                            {
                                stringList.Add($"{atribute.DomminionsModTag} {(int)MainPath} {atribute.Power - 1}");
                                stringList.Add($"{atribute.DomminionsModTag} {(int)CrossPath} 1");
                            }
                            else
                            {
                                stringList.Add($"{atribute.DomminionsModTag} {(int)MainPath} {atribute.Power}");
                            }
                        }
                        else if (atribute.DomminionsModTag == "#copyitem")
                        {
                            stringList.Insert(1, $"{atribute.DomminionsModTag} {atribute.Power}");
                        }
                        else if (atribute.DomminionsModTag == "#spell" || atribute.DomminionsModTag == "#autospell")
                        {
                            stringList.Add($"{atribute.DomminionsModTag} \"{atribute.Name}\"");
                        }
                        else
                        {
                            stringList.Add($"{atribute.DomminionsModTag} {atribute.Power}");
                        }
                    }
                    else
                    {
                        stringList.Add($"{atribute.DomminionsModTag}");
                    }
                }
            }

            stringList.Add($"#end");
            stringList.Add($"");
            stringList.AddRange(DebugInfo);

            return stringList;
        }

        /// <summary>
        /// Generate a single Item TODO -> a lot of this shit in seperate functions
        /// </summary>
        /// <returns></returns>
        public static Item GenerateItem()
        {
            MagicPath mainMagicPath = (MagicPath)Controller.Rng.Next(0, 8);
            Type type = GetItemType();
            AttributeType archeType = GetArcheType(type);
            int constLevel = Controller.GetConstLevel(); // and set forge level multiplier before next line
            mainMagicPath = Controller.GetMainPathAndCrossPathLevelsAndOrder(mainMagicPath, out int mainPathLevel, out MagicPath crossMagicPath, out int? crossPathLevel);

            int constMultiplier = 1 + (constLevel / 2);
            if (constLevel == 8)
                constMultiplier += 1;

            int points = DefaultPoints; 
            if (crossPathLevel != null)
            {
                points += Controller.PathLevelToGemCost(mainPathLevel) * constMultiplier + Controller.PathLevelToGemCost((int)crossPathLevel) * constMultiplier;
            }
            else
            {
                points += Controller.PathLevelToGemCost(mainPathLevel) * constMultiplier;
            }

            points = (int)(points * PointMultiplier);

            int pointsBeforeChange = points;

            List<Atribute> atributeListLocal = new();

            // todo functions and garbage
            if (Controller.BoolFromRng(35) && mainMagicPath is not MagicPath.blood && constLevel > 2)
            {
                // decrease
                if (Controller.BoolFromRng(50))
                {
                    points = (int)(points * 0.7);
                    Atribute att = new("itemcost", "#itemcost1", null, null, 0, 0, -20, 0, 0, 0, 0);
                    atributeListLocal.Add(att);

                    if (crossMagicPath is not (MagicPath.empty or MagicPath.blood))
                    {
                        points = (int)(points * 0.7);
                        att = new("itemcost", "#itemcost2", null, null, 0, 0, -20, 0, 0, 0, 0);
                        atributeListLocal.Add(att);
                    }
                }
                else
                {
                    points = (int)(points * 0.5);
                    Atribute att = new("itemcost", "#itemcost1", null, null, 0, 0, -40, 0, 0, 0, 0);
                    atributeListLocal.Add(att);

                    if (crossMagicPath is not (MagicPath.empty or MagicPath.blood))
                    {
                        points = (int)(points * 0.5);
                        att = new("itemcost", "#itemcost2", null, null, 0, 0, -40, 0, 0, 0, 0);
                        atributeListLocal.Add(att);
                    }
                }
            }
            int pointsAfterChange = points;

            if (mainMagicPath is MagicPath.blood)
            {
                // increase cost 2x
                Atribute att = new("itemcost", "#itemcost1", null, null, 0, 0, 100, 0, 0, 0, 0);
                atributeListLocal.Add(att);
            }
            else if (crossMagicPath is MagicPath.blood)
            {
                // increase cost 2x
                Atribute att = new("itemcost", "#itemcost2", null, null, 0, 0, 100, 0, 0, 0, 0);
                atributeListLocal.Add(att);
            }

            MagicPath[] thisMagicPaths = new MagicPath[2];
            thisMagicPaths[0] = mainMagicPath;
            thisMagicPaths[1] = crossMagicPath;
                 
            IEquipment equipment = UsePointsForEquipment(type, archeType, constLevel, thisMagicPaths, mainPathLevel, out var costForPrint, ref points);
            EnsureBoostersPerPath(mainMagicPath, crossMagicPath, archeType, points, atributeListLocal);
            UsePointsForAttributes(archeType, mainMagicPath, crossMagicPath, atributeListLocal, constLevel, points);

            // Posibility of points remaining at this point and not being correctly fixxed in the increase random attribute on item below NEEDS fix
            IncreaseRandomAttributes(points, atributeListLocal);

            List<string> debugInfo = new();

            int randomId = Controller.Rng.Next(1, 1000000);
            if (Controller.DebugLogging1)
            {
                debugInfo.Add("-- Debug");
                debugInfo.Add($"-- Breakdown of {type} of {mainMagicPath} {mainPathLevel} {crossMagicPath} {crossPathLevel} {randomId}");
                debugInfo.Add($"-- The intended archeType of this item is {archeType}: is this correct?");
                debugInfo.Add("-- Cost of weapon: " + costForPrint);
                debugInfo.Add("-- Cost of Ability: " + EvaluateAtributes(atributeListLocal));
                foreach (Atribute atribute in atributeListLocal)
                {
                    if (atribute.PowerScaling != 0 && atribute.ScalingCost != 0)
                        debugInfo.Add("-- - --" + atribute.Name + ": " + atribute.Power + "-> Total cost -> " + atribute.ScaledCost);
                    else
                        debugInfo.Add("-- - --" + atribute.Name + ": " + atribute.Power + "-> Total cost -> " + atribute.DefaultPointCost);
                }
                debugInfo.Add("-- No negative total points for item: " + pointsBeforeChange);
                if (pointsBeforeChange != pointsAfterChange)
                    debugInfo.Add("-- Corrected points: " + pointsAfterChange);
                debugInfo.Add("-- used points: " + (costForPrint + EvaluateAtributes(atributeListLocal)));
                if (pointsAfterChange > costForPrint + EvaluateAtributes(atributeListLocal))
                    debugInfo.Add("-- !! Not all points used !!");
                debugInfo.Add("-- Debug end");
            }

            string nameForEquipment = "";
            if (equipment is Weapon thisWeapon)
            {
                Atribute att = new("thisWeapon", "#weapon", null, null, 0, 0, thisWeapon.ThisId, 0, 0, 0, 0);
                atributeListLocal.Add(att);
                nameForEquipment = thisWeapon.ThisWeaponType.ToString();
            }
            else if (equipment is Armor thisArmor)
            {
                Atribute att = new("thisArmor", "#armor", null, null, 0, 0, thisArmor.ThisId, 0, 0, 0, 0);
                atributeListLocal.Add(att);
            }
            string name;
            if (nameForEquipment.Length > 0)
            {
                name = $"{archeType} {nameForEquipment} of {mainMagicPath} {mainPathLevel} {crossMagicPath} {crossPathLevel} {randomId}";
            }
            else
            {
                name = $"{archeType} {type} of {mainMagicPath} {mainPathLevel} {crossMagicPath} {crossPathLevel} {randomId}";
            }

            string generatedName;

            if (equipment is Weapon thisWeaponForName)
            {
                generatedName = GenerateName(atributeListLocal, ItemCurrentId, type, thisWeaponForName.ThisWeaponType);
                thisWeaponForName.Name = generatedName;
            }
            else if (equipment is Armor thisArmor)
            {
                generatedName = GenerateName(atributeListLocal, ItemCurrentId, type);
                equipment.Name = generatedName;
            }
            else
            {
                generatedName = GenerateName(atributeListLocal, ItemCurrentId ,type);
            }
            
            if (!string.IsNullOrEmpty(generatedName))
            {
                name = generatedName;
            }

            string description = "";
            foreach (Atribute atribute in atributeListLocal)
            {
                description += "(" + atribute.Name + " " + atribute.Power + " ) ";
            }

            Item item = new(type, archeType, constLevel, mainMagicPath, mainPathLevel, crossMagicPath, crossPathLevel,
               name, description, atributeListLocal, debugInfo);

            item.Equipment = equipment;
            item.Sprite = GetGeneralSprite2(type, atributeListLocal);
            item.SpriteOld = GetGeneralSprite(type, atributeListLocal);
            
            return item;
        }

        private static Sprite GetGeneralSprite2(Type type, List<Atribute> atributeListLocal)
        {
            if (atributeListLocal.Any(x => x.DomminionsModTag == "#magicboost"))
            {
                return new Sprite();
            }
            else
            {
                int leastAmountUsed = 0;
                while (true)
                {
                    if (Sprite.AllSprites.Count(x => x.SpriteType == type && x.UsedAmount <= leastAmountUsed) > 0)
                    {
                        var Sprite = data.Sprite.AllSprites.First(x => x.SpriteType == type && x.UsedAmount <= leastAmountUsed);
                        Sprite.UsedAmount++;
                        return Sprite;
                    }
                    else
                    {
                        leastAmountUsed++;
                    }
                }
            }
        }

        private static void EnsureBoostersPerPath(MagicPath itemPath, MagicPath crossPath, AttributeType itemType, int points, List<Atribute> itemAtributes)
        {
            if(crossPath != MagicPath.empty || itemType == AttributeType.Stealth) return;
            if (Controller.ItemList.Count(x => x.MainPath == itemPath && x.CrossPath == MagicPath.empty && x.ConstLevel is > 0 and < 8 && x.AtributeList.Any(att => att.DomminionsModTag == "#magicboost")) > 2) return;
            if (Atribute.AllAtributes.Find(x => x.DomminionsModTag == "#magicboost") == null) return;
            if (Atribute.AllAtributes.Find(x => x.DomminionsModTag == "#magicboost").DefaultPointCost > points) return;


            Atribute att = new(Atribute.AllAtributes.First(x => x.DomminionsModTag == "#magicboost"))
            {
                Power = 1
            };
            itemAtributes.Add(att);
        }

        private static string GetGeneralSprite(Type type, List<Atribute> atributes)
        {
            string sprite = "";

           
            if (atributes.Any(x => x.DomminionsModTag == "#magicboost"))
            {
                    return Enum.GetName(type) + "_booster.tga";
            }

            return Enum.GetName(type) + ".tga";
        }

        private static IEquipment UsePointsForEquipment(Type type, AttributeType archeType, int constLevel, MagicPath[] weaponMagicPaths, int mainPathLevel, out int costForPrint, ref int points)
        {
            // if weapon or armor assign points to weapon or armor or shield
            costForPrint = 0;
            if (type is Type.one_handed or Type.two_handed or Type.missle_weapon)
            {
                double procentToEquipment = 0.1;

                if (archeType is AttributeType.Thugging or AttributeType.Counterthug)
                {
                    procentToEquipment = 0.9;
                }



                int pointsForWeapon = (int)(points * procentToEquipment); // Needs better algoritm
                Weapon thisWeapon = Weapon.GenerateWeapon(pointsForWeapon, type, constLevel, archeType, weaponMagicPaths, out int cost);
                points -= cost;
                costForPrint = cost;
                Controller.WeaponList.Add(thisWeapon);
                return thisWeapon;
            }

            if (type is not (Type.armor or Type.shield or Type.helmet or Type.crown)) return null;
            {
                double procentToItem = Controller.Rng.NextDouble();

                // everyone like good armor but matters less on other
                if (archeType is AttributeType.Other)
                {
                    procentToItem -= 0.2;
                }
                else if (archeType is AttributeType.Counterthug or AttributeType.Thugging)
                {
                    procentToItem += 0.2;
                }

                if (procentToItem < 0.3)
                {
                    procentToItem = 0.3;
                }
                else if (procentToItem > 0.8)
                {
                    procentToItem = 0.8;
                }

                int pointsForArmor = (int)(points * procentToItem); // Needs better algoritm
                Armor thisArmor = Armor.GenerateArmor(pointsForArmor, type, out int cost);
                points -= cost;
                costForPrint = cost;
                Controller.ArmorList.Add(thisArmor);
                return thisArmor;
            }
        }

        private static void UsePointsForAttributes(AttributeType archeType, MagicPath mainMagicPath, MagicPath crossMagicPath,
                                                   List<Atribute> atributeListLocal, int constLevel, int points)
        {
            List<Atribute> archetypeAtributes = Atribute.AllAtributes.Where(x => x.Types.Contains(archeType) && x.MinimumConstLevel <= constLevel).ToList();
            List<Atribute> mainPathArchetypeAtributes = archetypeAtributes.Intersect(Atribute.AllAtributes.Where(x => x.PathAffinity.Contains(mainMagicPath))).ToList();
            List<Atribute> crossPathArchetypeAtributes = archetypeAtributes.Intersect(Atribute.AllAtributes.Where(x => x.PathAffinity.Contains(crossMagicPath))).ToList();
            List<Atribute> bothPathArchetypeAtributes = mainPathArchetypeAtributes.Intersect(crossPathArchetypeAtributes).ToList();
            List<Atribute> allAtributesLocal = Atribute.AllAtributes.ToList();

            if (ItemCurrentId < 499)
            {
                archetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#copyitem");
                mainPathArchetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#copyitem");
                crossPathArchetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#copyitem");
                bothPathArchetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#copyitem");
                allAtributesLocal.RemoveAll(x => x.DomminionsModTag == "#copyitem");
            }

            if (points < 100)
            {
                archetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#champprize");
                mainPathArchetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#champprize");
                crossPathArchetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#champprize");
                bothPathArchetypeAtributes.RemoveAll(x => x.DomminionsModTag == "#champprize");
                allAtributesLocal.RemoveAll(x => x.DomminionsModTag == "#champprize");
            }

            while (EvaluateAtributes(atributeListLocal) < points && atributeListLocal.Count < 16)
            {
                // generate
                Atribute localAtribute = null;

                if (constLevel <= 6)
                {
                    List<Atribute> list = bothPathArchetypeAtributes.Where(x => x.ScalingCost != 0 && (x.DefaultPointCost == points || (points - x.DefaultPointCost) % x.ScalingCost == 0) ).ToList();
                    list = list.Union(mainPathArchetypeAtributes.Where(x => x.ScalingCost != 0 && (x.DefaultPointCost == points || (points - x.DefaultPointCost) % x.ScalingCost == 0))).ToList();
                    list = list.Union(crossPathArchetypeAtributes.Where(x => x.ScalingCost != 0 && (x.DefaultPointCost == points || (points - x.DefaultPointCost) % x.ScalingCost == 0))).ToList();
                    
                    if (list.Count > 0)
                    {
                        localAtribute = Atribute.GetAtributeFrom(list.ToList());
                    }
                }

                if (localAtribute == null)
                {
                    if (bothPathArchetypeAtributes.Count > 0 && Controller.BoolFromRng(80))
                    {
                        localAtribute = Atribute.GetAtributeFrom(bothPathArchetypeAtributes);
                    }
                    else if (mainPathArchetypeAtributes.Count > 0 && Controller.BoolFromRng(70))
                    {
                        localAtribute = Atribute.GetAtributeFrom(mainPathArchetypeAtributes);
                    }
                    else if (crossPathArchetypeAtributes.Count > 0 && Controller.BoolFromRng(70))
                    {
                        localAtribute = Atribute.GetAtributeFrom(crossPathArchetypeAtributes);
                    }
                    else if (archetypeAtributes.Count > 0)
                    {
                        localAtribute = Atribute.GetAtributeFrom(archetypeAtributes);
                    }
                    else
                    {
                        List<Atribute> possibleAtributes = allAtributesLocal
                            .Where(x => x.MinimumConstLevel <= constLevel && x.ScaledCost <= points).ToList();
                        if (possibleAtributes.Count > 0)
                        {
                            int choice = Controller.Rng.Next(0, possibleAtributes.Count);
                            localAtribute = possibleAtributes[choice];
                        }
                        else
                        {
                            Debug.WriteLine($"No attributes possible on this item: {ItemCurrentId}, " + points + " remaining");
                            break;
                        }

                    }
                }


                if (atributeListLocal.Any(x => x.Name == localAtribute.Name) || Atribute.CheckForAtributeConflicts(localAtribute.DomminionsModTag, atributeListLocal))
                {
                    archetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    mainPathArchetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    crossPathArchetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    bothPathArchetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    allAtributesLocal.RemoveAll(x => x.Name == localAtribute.Name);
                    continue;
                }

                if (EvaluateAtributes(atributeListLocal) > points - localAtribute.DefaultPointCost)
                {
                    archetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    mainPathArchetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    crossPathArchetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    bothPathArchetypeAtributes.RemoveAll(x => x.Name == localAtribute.Name);
                    allAtributesLocal.RemoveAll(x => x.Name == localAtribute.Name);
                    continue;
                } // can still gain points but we then get infinite loops TODO FIX

                int maxNegativeAttributes;

                if (constLevel < 2){ maxNegativeAttributes = 0;} else if (constLevel < 4) { maxNegativeAttributes = 1; } else if (constLevel < 6 ) { maxNegativeAttributes = 2; } else { maxNegativeAttributes = 3; }

                if (localAtribute == null || atributeListLocal.Count(x => x.Affinity == AttributeAffinity.negative) >= maxNegativeAttributes && localAtribute.Affinity == AttributeAffinity.negative)
                {
                    archetypeAtributes.RemoveAll(x => x.Affinity == AttributeAffinity.negative);
                    mainPathArchetypeAtributes.RemoveAll(x => x.Affinity == AttributeAffinity.negative);
                    crossPathArchetypeAtributes.RemoveAll(x => x.Affinity == AttributeAffinity.negative);
                    bothPathArchetypeAtributes.RemoveAll(x => x.Affinity == AttributeAffinity.negative);
                    allAtributesLocal.RemoveAll(x => x.Name == localAtribute.Name);
                    continue;
                }

                // powerScaling
                localAtribute.Power = localAtribute.BasePower;

                int scalingDoublingHelper = 0;

                while (localAtribute.PowerScaling != 0 && localAtribute.ScalingCost != 0
                       && EvaluateAtributes(atributeListLocal) + localAtribute.ScaledCost + localAtribute.ScalingCost <= points
                       && localAtribute.Power + localAtribute.PowerScaling <= localAtribute.MaxValue)
                {
                    int chanceForScalingPostive = 90;
                    int chanceForScalingNegative = 60;
                    if (constLevel <= 4)
                    {
                        chanceForScalingPostive += 10; // 100% chance to stack as much as posible in one attribute
                        chanceForScalingNegative -= 20; // less negative scaling on lower levels
                    }
                    if (localAtribute.ScalingCost > 0 && Controller.BoolFromRng(chanceForScalingPostive))
                    {
                        localAtribute.Power += localAtribute.PowerScaling;
                        localAtribute.ScaledCost += localAtribute.ScalingCost;
                        scalingDoublingHelper += localAtribute.ScalingCost;

                        if (scalingDoublingHelper >= Controller.ScalingDoublingIncrement && localAtribute.Scaling != "Linear")
                        {
                            localAtribute.ScalingCost += localAtribute.ScalingCost;
                            scalingDoublingHelper -= Controller.ScalingDoublingIncrement;
                        }
                    }
                    else if (localAtribute.ScalingCost < 0 && Controller.BoolFromRng(chanceForScalingNegative))
                    {
                        localAtribute.Power += localAtribute.PowerScaling;
                        points += localAtribute.ScalingCost;
                        localAtribute.ScaledCost += localAtribute.ScalingCost;
                    }
                    else break;
                }

                List<Atribute> extraAtributesList = new();
                if (Atribute.HandleAtribute(localAtribute, atributeListLocal).Count != 0)
                {
                    extraAtributesList.AddRange(Atribute.HandleAtribute(localAtribute, atributeListLocal));
                    if (extraAtributesList.Count + atributeListLocal.Count < 16)
                    {
                        atributeListLocal.AddRange(extraAtributesList);
                    }
                    else
                    {
                        atributeListLocal.Add(localAtribute);
                        break;
                    }
                }

                atributeListLocal.Add(localAtribute);
            }
        }

        private static string GenerateName(List<Atribute> itemAtributes, int id , Type itemType, Weapon.WeaponType weaponType = Weapon.WeaponType.One_Handed_Axe)
        {
            string itemName = "";
            List<Atribute> atributesWithSetNames = itemAtributes.Where(x => !string.IsNullOrEmpty(x.NameForItem) && x.DomminionsModTag is not "#weapon" or "#armor").ToList();
            if (atributesWithSetNames.Count > 0)
            {
                Atribute atributeForName = atributesWithSetNames.Find(x => x.ScaledCost == atributesWithSetNames.Max(y => y.ScaledCost));
                itemName += atributeForName.NameForItem;
            }
            else if(itemAtributes.Count > 0)
            {
                List<Atribute> atributesWithDefaultNames = itemAtributes.Where(x => !string.IsNullOrEmpty(x.Name) && x.DomminionsModTag is not "#weapon" or "#armor").ToList();
                if (atributesWithDefaultNames.Count > 0)
                {
                    Atribute atributeForName = atributesWithDefaultNames.Find(x => x.ScaledCost == atributesWithDefaultNames.Max(y => y.ScaledCost));
                    itemName += atributeForName.Name;
                }
            }

            if (itemType is Type.one_handed or Type.two_handed or Type.missle_weapon)
            {
                itemName = Enum.GetName(weaponType) + " of " + itemName + " (" + id + ")";
            }
            else
            {
                itemName = Enum.GetName(itemType) + " of " + itemName + " (" + id + ")";
            }
            
            return itemName;
        }

        private static AttributeType GetArcheType(Type type)
        {
            int rng = Controller.Rng.Next(0, 6);

            if (type is Type.missle_weapon && rng is 3 or 4 or 5) // dit stuk is echt gruwelijk
            {
                rng = 0;
            }
            else if (type is Type.one_handed or Type.two_handed && rng is 4 or 5)
            {
                rng = rng is 4 ? 0 : 1;
            }

            if (Controller.BoolFromRng(50) && rng == 4)
            {
                rng--;
            }

            return rng switch
            {
                0 => AttributeType.Thugging,
                1 => AttributeType.Counterthug,
                2 => AttributeType.Commander,
                3 => AttributeType.Magic,
                4 => AttributeType.Stealth,
                5 => AttributeType.Other,
                _ => AttributeType.Thugging,
            };
        }

        private static Type GetItemType()
        {
            int typeRng = Controller.Rng.Next(1, 81);
            Type type = typeRng switch
            {
                < 10 => Type.one_handed,
                < 20 => Type.two_handed,
                < 30 => Type.missle_weapon,
                < 40 => Type.shield,
                < 50 => Type.armor,
                < 55 => Type.helmet,
                < 60 => Type.crown,
                < 70 => Type.boots,
                _ => Type.misc
            };
            return type;
        }
    }

    public enum Type
    {
        one_handed = 1,
        two_handed = 2,
        missle_weapon = 3,
        shield = 4,
        armor = 5,
        helmet = 6,
        boots = 7,
        misc = 8,
        crown = 9,
    }

    public enum MagicPath
    {
        fire = 0,
        air = 1,
        water = 2,
        earth = 3,
        astral = 4,
        death = 5,
        nature = 6,
        blood = 7,
        empty = 8,
    }
}