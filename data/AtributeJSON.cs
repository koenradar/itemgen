using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace data
{
    public class AtributeJSON
    {
        private static string JsonLocation { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory + $"/input/AtributeList.txt";

        public static List<Atribute> ReadAtributeList()
        {
            List<Atribute> tempList;
            string json = File.ReadAllText(JsonLocation);
            tempList = JsonConvert.DeserializeObject<List<Atribute>>(json,
                                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }
                );
            return tempList;
        }

        public static void WriteJSON()
        {
            string json = JsonConvert.SerializeObject(Atribute.AllAtributes.ToArray(), Formatting.Indented);
            if (File.Exists(JsonLocation))
            {
                using StreamWriter file = File.CreateText(JsonLocation);
                file.Write(json);
            }
            else
            {
                if (Controller.DebugLogging1)
                {
                    Debug.WriteLine("Error Writing JSON");
                }
            }
        }
    }
}
