﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Owin;

namespace FundooApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var url = Request.Url;

            //return RedirectToAction("GetNotes", "Notes");..

            return View();
        }

        public ActionResult Bar()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SideNav()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Redirect()
        {
            return View();
        }


    }
}
