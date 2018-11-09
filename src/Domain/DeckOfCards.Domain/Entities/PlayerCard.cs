using System;
using System.Collections.Generic;
using System.Text;

namespace DeckOfCards.Domain
{
    public class PlayerCard
    {
        public Guid Id { get; set; }
        public CardTemplate Template { get; set; }

    }
}
