using System;
using System.Collections.Generic;
using System.Linq;
using DeckOfCards.Domain;
using DeckOfCards.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeckOfCards.DataModel.JsonContractResolvers
{
    /// <summary>
    /// The Converter is responsible for mapping domain objects to and from the underlying database.
    /// In alot of ways, it is a subset of the DbContext/Repository and should be focused on mapping and recreating the entity.
    /// </summary>
    public class DeckConverter : JsonConverter<Deck>
    {
        public override Deck ReadJson(JsonReader reader, Type objectType, Deck existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            string debug = jObject.ToString();
            Guid id = Guid.Parse(jObject["@metadata"]["@id"].Value<string>());
            // Manual LINQ token conversion, but could also reuse DTO's from below
            IList<PlayingCard> ravenCards = jObject["Cards"]
                .Children()
                .Select(x => new PlayingCard(x["Id"].Value<string>(),
                    CardTemplate.NewTemplate(RanksEnumeration.FromValue(x["Rank"].Value<ushort>()),
                        SuitsEnumeration.FromValue(x["Suit"].Value<ushort>())))).ToList();
            var serializedDeck = Deck.FromCards(ravenCards);
            return serializedDeck;
        }

        /// <summary>
        /// This routine essentially replaces the manual Dto mapping in the handlers.  Database storage details can be managed here.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, Deck value, JsonSerializer serializer)
        {
            IReadOnlyList<PlayingCard> cards = value.ShowCards();
            var cardDtos = cards.Select(x => new CardDto()
            {
                Id = x.Id.ToString(),
                Rank = x.Template.Rank,
                Suit = x.Template.Suit,
                CardTemplateId = string.Format(IdConventions.CardTemplateIdFormatString, x.Template.Rank.Name, x.Template.Suit.Name) // todo: reuse the existing Func
                //CardTemplateId = x.CardTemplateId
            }).ToArray();

            var deckDto = new DeckDto()
            {
                Id = value.Id,
                Cards = cardDtos,
            };

            JObject obj = JObject.FromObject(deckDto);
            string debug = obj.ToString();
            obj.WriteTo(writer);
        }

        // temp class until we can get mapping and perf worked out
        protected class CardDto
        {
            public string Id { get; set; }
            public int Rank { get; set; }
            public int Suit { get; set; }
            public string CardTemplateId { get; set; }
        }

        protected class DeckDto
        {
            public string Id { get; set; }
            public CardDto[] Cards;
        }
    }
}
