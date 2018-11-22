using System;
using System.Collections.Generic;

namespace DeckOfCards.CQRS
{
    /// <summary>
    /// Events are in the past tense, and immutable. They are point in time "facts" about 
    /// interactions or interesting occurences with the nouns in your domain.
    /// </summary>
    public interface IEvent
    {
        Guid BroadcastingApplicationId { get; set; }
        DateTime BroadcastDateTime { get; set; }
        string EventName { get; set; }
        int EventVersion { get; set; }
    }

    /// <summary>
    /// Experimental...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEvent<T> where T : IEvent
    {
        Guid BroadcastingApplicationId { get; set; }
        DateTime BroadcastDateTime { get; set; }
        string EventName { get; set; }
        int EventVersion { get; set; }

        // useful? belong here?
        //string Source { get; set; }
        // Probably won't keep this
        //Guid TriggeringApplicationId { get; set; }
        // Possibility for some cross cutting auth stuff
        //string Subject { get; set; }

    }
}
