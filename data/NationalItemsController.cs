using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace data
{
    public static class NationalItemsController
    {
        public static List<Nation> Nations { get; set; } = DefaultNationsEarlyAge.Where(x => x.Id is 22 or 15).ToList();

        public static readonly Nation[] DefaultNationsEarlyAge = new[]
        {
            new Nation(5,"EA  Arcoscephale  -  Golden Era", MagicPath.nature, MagicPath.astral, MagicPath.air),
            new Nation(6,"EA  Ermor  -  New Faith", MagicPath.fire, MagicPath.air, MagicPath.astral),
            new Nation(7,"EA  Ulm  -  Enigma of Steel", MagicPath.earth, MagicPath.nature, MagicPath.empty),
            new Nation(8,"EA  Marverni  -  Time of Druids", MagicPath.earth, MagicPath.astral, MagicPath.nature),
            new Nation(9,"EA  Sauromatia  -  Amazon Queens", MagicPath.death, MagicPath.nature, MagicPath.blood),
            new Nation(10,"EA  T'ien Ch'i  -  Spring and Autumn", MagicPath.astral, MagicPath.water, MagicPath.air),
            new Nation(11,"EA  Machaka  -  Lion Kings", MagicPath.fire, MagicPath.nature, MagicPath.earth),
            new Nation(12,"EA  Mictlan  -  Reign of Blood", MagicPath.blood, MagicPath.fire, MagicPath.nature),
            new Nation(13,"EA  Abysia  -  Children of Flame", MagicPath.fire, MagicPath.blood, MagicPath.astral),
            new Nation(14,"EA  Caelum  -  Eagle Kings", MagicPath.air, MagicPath.water, MagicPath.empty),
            new Nation(15,"EA  C'tis  -  Lizard Kings", MagicPath.death, MagicPath.nature, MagicPath.empty),
            new Nation(16,"EA  Pangaea  -  Age of Revelry", MagicPath.nature, MagicPath.earth, MagicPath.water),
            new Nation(17,"EA  Agartha  -  Pale Ones", MagicPath.earth, MagicPath.water, MagicPath.empty),
            new Nation(18,"EA  Tir na n'Og  -  Land of the Ever Young", MagicPath.air, MagicPath.nature, MagicPath.water),
            new Nation(19,"EA  Fomoria  -  The Cursed Ones", MagicPath.astral, MagicPath.death, MagicPath.empty),
            new Nation(20,"EA  Vanheim  -  Age of Vanir", MagicPath.air, MagicPath.blood, MagicPath.earth),
            new Nation(21,"EA  Helheim  -  Dusk and Death", MagicPath.death, MagicPath.air, MagicPath.blood),
            new Nation(22,"EA  Niefelheim  -  Sons of Winter", MagicPath.water, MagicPath.death, MagicPath.blood),
            new Nation(24,"EA  Rus  -  Sons of Heaven", MagicPath.air, MagicPath.nature, MagicPath.empty),
            new Nation(25,"EA  Kailasa  -  Rise of the Ape Kings", MagicPath.water, MagicPath.astral, MagicPath.nature),
            new Nation(26,"EA  Lanka  -  Land of Demons", MagicPath.air, MagicPath.blood, MagicPath.empty),
            new Nation(27,"EA  Yomi  -  Oni Kings", MagicPath.death, MagicPath.earth, MagicPath.fire),
            new Nation(28,"EA  Hinnom  -  Sons of the Fallen", MagicPath.blood, MagicPath.astral, MagicPath.empty),
            new Nation(29,"EA  Ur  -  The First City", MagicPath.earth, MagicPath.nature, MagicPath.water),
            new Nation(30,"EA  Berytos  -  The Phoenix Empire", MagicPath.blood, MagicPath.air, MagicPath.fire),
            new Nation(31,"EA  Xibalba  -  Vigil of the Sun", MagicPath.death, MagicPath.blood, MagicPath.nature),
            new Nation(32,"EA  Mekone  -  Brazen Giants", MagicPath.earth, MagicPath.fire, MagicPath.air),
            new Nation(33,"EA  Ubar  -  Kingdom of the Unseen", MagicPath.fire, MagicPath.air, MagicPath.empty),
            new Nation(36,"EA  Atlantis  -  Emergence of the Deep Ones", MagicPath.earth, MagicPath.water, MagicPath.fire),
            new Nation(37,"EA  R'lyeh  -  Time of Aboleths", MagicPath.astral, MagicPath.water, MagicPath.empty),
            new Nation(38,"EA  Pelagia  -  Pearl Kings", MagicPath.water, MagicPath.astral, MagicPath.nature),
            new Nation(39,"EA  Oceania  -  Coming of the Capricorns", MagicPath.earth, MagicPath.nature, MagicPath.empty),
            new Nation(40,"EA  Therodos  -  Telkhine Spectre", MagicPath.nature, MagicPath.water, MagicPath.earth),
        };

        public static readonly Nation[] DefaultNationsMiddleAge = new[]
        {
            new Nation(43,"Arcosc", MagicPath.nature, MagicPath.astral, MagicPath.air),
            new Nation(44,"Arcosc", MagicPath.fire, MagicPath.air, MagicPath.astral),
            new Nation(45,"Arcosc", MagicPath.earth, MagicPath.nature, MagicPath.empty),

        };

        public static readonly Nation[] DefaultNationsLateAge = new[]
        {
            new Nation(80,"Arcosc", MagicPath.nature, MagicPath.astral, MagicPath.air),
            new Nation(81,"Arcosc", MagicPath.fire, MagicPath.air, MagicPath.astral),
            new Nation(82,"Arcosc", MagicPath.earth, MagicPath.nature, MagicPath.empty),

        };

        public static void ModifyItemsIntoNationals(bool nationalOnly) 
        {
            if (Nations.Count != 0)
            {
                foreach (var nation in Nations)
                {
                    List<Item> bothPathItem = Controller.ItemList.Where(x => x.MainPath == nation.MainMagicPath && x.CrossPath == nation.SecondaryMagicPath && x.AtributeList.Count < 14 && x.ConstLevel < 8).ToList();
                    bothPathItem = bothPathItem.Concat(Controller.ItemList.Where(x => x.MainPath == nation.SecondaryMagicPath && x.CrossPath == nation.MainMagicPath && x.AtributeList.Count < 14 && x.ConstLevel < 8)).ToList();

                    if (bothPathItem.Count > 2)
                    {
                        // add discount
                        bothPathItem[0].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                        bothPathItem[1].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                        if (nationalOnly)
                        {
                            bothPathItem[0].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            bothPathItem[1].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                        }
                    }
                    else
                    {
                        List<Item> secondaryPathItem = Controller.ItemList.Where(x => x.MainPath == nation.SecondaryMagicPath && x.AtributeList.Count < 14).ToList();
                        List<Item> mainPathItem = Controller.ItemList.Where(x => x.MainPath == nation.MainMagicPath && x.AtributeList.Count < 14).ToList();
                        if (secondaryPathItem.Count >= 1 && mainPathItem.Count >= 1)
                        {
                            secondaryPathItem[0].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            mainPathItem[0].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            if (nationalOnly)
                            {
                                secondaryPathItem[0].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                                mainPathItem[0].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            }
                        }
                        else if (mainPathItem.Count >= 2 )
                        {
                            mainPathItem[0].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            mainPathItem[1].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            if (nationalOnly)
                            {
                                mainPathItem[0].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                                mainPathItem[1].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            }
                        }
                        else if (secondaryPathItem.Count >= 2)
                        {
                            secondaryPathItem[0].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            secondaryPathItem[1].AtributeList.Add(new Atribute("national discount", "#nationrebate", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            if (nationalOnly)
                            {
                                secondaryPathItem[0].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                                secondaryPathItem[1].AtributeList.Add(new Atribute("national only", "#restricted", Array.Empty<MagicPath>(), Array.Empty<AttributeType>(), 0, 0, nation.Id, 0, 0, 0, 0));
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No national item for nation: " + nation.Id);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("!! No nations !!");
            }
        }

    }

    public class Nation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MagicPath MainMagicPath { get; set; }
        public MagicPath SecondaryMagicPath { get; set; }
        public MagicPath TertiaryMagicPath { get; set; }

        public Nation(int id,string name, MagicPath mainMagicPath, MagicPath secondaryMagicPath, MagicPath tertiaryMagicPath)
        {
            Id = id;
            Name = name;
            MainMagicPath = mainMagicPath;
            SecondaryMagicPath = secondaryMagicPath;
            TertiaryMagicPath = tertiaryMagicPath;
        }

        public Nation(int id, string name, MagicPath mainMagicPath, MagicPath secondaryMagicPath)
        {
            Id = id;
            Name = name;
            MainMagicPath = mainMagicPath;
            SecondaryMagicPath = secondaryMagicPath;
            TertiaryMagicPath = MagicPath.empty;
        }
    }
}
