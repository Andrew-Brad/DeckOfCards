using System;
using System.Collections.Generic;
using System.Linq;
using DeckOfCards.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeckOfCards.DataModel.JsonContractResolvers
{
    public class DeckConverter : JsonConverter<Deck>
    {
        public override Deck ReadJson(JsonReader reader, Type objectType, Deck existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            string debug = jObject.ToString();
            Guid id = Guid.Parse(jObject["@metadata"]["@id"].Value<string>());
            IList<PlayingCard> ravenCards = jObject["Cards"]
                .Children()
                .Select(x => new PlayingCard(Guid.Parse(x["Id"].Value<string>()),
                    CardTemplate.NewTemplate(RanksEnumeration.FromValue(x["Rank"].Value<ushort>()),
                        SuitsEnumeration.FromValue(x["Suit"].Value<ushort>())))).ToList();
            var serializedDeck = Deck.FromCards(ravenCards);
            return serializedDeck;
        }

        public override void WriteJson(JsonWriter writer, Deck value, JsonSerializer serializer)
        {
            IReadOnlyList<PlayingCard> cards = value.ShowCards();
            var ravenCards = cards.Select(x => new CardDto()
            {
                Id = x.Id.ToString(),
                Rank = x.Template.Rank,
                Suit = x.Template.Suit,
                CardTemplateId = string.Format("cards/{0}/{1}", x.Template.Rank.Name, x.Template.Suit.Name)
            });
            
            JObject obj = JObject.FromObject(new
            {
                Id = value.Id,
                Cards = ravenCards,                
                //Cards = cards.SelectMany(f => cards, () => new
                //{
                //    f.Template.Rank,
                //    f.Template.Suit
                //})
                //Cards = new[]
                //{
                //    Rank = cards.Select(x => x.Template.Rank),

                //}
                //Rank = value.ShowCards().Select(x=> x.Template.Rank),
                //Suit = value.ShowCards().Select(x => x.Template.Suit)
            });
            //string debug = obj.ToString();
            obj.WriteTo(writer);
        }

        // temp class until we can get mapping and perf worked out
        public class CardDto
        {
            public string Id { get; set; }
            public int Rank { get; set; }
            public int Suit { get; set; }
            public string CardTemplateId { get; set; }
        }
    }
}
