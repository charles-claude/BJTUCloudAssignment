using Microsoft.EntityFrameworkCore;
using UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService
{
    public class UserContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {}

        public DbSet<UserItem> payments { get; set; }
    }
}
