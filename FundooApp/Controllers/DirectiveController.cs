using FundooApp.Data.Models;
using FundooApp.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FundooApp.Controllers
{
    public class DirectiveController : Controller
    {
        // GET: Directive
        public ActionResult Header()
        {
            return View();
        }

        //GET: /Directive/SideNav
        [HttpGet]
        public async Task<ActionResult> SideNav()
        {
            var email = TempData["Email"].ToString();
            AccountsRepository accountsRepository = new AccountsRepository();
            ApplicationUser data=await accountsRepository.GetInfoAsync(email);
            TempData["ImageUrl"] = data.Picture;
            TempData.Keep();
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> SideNav(HttpPostedFileBase file)
        {

            if (file != null)
            {
                //byte[] imageData = null;
                var email = TempData["Email"].ToString();

                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/Images/" + ImageName);
                file.SaveAs(physicalPath);
                AccountsRepository accountsRepository = new AccountsRepository();
                await accountsRepository.RegisterUpdate(ImageName,email);
               

            }
            return RedirectToAction("GetNotes", "Notes");

        }
    }
}