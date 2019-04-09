using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    public class Auth0Client : Auth0ClientBase
    {
        public Auth0Client(Auth0ClientOptions options)
            : base(options, "wpf")
        {
        }
    }
}
