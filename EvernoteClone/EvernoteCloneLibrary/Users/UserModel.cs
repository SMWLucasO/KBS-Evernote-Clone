using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Users
{
    public class UserModel : IModel
    {
        public int Id { get; set; } = -1;
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsGoogleAccount { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
