using System;
using System.Collections.Generic;
using System.Text;

namespace ApiKickstart.Domain
{
    public class PlayerCard
    {
        public Guid Id { get; set; }
        public CardTemplate Template { get; set; }

    }
}
