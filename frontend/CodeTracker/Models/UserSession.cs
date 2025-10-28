using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeTracker.Models
{
    public static class UserSession
    {
        public static User CurrentUser { get; set; }

        public static readonly string  BaseUrl = "http://localhost:5020/api/";

    }
}
