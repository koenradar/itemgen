using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace data
{
    public class Spell
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int School { get; set; }
        public int ResearchLevel { get; set; }
        public int Path1 { get; set; }
        public int PathLevel1 { get; set; }

        public int Path2 { get; set; }
        public int PathLevel2 { get; set; }
        public int Effect_Record_Id { get; set; }
        public int Effects_Count { get; set; }
        public int Precision { get; set; }
        public int FatigueCost { get; set; }
        public int GemCost { get; set; }
        public int Next_Spell { get; set; }

        public bool IsRitual { get; set; }

        public EffectSpell EffectSpell { get; set; }

        public Spell(int id, string name, int researchLevel, int path1, int pathLevel1, int path2, int pathLevel2, int fatigueCost, int effectRecordId, bool isRitual)
        {
            Id = id;
            Name = name;
            ResearchLevel = researchLevel;
            Path1 = path1;
            PathLevel1 = pathLevel1;
            Path2 = path2;
            PathLevel2 = pathLevel2;
            FatigueCost = fatigueCost;
            Effect_Record_Id = effectRecordId;
            IsRitual = isRitual;
        }
    }

    public class EffectSpell
    {
        public int Record_Id { get; set; }
        [JsonPropertyName("effect_number")]
        public int EffectNumber { get; set; }
        public int? Duration { get; set; }
        public int Ritual { get; set; }
        [JsonPropertyName("object_type")]
        public string ObjectType { get; set; }
        [JsonPropertyName("raw_argument")]
        public int RawArgument { get; set; }
        [JsonPropertyName("modifiers_mask")]
        public int ModifiersMask { get; set; }
        [JsonPropertyName("range_base")]
        public int RangeBase { get; set; }
        [JsonPropertyName("range_per_level")]
        public int RangePerLevel { get; set; }
        [JsonPropertyName("range_strength_divisor")]
        public int RangeStrengthDivisor { get; set; }
        [JsonPropertyName("area_base")]
        public int AreaBase { get; set; }
        [JsonPropertyName("area_per_level")]
        public int AreaPerLevel { get; set; }
        [JsonPropertyName("area_battlefield_pct")]
        public int? AreaBattlefieldPct { get; set; }
    }

    public class NationalSpell
    {
        public int ID { get; set; }

        public static List<NationalSpell> NationalSpellsList { get; set; } = new List<NationalSpell>();

        // includes communions slave
        static NationalSpell()
        {
            List<NationalSpell> tempList;
            string json = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + $"/input/NationalSpells.json");
            tempList = JsonConvert.DeserializeObject<List<NationalSpell>>(json,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }
            );
            NationalSpellsList = tempList;
        }
    }
}
