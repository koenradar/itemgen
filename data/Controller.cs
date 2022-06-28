using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace data
{
    /// <summary>
    /// Logic controller class
    /// </summary>
    public static class Controller
    {
        public static int AmountOfItems { get; set; } = 887; // default 499 max possible 887 

        public static bool DebugLogging1 = false;
        public static bool MagicGen = false;
        public static string MagicGenName = "";
        public static Random Rng { get; set; } = new();
        public static uint GenNumber { get; set; }
        private static string Path { get; set; }

        public static string SpriteLocation { get; set; }
        public static List<Item> ItemList { get; set; } = new();
        public static List<Weapon> WeaponList { get; set; } = new();
        public static List<Armor> ArmorList { get; set; } = new();
        public static List<int> NessesaryVanillaItems { get; set; } = new();
        public static List<SecondaryEffect> SecondaryEffectList { get; set; } = new();

        public static List<SecondaryEffect> UsedSecondaryEffects { get; set; } = new();

        public static List<Spell> Spells { get; set; } = new List<Spell>();
        public static List<EffectSpell> SpellEffects { get; set; }

        public static List<int> ForgeLevelWeights { get; set; }
        public static int ForgeLevelWeightTotal { get; set; }
        public static List<int> CrossLevelWeights { get; set; }
        public static int CrossLevelWeightsTotal { get; set; }
        public static int ScalingDoublingIncrement { get; private set; } = 20;

        /// <summary>
        /// decide the settings for itemgen
        /// </summary>
        /// <returns>IsSuccesfull</returns>
        public static void Settings(int amountOfItems = 887, bool magicGen = false, bool debugging = false, int scalingIncrement = 20, string magicGenName = "", int defaultPoints = 10, double pointMultiplier = 0.7)
        {
            AmountOfItems = amountOfItems;
            MagicGen = magicGen;
            DebugLogging1 = debugging;
            ScalingDoublingIncrement = scalingIncrement;
            MagicGenName = magicGenName;
            Item.DefaultPoints = defaultPoints;
            Item.PointMultiplier = pointMultiplier;
        }
        
        // TODO -> refactor this shit
        public static MagicPath GetMainPathAndCrossPathLevelsAndOrder(MagicPath mainMagicPath, out int mainPathLevel, out MagicPath crossMagicPath, out int? crossPathLevel)
        {
            int mainpathRNG = Rng.Next(1, ForgeLevelWeightTotal);
            int mainpathRNGAcc = 0;
            int mainpathHelper = 1;
            mainPathLevel = 0;
            foreach (int weight in ForgeLevelWeights)
            {
                mainpathRNGAcc += weight;
                if (mainpathRNG <= mainpathRNGAcc)
                {
                    mainPathLevel = mainpathHelper;
                    break;
                }
                mainpathHelper += 1;
            }

            crossMagicPath = (MagicPath)Rng.Next(1, 9);
            crossPathLevel = null;

            // crosspath may not be main path
            while (crossMagicPath == mainMagicPath)
            {
                crossMagicPath = (MagicPath)Rng.Next(1, 9);
            }

            // rng for crosspath level of items
            if (crossMagicPath != MagicPath.empty)
            {
                int crosspathRNG = Rng.Next(1, CrossLevelWeightsTotal);
                int crosspathRNGAcc = 0;
                int crosspathHelper = 0;
                foreach (int weight in CrossLevelWeights)
                {
                    crosspathRNGAcc += weight;
                    if (crosspathRNG <= crosspathRNGAcc)
                    {
                        crossPathLevel = crosspathHelper;
                        break;
                    }
                    crosspathHelper += 1;
                }
            }

            if (crossPathLevel is 0 or null)
            {
                crossMagicPath = MagicPath.empty;
            }

            // If crosspath has a higher level than the main path swap them
            if (crossPathLevel > mainPathLevel)
            {
                int tempCrossLevel = (int)crossPathLevel;
                MagicPath tempCrossPath = crossMagicPath;

                crossMagicPath = mainMagicPath;
                crossPathLevel = mainPathLevel;

                mainPathLevel = tempCrossLevel;
                mainMagicPath = tempCrossPath;
            }

            return mainMagicPath;
        }

        public static int GetConstLevel()
        {
            int constLevelRNG = Rng.Next(0, 101);
            int constLevel;
            if (constLevelRNG < 15)
            {
                constLevel = 0;
                //  TODO change from static to local (multi threading or anything will break this) no static forgelevels like this
                ForgeLevelWeights = new List<int> { 90, 45, 20, 0, 0, 0, 0, 0, 0 };
                ForgeLevelWeightTotal = ForgeLevelWeights.Sum();
                CrossLevelWeights = new List<int> { 90, 70, 25, 0, 0, 0, 0, 0, 0, 0 };
                CrossLevelWeightsTotal = CrossLevelWeights.Sum();
            }
            else if (constLevelRNG < 40)
            {
                constLevel = 2;
                ForgeLevelWeights = new List<int> { 90, 45, 20, 15, 0, 0, 0, 0, 0 };
                ForgeLevelWeightTotal = ForgeLevelWeights.Sum();
                CrossLevelWeights = new List<int> { 90, 70, 25, 0, 0, 0, 0, 0, 0, 0 };
                CrossLevelWeightsTotal = CrossLevelWeights.Sum();
            }
            else if (constLevelRNG < 65)
            {
                constLevel = 4;
                ForgeLevelWeights = new List<int> { 70, 45, 20, 15, 10, 0, 0, 0, 0 };
                ForgeLevelWeightTotal = ForgeLevelWeights.Sum();
                CrossLevelWeights = new List<int> { 80, 50, 25, 20, 0, 0, 0, 0, 0, 0 };
                CrossLevelWeightsTotal = CrossLevelWeights.Sum();
            }
            else if (constLevelRNG < 90)
            {
                constLevel = 6;
                ForgeLevelWeights = new List<int> { 70, 45, 20, 15, 10, 5, 0, 0, 0 };
                ForgeLevelWeightTotal = ForgeLevelWeights.Sum();
                CrossLevelWeights = new List<int> { 80, 50, 25, 20, 10, 0, 0, 0, 0, 0 };
                CrossLevelWeightsTotal = CrossLevelWeights.Sum();
            }
            else
            {
                constLevel = 8;
                ForgeLevelWeights = new List<int> { 5, 15, 15, 25, 25, 15, 3, 0, 0 };
                ForgeLevelWeightTotal = ForgeLevelWeights.Sum();
                CrossLevelWeights = new List<int> { 20, 0, 0, 10, 10, 10, 2, 0, 0, 0 };
                CrossLevelWeightsTotal = CrossLevelWeights.Sum();
            }

            return constLevel;
        }

        /// <summary>
        /// Generate X amount of items
        /// </summary>
        /// <param name="amountOfItems"></param>
        /// <returns></returns>
        private static List<Item> FillItemList(int amountOfItems)
        {
            if (Atribute.AllAtributes.Count == 0)
            {
                throw new FileNotFoundException($"No attributes found or loaded check for {Path}");
            }
            if (amountOfItems > 0)
            {
                ItemList = new List<Item>();
                for (int i = 0; i < AmountOfItems; i++)
                {
                    ItemList.Add(Item.GenerateItem());
                }

                //if (NationalItemsController.Nations.Count > 0)
                //{
                //    NationalItemsController.ModifyItemsIntoNationals(false);
                //}
                return ItemList;
            }
            return new List<Item>();
        }

        /// <summary>
        /// Write a single Item to the file
        /// </summary>
        /// <param name="item"></param>
        private static void WriteItemToFile(Item item)
        {
            using System.IO.StreamWriter file = new(Path, true);
            foreach (string line in item.ToStringList())
            {
                file.WriteLine(line);
            }
        }

        /// <summary>
        /// Write a single weapon to the file
        /// </summary>
        /// <param name="weapon"></param>
        private static void WriteWeaponToFile(Weapon weapon)
        {
            using System.IO.StreamWriter file = new(Path, true);

            foreach (string line in weapon.ToStringList())
            {
                file.WriteLine(line);
            }
        }

        /// <summary>
        /// Write a single weapon to the file
        /// </summary>
        /// <param name="weapon"></param>
        private static void WriteSecondaryEffectToFile(SecondaryEffect secondary)
        {
            using System.IO.StreamWriter file = new(Path, true);

            foreach (string line in secondary.ToStringList())
            {
                file.WriteLine(line);
            }
        }

        /// <summary>
        /// Write a single Armor to the file
        /// </summary>
        /// <param name="armor"></param>
        private static void WriteArmorToFile(Armor armor)
        {
            using System.IO.StreamWriter file = new(Path, true);

            foreach (string line in armor.ToStringList())
            {
                file.WriteLine(line);
            }
        }

        /// <summary>
        /// Generates everything every Itemgen file needs at the start
        /// </summary>
        private static void GenerateFileStart()
        {
            if (System.IO.File.Exists(Path))
            {
                throw new IOException($"File to generate already exists at {Path}");
            }
            else
            {
                GenNumber = (uint)Rng.Next(1, 10000000);
                Path = System.AppDomain.CurrentDomain.BaseDirectory + $"/output/itemgen_{GenNumber}.dm";
                if (DebugLogging1)
                {
                    Debug.WriteLine("+++++----+++++");
                    Debug.WriteLine(Path);
                    Debug.WriteLine("+++++----+++++");
                }
                WriteModDescription(GenNumber);
                UnforgableNormalMagicItems(GenNumber);

                // TODO make this a function (createContentFolder())
                string folderDirectory = System.AppDomain.CurrentDomain.BaseDirectory + $"/output/ItemGen_{GenNumber}/";
                SpriteLocation = folderDirectory + "Sprites/";
                if (System.IO.Directory.Exists(folderDirectory))
                {
                    throw new IOException($"Folder to generate already exists at {folderDirectory}");
                }
                else
                {
                    System.IO.Directory.CreateDirectory(folderDirectory);

                    var files = System.IO.Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "/input/Sprites"); // TODO ADD MORE SPRITE OPTIONS
                    foreach (var file in files)
                    {
                        string[] splitString = file.Split('/');
                        string name = splitString[^1];
                        System.IO.Directory.CreateDirectory(folderDirectory + "Sprites");
                        System.IO.File.Copy(file, folderDirectory + $"{name}");
                    }

                    var dirs = Directory.EnumerateDirectories(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites");
                    foreach (var dir in dirs)
                    {
                        System.IO.Directory.CreateDirectory(folderDirectory + dir.Split("\\")[^1]);

                        var localFiles = Directory.EnumerateFiles(dir);
                        foreach (var file in localFiles)
                        {
                            File.Copy(file, folderDirectory + dir.Split("\\")[^1] + "/" + file.Split("\\")[^1]);   
                        }
                    }

                    System.IO.File.Copy("input/ItemGenBanner.tga", folderDirectory + "ItemGenBanner.tga");
                }
            }
        }

        /// <summary>
        /// Reset after generating
        /// </summary>
        private static void EndFile()
        {
            Debug.WriteLine("+++++----+++++");
            Debug.WriteLine(Path);
            Debug.WriteLine("+++++----+++++");
            GenNumber = (uint)Rng.Next(1, 10000000);
            Path = System.AppDomain.CurrentDomain.BaseDirectory + $"/output/itemgen_{GenNumber}.dm";
            Item.ItemCurrentId = 1;
            Weapon.Id = 700;
            Armor.Id = 250;
            ItemList = new List<Item>();
            WeaponList = new List<Weapon>();
            ArmorList = new List<Armor>();
        }

        /// <summary>
        /// Generates the mod by calling generating functions
        /// </summary>
        public static void GenerateMod()
        {
            SetSpells();
            ItemList = FillItemList(AmountOfItems);
            EnsureBoosterPerPath();

            GenerateFileStart();
            foreach (var item in ItemList)
            {
                WriteItemToFile(item);
            }
            foreach (var weapon in WeaponList)
            {
                WriteWeaponToFile(weapon);
            }
            foreach (var secondaryEffect in UsedSecondaryEffects)
            {
                WriteSecondaryEffectToFile(secondaryEffect);
            }
            foreach (var armor in ArmorList)
            {
                WriteArmorToFile(armor);
            }
            EndFile();
        }

        /// <summary>
        /// Make sure one booster is generated between const 2-6 per path
        /// </summary>
        private static void EnsureBoosterPerPath()
        {
            foreach (MagicPath magicPath in Enum.GetValues(typeof(MagicPath)))
            {
                if (magicPath == MagicPath.empty) continue;
                if (ItemList.Any(x => x.MainPath == magicPath && x.CrossPath == MagicPath.empty && x.ConstLevel is > 0 and < 8 && x.AtributeList.Any(att => att.DomminionsModTag == "#magicboost"))) continue;
                List<Item> localItems = ItemList.Where(x => x.MainPath == magicPath && x.ConstLevel is > 2 and < 8 && x.MainPathLevel > 2 && x.CrossPath == MagicPath.empty).ToList();
                if (localItems.Count == 0) continue;

                while (true)
                { 
                    int itemToBooster = Rng.Next(0, localItems.Count);

                    if (Atribute.AllAtributes.All(x => x.DomminionsModTag != "#magicboost"))
                    { 
                        break;
                    }
                    if (Atribute.AllAtributes.All(x => x.DomminionsModTag != "#tainted"))
                    {
                        break;
                    }

                    if (Atribute.CheckForAtributeConflicts("#magicboost", localItems[itemToBooster].AtributeList))
                    { 
                        continue;
                    }

                    Atribute att = new(Atribute.AllAtributes.First(x => x.DomminionsModTag == "#magicboost"))
                    {
                        Power = 1
                    };

                    localItems[itemToBooster].AtributeList.Add(att);

                    att = new(Atribute.AllAtributes.First(x => x.DomminionsModTag == "#tainted"))
                    {
                        Power = 35
                    };

                    localItems[itemToBooster].AtributeList.Add(att);
                    localItems[itemToBooster].Description += "(Magicboost 1) (tainted 35)";
                    localItems[itemToBooster].SpriteOld = Enum.GetName(localItems[itemToBooster].ItemType) + "_booster.tga";
                    break;
                }
            }
        }


        private static void WriteModDescription(uint genNumber)
        {
            List<string> descriptionList = new()
            {
                $"#modname \"ItemGen-Test {genNumber}\"",
                "#description \"A mod that randomly generates items in the same vain of nationgen and magicgen except its made in a real programming language\"",
                "#version 0.1",
                $"#icon \"./ItemGen_{genNumber}/ItemGenBanner.tga\""
            };

            using System.IO.StreamWriter file = new(Path, true);
            foreach (string line in descriptionList)
            {
                file.WriteLine(line);
            }
        }

        /// <summary>
        /// Set the first 499 Items to unforgable cant remove them without Errors also add mod name
        /// </summary>
        private static void UnforgableNormalMagicItems(uint genNumber)
        {
            List<string> unforgableNormalItems = new();
            if (false)
            {
                for (int i = 1; i <= 499; i++)
                {
                    unforgableNormalItems.Add($"#selectitem {i}");
                    unforgableNormalItems.Add("#constlevel 12");
                    unforgableNormalItems.Add("#end");
                }
            }
            else
            {
                foreach (int itemId in NessesaryVanillaItems)
                {
                    unforgableNormalItems.Add($"#selectitem {itemId}");
                    unforgableNormalItems.Add("#constlevel 12");
                    unforgableNormalItems.Add("#end");
                }
            }

            using System.IO.StreamWriter file = new(Path, true);
            foreach (string line in unforgableNormalItems)
            {
                file.WriteLine(line);
            }
        }

        public static int PathLevelToGemCost(int pathLevel)
        {
            return pathLevel switch
            {
                1 => 5,
                2 => 10,
                3 => 15,
                4 => 25,
                5 => 40,
                6 => 60,
                7 => 80,
                8 => 100,
                9 => 120,
                _ => 0,
            };
        }

        public static bool BoolFromRng(int chanceToSucceed)
        {
            return Rng.Next(0, 101) <= chanceToSucceed;
        }

        public static void ReadSpellJson()
        {
            List<Spell> spells;
            string spellsJson = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "input/spells.json");
            spells = JsonConvert.DeserializeObject<List<Spell>>(spellsJson,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }
            );
            Spells = spells;
             
            List<EffectSpell> spellEffects;
            string spellEffectsJson = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "input/effects_spells.json");
            spellEffects = JsonConvert.DeserializeObject<List<EffectSpell>>(spellEffectsJson,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }
            );
            SpellEffects = spellEffects;
        }

        public static void SetSpells()
        {
            ReadSpellJson();
            if (MagicGen && !string.IsNullOrEmpty(MagicGenName))
            {
                Spells = new List<Spell>();
                ReadMagicGenDm(MagicGenName);
            }
            if (Spells.Count == 0 || SpellEffects.Count == 0)
            {
                Debug.WriteLine("No spell json found");
            }
            else
            {
                foreach (var spell in Spells)
                {
                    spell.EffectSpell = SpellEffects.First(x => x.Record_Id == spell.Effect_Record_Id);
                }

                Atribute.UpdateAtributeLists(GenerateSpellAtributeList());
            }
        }

        /// <summary>
        /// Static constructor to fill some statics
        /// </summary>
        static Controller()
        {
            List<Atribute> tempAtributeList = AtributeJSON.ReadAtributeList();
            
            Sprite a = new Sprite("test", Type.two_handed);

            var lines = File.ReadLines(System.AppDomain.CurrentDomain.BaseDirectory + "/input/NeccesaryItemsVanilla.txt");
            foreach (string line in lines)
            {
                int NeccesarItemID = int.Parse(line);
                NessesaryVanillaItems.Add(NeccesarItemID);
            }

            if (tempAtributeList.Count > 0)
            {
                Atribute.UpdateAtributeLists(tempAtributeList);
            }
            else
            {
                if (DebugLogging1)
                {
                    Debug.WriteLine("Cant read Atributelist");
                }
            }

            List<SecondaryEffect> tempEffectList = SecondaryEffectJSON.ReadSecondaryEffectList();

            if (tempEffectList.Count > 0)
            {
                SecondaryEffectList = tempEffectList;
            }
            else
            {
                Debug.WriteLine("No SecondaryEffect");
            }
        }

        private static void ReadMagicGenDm(string fileName)
        {
            string FilePath = System.AppDomain.CurrentDomain.BaseDirectory + "/input/" + fileName;
            string[] dmLines = File.ReadAllLines(FilePath);

            // Spells.Where(x =>x.FatigueCost is >= 20 and <= 80 && x.EffectSpell.Ritual == 0 && x.Path1 != -1).ToList();
            List<int> ids = new List<int>();
            List<string> names = new List<string>();
            List<int> researchLevels = new List<int>();
            List<int> path1s = new List<int>();
            List<int> pathLevel1s = new List<int>();
            List<int> path2s = new List<int>();
            List<int> pathLevel2s = new List<int>();
            List<int> fatigueCosts = new List<int>();
            List<int> effectRecordIds = new List<int>();
            List<bool> isRituals = new List<bool>();

            bool newSpell = false;
            
            foreach (string line in dmLines)
            {
                if (line.Contains("#newspell"))
                {
                    int y = 0;
                    List<char> chars = line.Where(x => int.TryParse(x.ToString(), out y)).ToList();
                    string numbers = "";
                    foreach (var c in chars)
                    {
                        numbers += c;
                    }
                    ids.Add(int.Parse(numbers));
                    newSpell = true;
                }
                else if (line.Contains("#name") && newSpell)
                {
                    int y = 0;
                    string[] strings = line.Split("\"");
                    
                    names.Add(strings[1]);
                }
                else if (line.Contains("#effect") && newSpell)
                {
                    int y = 0;
                    List<char> chars = line.Where(x => int.TryParse(x.ToString(), out y)).ToList();
                    string numbers = "";
                    foreach (var c in chars)
                    {
                        numbers += c;
                    }

                    int id = int.Parse(numbers);
                    if (id >= 10000)
                    {
                        id -= 10000;
                        isRituals.Add(true);
                       
                    }
                    else if (id >= 1000)
                    {
                        id %= 1000;
                        isRituals.Add(false);
                    }
                    else
                    {
                        isRituals.Add(false);
                    }

                    effectRecordIds.Add(id);
                }  
                else if (line.Contains("#path ") && newSpell)
                {
                    int y = 0;
                    List<char> chars = line.Where(x => int.TryParse(x.ToString(), out y)).ToList();
                    string numbers = "";
                    foreach (var c in chars)
                    {
                        numbers += c;
                    }

                    string one = numbers[0].ToString();
                    string two = numbers[1].ToString();

                    int mainOrSecond = int.Parse(one);
                    int path = int.Parse(two);

                    if (mainOrSecond == 0)
                    {
                        path1s.Add(path);
                    }
                    else
                    {
                        path2s.Add(path);
                    }

                   
                }
                else if (line.Contains("#pathlevel") && newSpell)
                {
                    int y = 0;
                    List<char> chars = line.Where(x => int.TryParse(x.ToString(), out y)).ToList();
                    string numbers = "";
                    foreach (var c in chars)
                    {
                        numbers += c;
                    }

                    string one = numbers[0].ToString();
                    string two = numbers[1].ToString();

                    int mainOrSecond = int.Parse(one);
                    int path = int.Parse(two);

                    if (mainOrSecond == 0)
                    {
                        pathLevel1s.Add(path);
                    }
                    else
                    {
                        pathLevel2s.Add(path);
                    }

                }
                else if (line.Contains("#researchlevel") && newSpell)
                {
                    int y = 0;
                    List<char> chars = line.Where(x => int.TryParse(x.ToString(), out y)).ToList();
                    string numbers = "";
                    foreach (var c in chars)
                    {
                        numbers += c;
                    }
                    researchLevels.Add(int.Parse(numbers));
                }
                else if (line.Contains("#fatiguecost") && newSpell)
                {
                    int y = 0;
                    List<char> chars = line.Where(x => int.TryParse(x.ToString(), out y)).ToList();
                    string numbers = "";
                    foreach (var c in chars)
                    {
                        numbers += c;
                    }
                    numbers.Split();
                    fatigueCosts.Add(int.Parse(numbers));
                }
                else if (line.Contains("#end"))
                {
                    newSpell = false;
                }
            }

            for (int i = 0; i < ids.Count; i++)
            {
                Spell spell = new Spell(ids[i], names[i], researchLevels[i], path1s[i], pathLevel1s[i], path2s[i], pathLevel2s[i], fatigueCosts[i], effectRecordIds[i], isRituals[i] );
                Spells.Add(spell);
            }
        }

        private static List<Atribute> GenerateSpellAtributeList()
        {
            List<Atribute> atributesToAdd = new List<Atribute>();
            List<Spell> localSpells = Spells.Where(x =>x.FatigueCost is >= 20 and <= 80 && (x.EffectSpell.Ritual == 0 || !x.IsRitual )&& x.School != -1 && x.Path1 != -1 && x.ResearchLevel != -1 && !x.Name.Contains("Additional") && !x.Name.Contains("Extra")).ToList();

            foreach (var nationalSpell in NationalSpell.NationalSpellsList)
            {
                localSpells.RemoveAll(x => x.Id == nationalSpell.ID);
            }
            
            AttributeType[] attributeTypes = new AttributeType[3];
            attributeTypes[0] = AttributeType.Thugging;
            attributeTypes[1] = AttributeType.Magic;
            attributeTypes[2] = AttributeType.Commander;

            foreach (var spell in localSpells)
            {

                int cost = 10;
                int scalingCost = 10;

                MagicPath[] spellMagicPaths;
                if (spell.Path2 != -1)
                {
                    spellMagicPaths = new MagicPath[2];
                    spellMagicPaths[0] = (MagicPath)spell.Path1;
                    spellMagicPaths[1] = (MagicPath)spell.Path2;
                    cost += (spell.PathLevel1 + spell.PathLevel2) ^ 2;
                    scalingCost += (spell.PathLevel1 + spell.PathLevel2) ^ 3;
                }
                else
                {
                    spellMagicPaths = new MagicPath[1];
                    spellMagicPaths[0] = (MagicPath)spell.Path1;
                    cost += spell.PathLevel1 ^ 2;
                    scalingCost += spell.PathLevel1 ^ 3;
                }

                int researchlevel = spell.ResearchLevel;
                cost += researchlevel * 2;
                scalingCost += researchlevel * 2;
                if (researchlevel % 2 != 0)
                {
                    researchlevel++;
                }
                else if (researchlevel > 8)
                {
                    researchlevel = 8;
                }

                cost *= (spell.FatigueCost / 20);
                scalingCost *= (spell.FatigueCost / 20);


                Atribute addAtribute = new(spell.Name, "#autospellrepeat", spellMagicPaths, attributeTypes, cost,
                    researchlevel, 1, 1, 1, scalingCost, 1);
                addAtribute.MaxValue = 20;
                atributesToAdd.Add(addAtribute);
            }

            return atributesToAdd;
        }

    }
}