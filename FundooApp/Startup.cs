using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FundooApp.Data;
[assembly: OwinStartup(typeof(FundooApp.Data.Startup))]

namespace FundooApp
{
    public partial class Startup
    {
       
    }
}