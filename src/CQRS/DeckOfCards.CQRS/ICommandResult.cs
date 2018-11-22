using System;
using System.Collections.Generic;
using System.Text;

namespace DeckOfCards.CQRS
{
    /// <summary>
    /// Marker interface for object that are the result of a command.
    /// </summary>
    public interface ICommandResult
    {
        CommandResultStatus ResultStatus { get; set; }
    }
    public enum CommandResultStatus
    {
        //todo: flush out some more meaningful failure modes 
        NotYetProcessed = 0,
        SuccessfullyProcessed = 1,
        ServiceUnavailable = 2,
        CriticalError = 3
    }
}
