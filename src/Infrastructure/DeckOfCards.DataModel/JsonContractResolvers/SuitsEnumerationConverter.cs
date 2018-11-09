using System;
using DeckOfCards.Domain;
using Ardalis.SmartEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeckOfCards.DataModel.JsonContractResolvers
{
    // https://www.newtonsoft.com/json/help/html/N_Newtonsoft_Json_Converters.htm
    public class SuitsEnumerationConverter : JsonConverter<SmartEnum<SuitsEnumeration, ushort>>
    {
        public override SmartEnum<SuitsEnumeration, ushort> ReadJson(JsonReader reader, Type objectType, SmartEnum<SuitsEnumeration, ushort> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            ushort enumValue = Convert.ToUInt16(reader.Value);
            return SuitsEnumeration.FromValue(enumValue);
        }

        public override void WriteJson(JsonWriter writer, SmartEnum<SuitsEnumeration, ushort> value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value.Value);
            token.WriteTo(writer);
        }
    }
}
