using System;
using System.Collections.Generic;

namespace DeckOfCards.WebApi.Views
{
    public class NewDeckView
    {
        public int CardCount => this.Cards.Count;
        public List<CardDto> Cards { get; set; }
        public class CardDto
        {
            public string Id { get; set; }
            public string Suit { get; set; }
            public string Rank { get; set; }
            public string Name { get; set; }
        }

    }
}
