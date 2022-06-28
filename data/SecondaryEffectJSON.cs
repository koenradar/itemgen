using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
    public class SecondaryEffectJSON
    {
        private static string JsonLocation { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory + $"/input/SecondaryEffectList.txt";

        public static List<SecondaryEffect> ReadSecondaryEffectList()
        {
            List<SecondaryEffect> tempList;
            string json = File.ReadAllText(JsonLocation);
            tempList = JsonConvert.DeserializeObject<List<SecondaryEffect>>(json,
                                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }
                );
            return tempList;
        }
    }
}
