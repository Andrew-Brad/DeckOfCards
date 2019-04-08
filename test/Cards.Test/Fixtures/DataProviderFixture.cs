using System;
using System.Collections.Generic;
using System.Linq;
using DeckOfCards.Domain;
using Bogus;
using Xunit;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Bogus.DataSets;

namespace DeckOfCards.Test.Fixtures
{
    [CollectionDefinition(TestConstants.DataProviderCollection)]
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
        public readonly Deck Standard52CardDeck;
        public readonly Deck ZeroCardDeck;

        public DataProviderFixture()
        {
            // Instantiate providers and necessary validation/mocking rules
            ValidCardProvider = new Faker<CardTemplate>()
                .CustomInstantiator(f => CardTemplate.NewTemplate(
                    f.PickRandomParam(RanksEnumeration.List.ToArray()),
                    f.PickRandomParam(SuitsEnumeration.List.ToArray())
                    ))
                .RuleFor(o => o.ImageUrl, f => f.Image.LoremPixelUrl(LoremPixelCategory.Cats));

            //.StrictMode(false) // can ensure all properties covered by rules

            CardTemplates = new List<CardTemplate>();
            foreach (var suit in SuitsEnumeration.List)
            {
                foreach (var rank in RanksEnumeration.List)
                {
                    var template = CardTemplate.NewTemplate(rank, suit);
                    template.CardName = "Unit test runtime value: " + rank.Name + " of " + suit.Name;
                    template.ImageUrl = ValidCardProvider.Generate().ImageUrl;
                    CardTemplates.Add(template);
                }
            }

            Standard52CardDeck = Deck.Standard52CardDeck(CardTemplates);
            ZeroCardDeck = Deck.FromCards(null);

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

        /// <summary>
        /// Populate the datastore with a traditional 52 card deck.
        /// </summary>
        /// <returns></returns>
        public async Task SeedValidDeck(IDocumentStore datastore)
        {
            using (var session = datastore.OpenAsyncSession())
            {
                await session.StoreAsync(Standard52CardDeck);
                await session.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Populate the datastore with a 0 card deck.
        /// </summary>
        /// <returns></returns>
        public async Task SeedZeroCardDeck(IDocumentStore datastore)
        {
            using (var session = datastore.OpenAsyncSession())
            {
                await session.StoreAsync(ZeroCardDeck);
                await session.SaveChangesAsync();
            }
        }

        public void Dispose()
        {

        }
    }
}
