using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    /// <summary>
    /// The class representation of the 'NotebookLocation' table.
    /// </summary>
    public class NotebookLocationModel : IModel
    {
        public int Id { get; set; } = -1;
        public virtual string Path { get; set; }
    }
}
