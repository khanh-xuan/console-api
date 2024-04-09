using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.Models
{
    internal class AccessToken
    {
        public string Code { get; set; }
        public string Mesage { get; set; }
        public string Access_Token { get; set; }
    }
}
