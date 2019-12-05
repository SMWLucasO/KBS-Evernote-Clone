using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    public class NotebookLocationModel : IModel
    {
        public int Id { get; set; } = -1;
        public virtual string Path { get; set; }
    }
}
