using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    public class LocationUserModel : IModel
    {
        public int LocationID { get; set; } = -1;
        public int UserID { get; set; } = -1;
    }
}
