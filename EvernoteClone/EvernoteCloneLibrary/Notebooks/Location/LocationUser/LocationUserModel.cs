using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    public class LocationUserModel : IModel
    {
        public int LocationId { get; set; } = -1;
        public int UserId { get; set; } = -1;
    }
}
