 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.Models;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentsController : ControllerBase
    {
        private readonly PaymentContext _context;
        public PaymentsController(PaymentContext context)
        {
            _context = context;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentItem>>> Getpayments()
        {
            return await _context.payments.ToListAsync();
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentItem>> GetPaymentItem(long id)
        {
            var paymentItem = await _context.payments.FindAsync(id);

            if (paymentItem == null)
            {
                return NotFound();
            }

            return paymentItem;
        }

        // PUT: api/Payments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentItem(long id, PaymentItem paymentItem)
        {
            if (id != paymentItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(paymentItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentItemExists(id))
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

        // POST: api/Payments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PaymentItem>> PostPaymentItem(PaymentItem paymentItem)
        {
            _context.payments.Add(paymentItem);
            await _context.SaveChangesAsync();
            Sender.Send("Payment", "12_13");
            return CreatedAtAction(nameof(GetPaymentItem), new { id = paymentItem.Id }, paymentItem);
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PaymentItem>> DeletePaymentItem(long id)
        {
            var paymentItem = await _context.payments.FindAsync(id);
            if (paymentItem == null)
            {
                return NotFound();
            }

            _context.payments.Remove(paymentItem);
            await _context.SaveChangesAsync();

            return paymentItem;
        }

        private bool PaymentItemExists(long id)
        {
            return _context.payments.Any(e => e.Id == id);
        }
    }
}
