using MediatR;
using System;

namespace DeckOfCards.CommandResults
{
    public class DeprecateWidgetCommandResult : CommandResultBase
    {
        public int WidgetId { get; set; }
        public bool Success { get; set; }
    }    
}
