using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketService.Models
{
    public class TicketInput
    {
        public string Token { get; set; }

        public string FilmName { get; set; }

        public UInt16 Price { get; set; }
    }
}
