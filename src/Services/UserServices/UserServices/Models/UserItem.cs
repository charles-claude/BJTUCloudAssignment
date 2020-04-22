using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserServices.Models
{
    public class UserItem
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TokenId { get; set; }
    }
}
