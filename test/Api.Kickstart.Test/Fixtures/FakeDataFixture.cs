using System;
using System.Collections.Generic;
using System.Linq;
using DeckOfCards.Domain;
using Bogus;
using Xunit;

namespace DeckOfCards.Test.Fixtures
{
    [CollectionDefinition("FakeData")]
    public class FakeDataCollection : ICollectionFixture<FakeDataFixture>
    {
        // This class has no code, and is never created. 
        // Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    public class FakeDataFixture : IDisposable //can also use IAsyncLifetime
    {
        public readonly Faker<CardTemplate> ValidCardProvider;
        public readonly Faker<CardTemplate> InvalidCardProvider;
        public readonly List<CardTemplate> SourceCardProvider;

        public FakeDataFixture()
        {
            // Generate Fake data for this test suite

            ValidCardProvider = new Faker<CardTemplate>()
                .CustomInstantiator(f => CardTemplate.NewCard(
                    f.PickRandomParam(RanksEnumeration.List.ToArray()), 
                    f.PickRandomParam(SuitsEnumeration.List.ToArray())
                    ));

            SourceCardProvider = new List<CardTemplate>();
            foreach (var suit in SuitsEnumeration.List)
            {
                foreach (var rank in RanksEnumeration.List)
                {
                    var sourceCard = CardTemplate.NewCard(rank, suit);
                    sourceCard.CardName = "Unit test runtime value: " + rank + " of " + suit;
                    SourceCardProvider.Add(sourceCard);
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

        public void Dispose()
        {

        }
    }
}
