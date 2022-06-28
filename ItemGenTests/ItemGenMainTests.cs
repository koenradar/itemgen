using NUnit.Framework;
using data;

namespace ItemGenTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateOneItem()
        {
            Item item = Item.GenerateItem();

            Assert.IsNotNull(item);
            Assert.IsTrue(item.AtributeList.Count > 0);
        }

        [Test]
        public void Generate500Items()
        {
            for (int i = 0; i < 500; i++)
            {
                GenerateOneItem();
            }
        }

        [Test]
        public void Generate50000Items()
        {
            for (int i = 0; i < 50000; i++)
            {
                GenerateOneItem();
            }
        }

        [Test]
        public void GenerateWeapon()
        {
            int cost;
            Weapon weapon = Weapon.GenerateWeapon(50, Type.two_handed, 4, AttributeType.Thugging, new MagicPath[2]
                {
                    MagicPath.air,
                    MagicPath.astral
                }, out cost);

            Assert.NotZero(cost);
            Assert.IsNotNull(weapon);
            Assert.IsTrue(weapon.ThisWeaponType.ToString().Contains("Two_Handed"));
        }

        [Test]
        public void Generate50000Weapons()
        {
            GenerateWeapon();
        }

        [Test]
        public void GenerateArmor()
        {
            int cost;
            Armor armor = Armor.GenerateArmor(50, Type.armor, out cost);

            Assert.NotZero(cost);
            Assert.IsNotNull(armor);
            Assert.AreEqual(Type.armor, armor.ThisType);
        }

        [Test]
        public void Generate50000Armors()
        {
            GenerateArmor();
        }
    }
}