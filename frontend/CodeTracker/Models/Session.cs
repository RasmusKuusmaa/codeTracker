 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTracker.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime? TimeStarted { get; set; }
        public DateTime? TimeEnded { get; set; }
        public string Title { get; set; }
    }

    public class SessionResponse
    {
        public bool Success { get; set; }
        public List<Session> Data { get; set; }
        public string Message { get; set; }
    }
}
