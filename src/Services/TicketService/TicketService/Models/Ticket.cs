﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketService.Models
{
    public class Ticket
    {
        public long UserId { get; set; }
        public long Id { get; set; }
        
        public string FilmName { get; set; }

        public UInt16 Price { get; set; }
    }
}
