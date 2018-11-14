using System;
using System.Collections.Generic;
using System.Linq;
using DeckOfCards.Domain;
using Bogus;
using Xunit;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace DeckOfCards.Test.Fixtures
{
    [CollectionDefinition("FakeData")]
    public class DataProviderCollection : ICollectionFixture<DataProviderFixture>
    {
        // This class has no code, and is never created. 
        // Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    public class DataProviderFixture : IDisposable //can also use IAsyncLifetime
    {
        public readonly Faker<CardTemplate> ValidCardProvider;
        public readonly Faker<CardTemplate> InvalidCardProvider;
        public readonly List<CardTemplate> CardTemplates;

        public DataProviderFixture()
        {
            // Instantiate providers and necessary validation/mocking rules
            ValidCardProvider = new Faker<CardTemplate>()
                .CustomInstantiator(f => CardTemplate.NewCard(
                    f.PickRandomParam(RanksEnumeration.List.ToArray()), 
                    f.PickRandomParam(SuitsEnumeration.List.ToArray())
                    ));

            CardTemplates = new List<CardTemplate>();
            foreach (var suit in SuitsEnumeration.List)
            {
                foreach (var rank in RanksEnumeration.List)
                {
                    var sourceCard = CardTemplate.NewCard(rank, suit);
                    sourceCard.CardName = "Unit test runtime value: " + rank + " of " + suit;
                    CardTemplates.Add(sourceCard);
                }
            }

            //.StrictMode(false) // can ensure all properties covered by rules
            //.RuleFor(o => o.Id, f => Guid.NewGuid().ToString())
            //.RuleFor(o => o.Suit, f => f.PickRandomParam(SuitsEnumeration.List.ToArray()))
            //.RuleFor(o => o.Rank, f => f.PickRandomParam(RanksEnumeration.List.ToArray()))
            //.RuleFor(o => o.CardName, (f, o) => f.Name.Random.Words(1) + " #" + DateTime.Now.Ticks.ToString());

            InvalidCardProvider = new Faker<CardTemplate>()
                .StrictMode(false) // can ensure all properties covered by rules
                .RuleFor(o => o.Suit, f => null)
                .RuleFor(o => o.Rank, f => null)
                .RuleFor(o => o.CardName, f => string.Empty);

        }

        /// <summary>
        /// Populate the datastore with the traditional 52 card deck templates with randomized descriptions and metadata.
        /// </summary>
        /// <returns></returns>
        public async Task SeedCardTemplates(IDocumentStore datastore)
        {
            using (var session = datastore.OpenAsyncSession())
            {
                foreach (var cardTemplate in CardTemplates)
                {
                    await session.StoreAsync(cardTemplate);
                }
                await session.SaveChangesAsync();
            }
        }

        public void Dispose()
        {

        }
    }
}
