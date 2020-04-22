using Microsoft.EntityFrameworkCore;
using PaymentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {}

        public DbSet<PaymentItem> payments { get; set; }
    }
}
