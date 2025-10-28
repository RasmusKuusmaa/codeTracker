using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTracker.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public LoginData Data { get; set; }
        public string Message { get; set; }
    }
}
