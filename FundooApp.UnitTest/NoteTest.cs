using FundooApp.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FundooApp.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Collections.Generic;
using FundooApp.Data.Infrastructure;
using Moq;
using System.Data.Entity;

namespace FundooApp.UnitTest
{
    [TestClass]
    public class NoteTest
    {
       
        [TestMethod]
        public  void Get()
        {
            NotesApiController notesController = new NotesApiController();
           
            List<tblNote> result =  notesController.GetNotes() ;
            
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task AddNote()
        {
            NotesApiController notesController = new NotesApiController();

            tblNote tblNote = new tblNote();
            tblNote.Title = "testnote";
            tblNote.Content = "this is my content";
            tblNote.IsPin = 0;
            tblNote.ColorCode = "black";
            tblNote.Mode = "1";
            tblNote.IsTrash = 1;
            tblNote.IsActive = 0;
           var result =await notesController.AddNote(tblNote);

            Assert.AreEqual(1, result);

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task UpdateNote()
        {
            NotesApiController notesController = new NotesApiController();

            tblNote tblNote = new tblNote();
            tblNote.ID = 9;
            tblNote.Title = "repos";
            tblNote.Content = "hello,hey";
            tblNote.IsPin = 0;
            tblNote.ColorCode = "white";
            tblNote.Mode = "2";
            tblNote.IsTrash = 0;
            tblNote.IsActive = 0;

            var result = await notesController.AddNote(tblNote);

            Assert.AreEqual(1,result);
        }

        [TestMethod]
        public async Task DeleteNote()
        {
            NotesApiController notesController = new NotesApiController();

            tblNote tblNote = new tblNote();
            tblNote.ID = 7;
           
            tblNote.Mode = "3";
            tblNote.IsDelete = 1;
           

            var result = await notesController.AddNote(tblNote);

            Assert.AreEqual(1,result);
        }
    }
}
