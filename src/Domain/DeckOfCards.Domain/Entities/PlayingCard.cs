﻿using System;
using System.Collections.Generic;

namespace DeckOfCards.Domain
{
    /// <summary>
    /// A playing card represent a single card that belongs to 1 (and only 1) deck.  The deck is responsible for handling the consistency
    /// rules around around managing these cards (i.e. drawing, shuffling, etc.)
    /// A <see cref="PlayingCard"/> can only have one instance, and is globally unique on creation (can't be reused later in a different deck).
    /// </summary>
    public class PlayingCard
    {
        public Guid Id { get; set; }
        //public DateTimeOffset CreationDate { get; set; }
        public CardTemplate Template { get; set; }
    }
}