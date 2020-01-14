using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyut_autoWrite
{
    public class LoginModel
    {
        public string account { get; set; }
        public string password { get; set; }
        public string verifyCode { get; set; }
        public bool isLogin { get; set; } = false;
    }
}
