using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
    public class Sprite
    {
        public string FilePath { get; set; }

        public static List<Sprite> AllSprites { get; set; } = new List<Sprite>();

        public int UsedAmount = 0;

        public Type SpriteType { get; set; }

        public Sprite(string filePath, Type spriteType)
        {
            FilePath = filePath;
            SpriteType = spriteType;
        }

        public Sprite()
        {

        }

        static Sprite()
        {
            var directories = Directory.EnumerateDirectories(System.AppDomain.CurrentDomain.BaseDirectory + "/Sprites").ToList();
            foreach (var directory in directories)
            {
                string type = directory.Split('\\').Last();

                var sprites = Directory.EnumerateFiles(directory);

                foreach (var sprite in sprites)
                {
                    // hard coded and bad
                    AllSprites.Add(new Sprite(sprite.Split("\\")[^2] + "/" + sprite.Split("\\")[^1], Enum.Parse<Type>(type.ToLower())));
                }
            }
        }
    }
}
