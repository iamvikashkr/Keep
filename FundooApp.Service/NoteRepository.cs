using FundooApp.Data.Infrastructure;
using FundooApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooApp.Service
{
    public class NotesRepository : INoteRepository
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();


        public List<tblNote> GetNotes(string userid)
        {
            var list = new List<tblNote>();
            try
            {
                //list = dbContext.tblNotes.OrderBy(a => a.ID).ToList<tblNote>();
                tblNote tblNote = new tblNote();
                var data = from a in dbContext.tblNotes
                           where a.UserID == userid
                           select a;
                foreach (tblNote item in data)
                {
                    list.Add(item);
                }

                return list;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return list;
        }



        public async Task<int> AddNote(tblNote model)
        {
            int i = 0;
            try
            {
                dbContext.tblNotes.Add(model);
                i = await dbContext.SaveChangesAsync();
                return i;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return i;
        }

        public async Task<int> UpdateNote(tblNote model)
        {
            int i = 0;

            try
            {
                tblNote tbl = dbContext.tblNotes.Where<tblNote>(a => a.ID == model.ID).First();
                tbl.Title = model.Title;
                tbl.Content = model.Content;
                tbl.UserID = model.UserID;
                tbl.ColorCode = model.ColorCode;
                tbl.IsTrash = model.IsTrash;
                tbl.IsPin = model.IsPin;
                tbl.IsDelete = model.IsDelete;
                tbl.IsArchive = model.IsArchive;
                tbl.Reminder = model.Reminder;
                tbl.ImageUrl = model.ImageUrl;
                //tbl.Reminder = Convert.ToString(DateTime.Now.ToShortTimeString());
                //tbl.Reminder = Convert.ToString(DateTime.Now.ToString("MMMM"))+", "+ Convert.ToString(DateTime.Now.ToShortTimeString());
                tbl.UserID = model.UserID;
                i = await dbContext.SaveChangesAsync();
                return i;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return i;
        }

        public async Task<int> DeleteNote(tblNote model)
        {
            int i = 0;
            try
            {
                tblNote tbl = dbContext.tblNotes.Where<tblNote>(a => a.ID == model.ID).First();
                dbContext.tblNotes.Remove(tbl);
                i = await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return i;

        }
    }
}
