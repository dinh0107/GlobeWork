using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    public class LinkedInProfileResponseController : Controller
    {
        public class LinkedInTokenResponse
        {
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
        }
    }
}