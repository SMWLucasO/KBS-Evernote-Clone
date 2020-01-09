using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Labels
{
    /// <summary>
    /// The class representation of the 'LabelModel' table.
    /// </summary>
    public class LabelModel : IModel
    {
        public int Id { get; set; } = -1;
        public virtual string Title { get; set; }

    }
}