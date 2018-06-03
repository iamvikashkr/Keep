using FundooApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooApp.Service
{
    public interface INoteRepository
    {
        List<tblNote> GetNotes(string userid);       
        Task<int> AddNote(tblNote model);
        Task<int> UpdateNote(tblNote model);     
        Task<int> DeleteNote(tblNote model);
       
        

    }
}
