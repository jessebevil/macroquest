using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGAuth.Models;

namespace RGAuth.Code
{
    public class AuthorizationInfo
    {
        public User user { get; set; }
        public AuthToken token { get; set; }
        public string apiKey { get; set; }
        public string errInfo { get; set; }
    }
}
