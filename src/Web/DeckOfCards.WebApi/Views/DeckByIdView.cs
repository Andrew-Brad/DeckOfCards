using System;
using System.Collections.Generic;

namespace DeckOfCards.WebApi.Views
{
    public class DeckByIdView
    {
        public int CardCount => this.Cards.Count;
        public List<NewDeckView.CardViewDto> Cards { get; set; }
    }
}
