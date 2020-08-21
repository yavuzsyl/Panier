using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Options
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
    }
}
