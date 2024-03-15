using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oauth0.Models
{
    public class Jwt
    {
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string nickname { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public string updated_at { get; set; }
        public string iss { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public long iat { get; set; }
        public double exp { get; set; }
        public string c_hash { get; set; }
        public string sid { get; set; }
        public string nonce { get; set; }
    }
}