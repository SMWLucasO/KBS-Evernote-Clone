using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    /// <summary>
    /// The class representation of the 'LocationUser' table.
    /// </summary>
    public class LocationUserModel : IModel
    {
        public int LocationId { get; set; } = -1;
        public int UserId { get; set; } = -1;
    }
}
