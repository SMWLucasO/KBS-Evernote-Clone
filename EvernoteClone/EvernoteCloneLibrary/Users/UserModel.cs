using EvernoteCloneLibrary.Database;
using System;

namespace EvernoteCloneLibrary.Users
{
    /// <summary>
    /// The class(OOP) representation of the 'User' table.
    /// </summary>
    public class UserModel : IModel
    {
        public int Id { get; set; } = -1;
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsGoogleAccount { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
