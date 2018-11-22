using System;
using DeckOfCards.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeckOfCards.DataModel.JsonContractResolvers
{
    // https://www.newtonsoft.com/json/help/html/N_Newtonsoft_Json_Converters.htm
    public class PlayingCardConverter : JsonConverter<PlayingCard>
    {
        public override PlayingCard ReadJson(JsonReader reader, Type objectType, PlayingCard existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            PlayingCard card = obj.ToObject<PlayingCard>();
            return card;
        }

        public override void WriteJson(JsonWriter writer, PlayingCard value, JsonSerializer serializer)
        {
            JObject obj = JObject.FromObject(value);
            obj.Property("Template").Remove();
            obj.WriteTo(writer);
        }
    }
}
