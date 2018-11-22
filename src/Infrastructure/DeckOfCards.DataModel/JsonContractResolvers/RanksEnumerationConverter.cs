using System;
using DeckOfCards.Domain;
using Ardalis.SmartEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeckOfCards.DataModel.JsonContractResolvers
{
    // https://www.newtonsoft.com/json/help/html/N_Newtonsoft_Json_Converters.htm
    public class RanksEnumerationConverter : JsonConverter<SmartEnum<RanksEnumeration, ushort>>
    {
        public override SmartEnum<RanksEnumeration, ushort> ReadJson(JsonReader reader, Type objectType, SmartEnum<RanksEnumeration, ushort> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            ushort enumValue = Convert.ToUInt16(reader.Value);
            return RanksEnumeration.FromValue(enumValue);
        }

        public override void WriteJson(JsonWriter writer, SmartEnum<RanksEnumeration, ushort> value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value.Value);
            token.WriteTo(writer);
        }
    }
}
