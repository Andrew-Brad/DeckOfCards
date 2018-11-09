using ApiKickstart.CQRS;

namespace ApiKickstart.CommandResults
{
    public class CommandResultBase : ICommandResult
    {
        public CommandResultStatus ResultStatus { get; set; }
    }
}
