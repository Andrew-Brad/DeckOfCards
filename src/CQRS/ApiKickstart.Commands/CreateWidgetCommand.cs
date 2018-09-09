using System;
using MediatR;
using ApiKickstart.CommandResults;
using ApiKickstart.CQRS;
using ApiKickstart.Domain;

namespace ApiKickstart.Commands
{
    /// <summary>
    /// Represents a logical business command to create a new widget.
    /// </summary>
    public class CreateWidgetCommand : ICommand, IRequest<CreateWidgetCommandResult>
    {
        public string WidgetName { get; set; }
        public string Description { get; set; }
        public int? SupplierId { get; set; }
        public bool Deprecated { get; set; } = false;//future use case for a breaking api/migration change - move to some domain enumeration
        public SuitsEnumeration Classification { get; set; }// will map "1" or "RawMaterial" to internal enum

    }
}
