﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketService.Models;
using TicketService.Utils;

namespace TicketService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly TicketContext _context;

        public TicketsController(TicketContext context)
        {
            _context = context;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicket()
        {
            return await _context.Ticket.ToListAsync();
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(long id)
        {
            var ticket = await _context.Ticket.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        // PUT: api/Tickets/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(long id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(TicketInput ticketinput)
        {
            string Data;
            Sender.Send("Client", ticketinput.Token);
            System.Threading.Thread.Sleep(1000);
            Data = Receiver.Receive("Ticket");
            if (Data == "Unknown")
                return (Unauthorized());
            Ticket ticket = new Ticket();
            ticket.FilmName = ticketinput.FilmName;
            ticket.UserId = long.Parse(Data);
            ticket.Price = ticketinput.Price;
            _context.Ticket.Add(ticket);

            Sender.Send("Payment", ticket.Id.ToString() + "_" + ticket.UserId.ToString());
            System.Threading.Thread.Sleep(1000);
            string PaymentResponse = Receiver.Receive("Ticket");
            if (PaymentResponse != "OK")
                return (Unauthorized());
            else
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
            }
            
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ticket>> DeleteTicket(long id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        private bool TicketExists(long id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
