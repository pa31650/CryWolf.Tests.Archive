using System;
using System.Collections.Generic;
using System.Text;

namespace API.Models.v1
{
    public partial class Auth : CryWolf
    {
        public string jurisdiction { get; set; }
        public string userName { get; set; }
        public string token { get; set; }
        public double tokenExpiresInMinutes { get; set; }
        public string refreshToken { get; set; }
    }
    

}
