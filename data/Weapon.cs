using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace data
{
    public class Weapon : IEquipment
    {
        public static int Id { get; set; } = 900;

        public int ThisId { get; set; } = Id;

        public string Name { get; set; } = "name not set @koenradar";

        private SecondaryEffect ThisSecondaryEffect { get; set; }

        public uint AOE { get; set; }

        public int Damage { get; set; }

        public List<DamageType> ThisDamageTypes { get; set; }

        public WeaponType ThisWeaponType { get; set; }

        public int NumberOfAttacks { get; set; }

        public int Range { get; set; }
        public int Ammunition { get; private set; }
        public int AttackValue { get; private set; }
        public int DefenceValue { get; private set; }
        public int Length { get; private set; }

        public bool UwOk { get; set; } = false;

        // Melee
        public Weapon(int numberOfAttacks, int damage, int attackValue, int defenceValue, List<DamageType> damageTypes, int length, WeaponType weaponType)
        {
            ThisId = Id;
            Id++;
            NumberOfAttacks = numberOfAttacks;
            Damage = damage;
            AttackValue = attackValue;
            DefenceValue = defenceValue;
            ThisDamageTypes = damageTypes;
            Length = length;
            ThisWeaponType = weaponType;
        }

        //Ranged
        public Weapon(int numberOfAttacks, int damage, int attackValue, List<DamageType> damageTypes, int range, int ammunition, WeaponType weaponType)
        {
            ThisId = Id;
            Id++;
            NumberOfAttacks = numberOfAttacks;
            Damage = damage;
            AttackValue = attackValue;
            ThisDamageTypes = damageTypes;
            Range = range;
            Ammunition = ammunition;
            ThisWeaponType = weaponType;
        }

        public static Weapon GenerateWeapon(int points, Type type, int constLevel, AttributeType archeType , MagicPath[] weaponMagicPaths, out int cost)
        {
            cost = 0;
            WeaponType creatingWeaponType = WeaponTypeGenerator(type, archeType);

            Weapon creatingWeapon = null;
            // Standard for weapon type and standard cost
            switch (creatingWeaponType)
            {
                case WeaponType.One_Handed_Sword:
                    cost += 4;
                    creatingWeapon = new Weapon(1, 8, 1, 2, new List<DamageType>() {DamageType.slash}, 2, WeaponType.One_Handed_Sword);
                    break;

                case WeaponType.One_Handed_Dagger:
                    cost += 2;
                    creatingWeapon = new Weapon(1, 4, 2, 1, new List<DamageType>() { DamageType.piercing, DamageType.armorPiercing }, 1, WeaponType.One_Handed_Dagger);
                    break;

                case WeaponType.One_Handed_flail:
                    cost += 4;
                    creatingWeapon = new Weapon(1, 10, 2, -2, new List<DamageType>() { DamageType.blunt }, 1, WeaponType.One_Handed_flail);
                    break;

                case WeaponType.One_Handed_Axe:
                    cost += 4;
                    creatingWeapon = new Weapon(1, 9, 2, 0, new List<DamageType>() { DamageType.slash }, 2, WeaponType.One_Handed_Axe);
                    break;

                case WeaponType.One_Handed_Staff:
                    cost += 1;
                    creatingWeapon = new Weapon(1, 8, 1, 1, new List<DamageType>() { DamageType.blunt }, 2, WeaponType.One_Handed_Staff);
                    break;

                case WeaponType.One_Handed_Spear:
                    cost += 8;
                    creatingWeapon = new Weapon(1, 8, 1, 0, new List<DamageType>() { DamageType.piercing }, 3, WeaponType.One_Handed_Spear);
                    break;

                case WeaponType.Two_Handed_Sword:
                    cost += 6;
                    creatingWeapon = new Weapon(1, 12, 2, 1, new List<DamageType>() { DamageType.slash }, 3, WeaponType.Two_Handed_Sword);
                    break;

                case WeaponType.Two_Handed_Hammer:
                    cost += 6;
                    creatingWeapon = new Weapon(1, 14, 3, 0, new List<DamageType>() { DamageType.blunt }, 3, WeaponType.Two_Handed_Hammer);
                    break;

                case WeaponType.Two_Handed_Axe:
                    cost += 6;
                    creatingWeapon = new Weapon(1, 13, 3, 0, new List<DamageType>() { DamageType.slash }, 3, WeaponType.Two_Handed_Axe);
                    break;

                case WeaponType.Two_Handed_Staff:
                    cost += 2;
                    creatingWeapon = new Weapon(1, 8, 0, 0, new List<DamageType>() { DamageType.slash }, 3, WeaponType.Two_Handed_Staff);
                    break;

                case WeaponType.Two_Handed_Pike:
                    cost += 11;
                    creatingWeapon = new Weapon(1, 9, 3, 1, new List<DamageType>() { DamageType.piercing }, 4, WeaponType.Two_Handed_Pike);
                    break;

                case WeaponType.Ranged_Bow:
                    cost += 3;
                    creatingWeapon = new Weapon(1, 12, 3, new List<DamageType>() {DamageType.piercing}, 45 + constLevel * 5, 25, WeaponType.Ranged_Bow);
                    break;

                case WeaponType.Ranged_Crossbow:
                    cost += 4;
                    creatingWeapon = new Weapon(-2, 12, 4, new List<DamageType>() { DamageType.armorPiercing, DamageType.piercing }, 50 + constLevel * 5, 30, WeaponType.Ranged_Crossbow);
                    break;

                case WeaponType.Ranged_Sling:
                    cost += 2;
                    creatingWeapon = new Weapon(1, 8, 4, new List<DamageType>() { DamageType.blunt }, 30 + constLevel * 5, 20, WeaponType.Ranged_Sling);
                    break;
            }

            // Some more variance in the weapons
            /*if (Controller.Rng.Next(1, 101) > 80)
            {
                // Lighter + attack - damage
                int change = Controller.Rng.Next(1, 4);
                creatingWeapon.Damage -= change;
                creatingWeapon.AttackValue += change;
            }
            else if (Controller.Rng.Next(1, 101) > 80)
            {
                // Heavier - attack + damage
                int change = Controller.Rng.Next(1, 4);
                creatingWeapon.Damage += change * 2;
                creatingWeapon.AttackValue -= change;
            }
            else if (Controller.Rng.Next(1, 101) > 80 && type != Type.missle_weapon)
            {
                // Defensive - attack + defense
                int change = Controller.Rng.Next(1, 4);
                creatingWeapon.DefenceValue += change;
                creatingWeapon.AttackValue -= change;
            }
            else if (Controller.Rng.Next(1, 101) > 80 && type != Type.missle_weapon)
            {
                // Offensive + attack - defence
                int change = Controller.Rng.Next(1, 4);
                creatingWeapon.DefenceValue -= change;
                creatingWeapon.AttackValue += change;
            }*/

            double costMultiplier = 1;

            if (type is Type.two_handed or Type.missle_weapon)
            {
                costMultiplier -= 0.25;
            }
            
            if (!creatingWeapon.ThisDamageTypes.Contains(DamageType.armorPiercing))
            {
                if (Controller.BoolFromRng(40 + constLevel * 5))
                {
                    if (Controller.BoolFromRng(10 + constLevel * 5))
                    {
                        creatingWeapon.ThisDamageTypes.Add(DamageType.armorPiercing);
                        creatingWeapon.Damage -= 4;
                        cost += 6;
                        costMultiplier += 2;
                    }
                    else if (Controller.BoolFromRng(5 + constLevel * 5) && constLevel > 2)
                    {
                        creatingWeapon.ThisDamageTypes.Add(DamageType.armorNegating);
                        creatingWeapon.Damage /= 2;
                        cost += 12;
                        costMultiplier += 3;
                    }
                    else if (Controller.BoolFromRng(10 + constLevel * 5))
                    {
                        creatingWeapon.ThisDamageTypes.Add(DamageType.partialLifeDrain);
                        creatingWeapon.Damage -= 2;
                        cost += 6;
                        costMultiplier += 2;
                    }
                }
            }

            if (Controller.BoolFromRng(10 + constLevel * 5))
            {
                creatingWeapon.ThisDamageTypes.Add(DamageType.raiseDead);
                cost += 3;
            }
            else if (Controller.BoolFromRng(5 + constLevel * 5) && constLevel >= 2 && points > 5)
            {
                creatingWeapon.ThisDamageTypes.Add(DamageType.demon);
                cost += 5;
            }
            else if (Controller.BoolFromRng(5 + constLevel * 5) && constLevel >= 4  && points > 10)
            {
                creatingWeapon.ThisDamageTypes.Add(DamageType.holy);
                cost += 10;
            }
            else if (Controller.BoolFromRng(5 + constLevel * 5) && constLevel >= 4 && points > 10)
            {
                creatingWeapon.ThisDamageTypes.Add(DamageType.magic);
                cost += 10;
            }
            else if (Controller.BoolFromRng(5 + constLevel * 5) && constLevel >= 4 && points > 10)
            {
                creatingWeapon.ThisDamageTypes.Add(DamageType.large);
                cost += 10;
            }
            else if (Controller.BoolFromRng(5 + constLevel * 5) && constLevel >= 4 && points > 10)
            {
                creatingWeapon.ThisDamageTypes.Add(DamageType.small);
                cost += 10;
            }

            // secondary effect
            int bowSecondaryBoost = 0;
            if (creatingWeapon.ThisWeaponType is WeaponType.Ranged_Bow or WeaponType.Ranged_Crossbow
                or WeaponType.Ranged_Sling)
            {
                bowSecondaryBoost = 5;
                if (weaponMagicPaths.Contains(MagicPath.astral) || weaponMagicPaths.Contains(MagicPath.water))
                    creatingWeapon.UwOk = true;
            }
               
            // if rng -> get a secondary effect -> reduct points
            if (Controller.BoolFromRng(0 + constLevel * (15 + bowSecondaryBoost)))
            {
                int pointsLeftForSecondaryEffect = points - cost;
                List<SecondaryEffect> secondaryEffectsForItem = Controller.SecondaryEffectList.FindAll(x => x.Cost < pointsLeftForSecondaryEffect && x.ThisType.Contains(archeType) && (x.PathAffinityies.Contains(weaponMagicPaths[0]) || x.PathAffinityies.Contains(weaponMagicPaths[1])));

                if (secondaryEffectsForItem.Count > 0)
                {
                    int weightSum = secondaryEffectsForItem.Sum(x => x.Weight);
                    // Get random within sum function call
                    int attributeToPickWeight = Controller.Rng.Next(1, weightSum);
                    int checkingWeight = 0;
                    foreach (var secondary in secondaryEffectsForItem)
                    {
                        if (checkingWeight >= attributeToPickWeight)
                        {
                            // assign secondary effect to the item
                            creatingWeapon.ThisSecondaryEffect = new SecondaryEffect(secondary);
                            Controller.UsedSecondaryEffects.Add(creatingWeapon.ThisSecondaryEffect); 
                            cost += secondary.Cost;
                            break;
                        }
                        checkingWeight += secondary.Weight;
                    }
                }

                // more rng for tetiary effect TODO fix bad reuse of code
                if (Controller.BoolFromRng(0 + constLevel * (13 + bowSecondaryBoost)) && creatingWeapon.ThisSecondaryEffect != null)
                {
                    int pointsLeftForTetiaryEffect = points - cost;

                    // hier ook kost multiply
                    List<SecondaryEffect> tetiaryEffectsForItem = Controller.SecondaryEffectList.FindAll(x => x.Cost < pointsLeftForTetiaryEffect && x.Tertiary);

                    if (tetiaryEffectsForItem.Count > 0)
                    {
                        int weightSum = tetiaryEffectsForItem.Sum(x => x.Weight);
                        // Get random within sum function call
                        int attributeToPickWeight = Controller.Rng.Next(1, weightSum);
                        int checkingWeight = 0;
                        foreach (var tetiary in tetiaryEffectsForItem)
                        {
                            if (checkingWeight >= attributeToPickWeight)
                            {
                                // assign secondary effect to the item
                                creatingWeapon.ThisSecondaryEffect.ThisTetiaryEffect = new SecondaryEffect(tetiary);
                                Controller.UsedSecondaryEffects.Add(creatingWeapon.ThisSecondaryEffect.ThisTetiaryEffect);

                                if (creatingWeapon.ThisSecondaryEffect.Aoe > 0 && creatingWeapon.AOE > 0)
                                {
                                    cost += (int)(cost * creatingWeapon.AOE * creatingWeapon.ThisSecondaryEffect.Aoe);
                                }
                                else if (creatingWeapon.AOE > 0)
                                {
                                    cost += (int)(cost * creatingWeapon.AOE);
                                }
                                else if (creatingWeapon.ThisSecondaryEffect.Aoe > 0)
                                {
                                    cost += cost * creatingWeapon.ThisSecondaryEffect.Aoe;
                                }
                                else
                                {
                                    cost += tetiary.Cost;
                                }
                                break;
                            }
                            checkingWeight += tetiary.Weight;
                        }
                    }
                }
            }
            // end secondary

            UpgradeWeapon(points, constLevel, ref cost, creatingWeapon, costMultiplier);
            // return weapon and cost

            return creatingWeapon;
        }

        public static void UpgradeWeapon(int points, int constLevel, ref int cost, Weapon creatingWeapon, double costMultiplier)
        {
            double loopMultiplier = 1;
            // While point > 0 and RNG
            while (cost < points)
            {
                // upgrade secondary damage
                if (creatingWeapon.ThisSecondaryEffect != null && Controller.Rng.Next(1, 101) > 90 - (constLevel * 5) &&
                    cost + creatingWeapon.ThisSecondaryEffect.DamageScalingCost <= points &&
                    creatingWeapon.ThisSecondaryEffect.DamageScalingCost != 0)
                {
                    creatingWeapon.ThisSecondaryEffect.Damage++;
                    cost += creatingWeapon.ThisSecondaryEffect.DamageScalingCost;
                }
                // end
                else
                {
                    if (cost + (int) (8 * costMultiplier * loopMultiplier) <= points &&
                        Controller.Rng.Next(1, 101) > 95 - constLevel * 5)
                    {
                        if (creatingWeapon.NumberOfAttacks >= 5 && creatingWeapon.ThisWeaponType is not (WeaponType.Ranged_Crossbow or WeaponType.Ranged_Sling or WeaponType.Ranged_Bow)) continue;
                        if (creatingWeapon.NumberOfAttacks >= 100) continue;
                        creatingWeapon.NumberOfAttacks++;
                        cost += (int) (8 * costMultiplier * loopMultiplier);
                    }
                    else if (Controller.Rng.Next(1, 101) < 30 && creatingWeapon.AttackValue <= 24 && creatingWeapon.ThisWeaponType is not (WeaponType.Ranged_Crossbow or WeaponType.Ranged_Sling or WeaponType.Ranged_Bow))
                    {
                        creatingWeapon.AttackValue++;
                        cost += (int) (2 * costMultiplier * loopMultiplier);
                    }
                    else if (Controller.Rng.Next(1, 101) < 30 && creatingWeapon.AttackValue <= 100 && creatingWeapon.ThisWeaponType is (WeaponType.Ranged_Crossbow or WeaponType.Ranged_Sling or WeaponType.Ranged_Bow))
                    {
                        creatingWeapon.AttackValue++;
                        cost += (int)(costMultiplier * loopMultiplier);
                    }
                    else if (Controller.Rng.Next(1, 101) < 30 && creatingWeapon.DefenceValue <= 24)
                    {
                        creatingWeapon.DefenceValue++;
                        cost += (int) (2 * costMultiplier * loopMultiplier);
                    }
                    else
                    {
                        int change = 1;
                        creatingWeapon.Damage += change;
                        cost += (int) (change * loopMultiplier);
                    }
                }

                loopMultiplier += 0.05;
            }
        }

        // TODO use archeType
        private static WeaponType WeaponTypeGenerator(Type type, AttributeType archeType)
        {
            WeaponType creatingWeaponType;
            if (type == Type.one_handed)
            {
                creatingWeaponType = (WeaponType) Controller.Rng.Next(0, 6);
                if (archeType == AttributeType.Magic)
                {
                    creatingWeaponType = WeaponType.One_Handed_Staff;
                }
                else if (creatingWeaponType == WeaponType.One_Handed_Staff)
                {
                    creatingWeaponType = WeaponType.One_Handed_Axe;
                }
            }
            else if (type == Type.two_handed)
            {
                creatingWeaponType = (WeaponType) Controller.Rng.Next(6, 11);
                if (archeType == AttributeType.Magic)
                {
                    creatingWeaponType = WeaponType.Two_Handed_Staff;
                }
                else if(creatingWeaponType == WeaponType.Two_Handed_Staff)
                {
                    creatingWeaponType = WeaponType.Two_Handed_Pike;
                }
            }
            else // misile
            {
                creatingWeaponType = (WeaponType) Controller.Rng.Next(11, 14);
            }

            return creatingWeaponType;
        }


        public List<string> ToStringList()
        {
            List<string> stringList = new()
            {
                $"#newweapon {ThisId}",
                $"#name \"{Name}\"",
                $"#dmg {Damage}",
                "#magic"
            };

            

            if (NumberOfAttacks != 0)
            {
                stringList.Add($"#nratt {NumberOfAttacks}");
            }
            
            if (AttackValue != 0)
            {
                stringList.Add($"#att {AttackValue}"); // Attack / precision
            }

            // Natural attacks?

            if (ThisWeaponType is WeaponType.Ranged_Bow or WeaponType.Ranged_Crossbow)
            {
                stringList.Add($"#range {Range}");
                stringList.Add($"#ammo {Ammunition}");
                stringList.Add("#twohanded");
                stringList.Add("#bowstr");
                stringList.Add("#flysprite 109");
                if (UwOk)
                    stringList.Add("#uwok");
            }
            else if (ThisWeaponType is WeaponType.Ranged_Sling)
            {
                stringList.Add($"#range {Range}");
                stringList.Add($"#ammo {Ammunition}");
                stringList.Add("#bowstr");
                stringList.Add("#flysprite 111");
                stringList.Add("#sound 15");
                if (UwOk)
                    stringList.Add("#uwok");
            }
            else
            {
                if (DefenceValue != 0)
                    stringList.Add($"#def {DefenceValue}");
                if (Length > 0)
                    stringList.Add($"#len {Length}");
            }

            if (ThisWeaponType is WeaponType.Two_Handed_Axe or WeaponType.Two_Handed_Hammer or WeaponType.Two_Handed_Sword or WeaponType.Two_Handed_Staff or WeaponType.Two_Handed_Pike)
            {
                stringList.Add("#twohanded");
            }
            else if (ThisWeaponType is WeaponType.One_Handed_flail)
            {
                stringList.Add("#flail");
                stringList.Add("#sound 11");
            }

            if (ThisSecondaryEffect is {Always: true})
            {
                stringList.Add($"#secondaryeffectalways {ThisSecondaryEffect.ThisId}");
            }
            else if (ThisSecondaryEffect is {Always: false})
            {
                stringList.Add($"#secondaryeffect {ThisSecondaryEffect.ThisId}");
            }

            if (ThisWeaponType is WeaponType.Two_Handed_Pike or WeaponType.One_Handed_Spear)
            {
                stringList.Add("#charge");
                stringList.Add("#sound 12");
            }

            if (ThisWeaponType is WeaponType.Ranged_Bow)
            {
                stringList.Add("#sound 14");
            }
            else if (ThisWeaponType is WeaponType.Ranged_Crossbow)
            {
                stringList.Add("#sound 13");
            }

            if (AOE != 0)
            {
                stringList.Add($"#aoe {AOE}");
            }

            DamageTypesToInputedList(stringList, ThisDamageTypes);

            stringList.Add("#end");

            return stringList;
        }

        public static void DamageTypesToInputedList(List<string> stringList, List<DamageType> damageTypes)
        {
            foreach (DamageType thisDamageType in damageTypes)
            {
                switch (thisDamageType)
                {
                    case DamageType.blunt:
                        stringList.Add("#blunt");
                        break;

                    case DamageType.weakness:
                        stringList.Add("#dt_weakness");
                        break;

                    case DamageType.slash:
                        stringList.Add("#slash");
                        break;

                    case DamageType.piercing:
                        stringList.Add("#pierce");
                        break;

                    case DamageType.armorPiercing:
                        stringList.Add("#armorpiercing");
                        break;

                    case DamageType.armorNegating:
                        stringList.Add("#armornegating");
                        break;

                    case DamageType.partialLifeDrain:
                        stringList.Add("#dt_weapondrain");
                        break;

                    case DamageType.raiseDead:
                        stringList.Add("#dt_raise");
                        break;

                    case DamageType.shock:
                        stringList.Add("#shock");
                        stringList.Add("#nostr");
                        break;

                    case DamageType.fire:
                        stringList.Add("#fire");
                        stringList.Add("#nostr");
                        break;

                    case DamageType.cold:
                        stringList.Add("#cold");
                        stringList.Add("#nostr");
                        break;

                    case DamageType.acid:
                        stringList.Add("#acid");
                        stringList.Add("#nostr");
                        break;

                    case DamageType.poison:
                        stringList.Add("#poison");
                        stringList.Add("#dt_poison");
                        stringList.Add("#nostr");
                        break;
                    case DamageType.affliction:
                        stringList.Add("#dt_aff");
                        stringList.Add("#nostr");
                        break;
                    case DamageType.mrNegEasy:
                        stringList.Add("#mrnegateseasily");
                        break;
                    case DamageType.mrNeg:
                        stringList.Add("#mrnegates");
                        break;
                    case DamageType.mrNegHard:
                        stringList.Add("#hardmrneg");
                        break;
                    case DamageType.soulSlaying:
                        stringList.Add("#soulslaying");  
                        break;
                    case DamageType.fatigue:
                        stringList.Add("#dt_stun");
                        break;
                    case DamageType.stun:
                        stringList.Add("#dt_realstun");
                        break;
                    case DamageType.fatigueSize:
                        stringList.Add("#dt_sizestun");
                        break;
                    case DamageType.lifeDrain:
                        stringList.Add("#dt_drain");
                        break;
                    case DamageType.chainLightning:
                        break;
                    case DamageType.paralyze:
                        stringList.Add("#dt_paralyze");
                        break;
                    case DamageType.capped:
                        stringList.Add("#dt_cap");
                        break;
                    case DamageType.sizeresist:
                        stringList.Add("#sizeresist");
                        break;
                    case DamageType.mind:
                        stringList.Add("#mind");
                        break;
                    case DamageType.undeadImmume:
                        stringList.Add("#undeadimmune");
                        break;
                    case DamageType.inatimateImmume:
                        stringList.Add("#inanimateimmune");
                        break;
                    case DamageType.undeadOnly:
                        stringList.Add("#undeadonly");
                        break;
                    case DamageType.demonOnly:
                        stringList.Add("#demononly");
                        break;
                    case DamageType.sacredOnly:
                        stringList.Add("#sacredonly");
                        break;
                    case DamageType.friendlyImmume:
                        stringList.Add("#friendlyimmune");
                        break;
                    case DamageType.holy:
                        stringList.Add("#dt_holy");
                        break;
                    case DamageType.magic:
                        stringList.Add("#dt_magic");
                        break;
                    case DamageType.large:
                        stringList.Add("#dt_large");
                        break;
                    case DamageType.demon:
                        stringList.Add("#dt_demon");
                        break;
                    case DamageType.small:
                        stringList.Add("#dt_small");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public enum WeaponType
        {
            One_Handed_Sword,
            One_Handed_Dagger,
            One_Handed_flail,
            One_Handed_Axe,
            One_Handed_Staff,
            One_Handed_Spear,// 5
            Two_Handed_Sword,
            Two_Handed_Hammer,
            Two_Handed_Axe,
            Two_Handed_Staff,
            Two_Handed_Pike,// 10
            Ranged_Bow,
            Ranged_Crossbow,
            Ranged_Sling, // 13
        }

        public enum DamageType
        {
            blunt,
            slash,
            piercing,
            armorPiercing,
            armorNegating,
            partialLifeDrain,
            raiseDead,
            shock,
            fire,
            cold,
            acid,
            poison,
            affliction,
            mrNegEasy,
            mrNeg,
            mrNegHard,
            weakness,
            soulSlaying,
            fatigue,
            stun,
            fatigueSize,
            lifeDrain,
            chainLightning,
            paralyze,
            capped,
            sizeresist,
            mind,
            undeadImmume,
            inatimateImmume,
            undeadOnly,
            demonOnly,
            sacredOnly,
            friendlyImmume,
            holy,
            magic,
            large,
            demon,
            small
        }
    }
}