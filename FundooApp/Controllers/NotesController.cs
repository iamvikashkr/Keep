using FundooApp.Data;
using FundooApp.Data.Log;
using FundooApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FundooApp.Controllers
{

    [Authorize]
    public class NotesController : Controller
    {
        AccountController accountController = new AccountController();
        static List<tblNote> list = new List<tblNote>();
        static string userid;
        static string token;

        //GET: Notes/GetNotes
        [HttpGet]
        public async Task<ActionResult> GetNotes()
        {
            try
            {

               token = TempData["access_token"].ToString();
               userid = TempData["UserID"].ToString();
                TempData.Keep();
                list = await accountController.ConsumeApi("", token, userid);

                return View(list);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
                return RedirectToAction("Login", "Account");
            }


        }

        //POST: Notes/GetNotes
        [HttpPost]
        public async Task<ActionResult> GetNotes(tblNote model)
        {
            var i = 0;

            try
            {
                //string token = TempData["access_token"].ToString();
                //string userid = TempData["UserID"].ToString();
                //TempData.Keep();
                model.UserID = userid;
                if (ModelState.IsValid)
                {
                    i = await accountController.ConsumePostApi(model, token);
                    return View();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
            }
            return View(i);

        }

        //GET: Notes/Trash
        [HttpGet]
        public async Task<ActionResult> Trash()
        {
            try
            {
                string token = TempData["access_token"].ToString();
                string userid = TempData["UserID"].ToString();
                TempData.Keep();
                list = await accountController.ConsumeApi("", token, userid);

                return View(list);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());

                return RedirectToAction("Login", "Account");
            }
        }

        //GET: Notes/Archive
        [HttpGet]
        public async Task<ActionResult> Archive()
        {
            try
            {
                string token = TempData["access_token"].ToString();
                string userid = TempData["UserID"].ToString();
                TempData.Keep();
                list = await accountController.ConsumeApi("", token, userid);

                return View(list);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());

                return RedirectToAction("Login", "Account");
            }

        }

        //GET: Notes/Reminder
        [HttpGet]
        public async Task<ActionResult> Reminder()
        {
            try
            {
                string token = TempData["access_token"].ToString();
                string userid = TempData["UserID"].ToString();
                TempData.Keep();
                list = await accountController.ConsumeApi("", token, userid);

                return View(list);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());

                return RedirectToAction("Login", "Account");
            }

        }


        [HttpGet]
        public ActionResult GetNoteExternal(string userId, string Token)
        {
            try
            {
                   token = Token;
                   userid = userId;
                TempData["access_token"] = Token;
                TempData["UserID"] = userId;
                TempData.Keep();
                NotesApiController noteApiController = new NotesApiController();
                list = noteApiController.GetNotes(userId);

                return View("GetNotes", list);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());

                return RedirectToAction("Login", "Account");
            }


        }

        [HttpGet]
        public ActionResult PopUp()
        {
            TempData.Keep();

            return View();
        }


        [HttpGet]
        public ActionResult UpdateImage()
        {
            TempData.Keep();

            return View();
        }

        [HttpGet]
        public ActionResult NoteImage( )
        {
            //var list = new List<tblNote>();

            //list.Add(model);
            TempData.Keep();


            return View();
        }


        [HttpGet]
        public async Task<ActionResult> List()
        {
            try
            {
                string token = TempData["access_token"].ToString();
                string userid = TempData["UserID"].ToString();
                TempData.Keep();
                list = await accountController.ConsumeApi("", token, userid);

                return View(list);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
                return RedirectToAction("Login", "Account");
            }
        }
    }
}