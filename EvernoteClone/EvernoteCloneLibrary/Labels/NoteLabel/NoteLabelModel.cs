using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Labels.NoteLabel
{
    /// <summary>
    /// The class representation of the 'NoteLabel' table.
    /// </summary>
    public class NoteLabelModel : IModel
    {
        public int NoteId { get; set; } = -1;
        public int LabelId { get; set; } = -1;
    }
}