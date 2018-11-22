using System;
using System.Collections.Generic;

namespace DeckOfCards.WebApi.Views
{
    public class NewDeckView
    {
        public int CardCount => this.Cards.Count;
        public List<CardViewDto> Cards { get; set; }
        public class CardViewDto
        {
            public string Id { get; set; }
            public string Suit { get; set; }
            public string Rank { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
        }

    }
}
