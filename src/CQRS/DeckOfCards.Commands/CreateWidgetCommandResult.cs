using ApiKickstart.CQRS;
using ApiKickstart.Domain;
using System;

namespace ApiKickstart.CommandResults
{
    public class CreateWidgetCommandResult : CommandResultBase
    {
        public CardTemplate Widget { get; set; }
    }
}
