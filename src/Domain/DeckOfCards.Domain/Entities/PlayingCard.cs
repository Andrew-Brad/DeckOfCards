using System;
using System.Collections.Generic;

namespace DeckOfCards.Domain
{
    /// <summary>
    /// A playing card represents a single card that belongs to 1 (and only 1) deck.  The deck is responsible for handling the consistency
    /// rules around around managing these cards (i.e. drawing, shuffling, etc.)
    /// A <see cref="PlayingCard"/> can only have one instance, and is globally unique on creation (can't be reused later in a different deck).
    /// </summary>
    public class PlayingCard
    {
        public string Id { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public CardTemplate Template { get; set; }
        public string CardTemplateId { get; set; } //{ get => Template.Id; } // shell property to enable related documents, poor DDD visuals
        public PlayingCard(string id, CardTemplate template)
        {
            this.Id = id;
            Template = template;
            //this.CardTemplateId = template?.Id; //  this has to be done to satisfy the serializer, makes for poor DDD visuals
        }
    }
}
