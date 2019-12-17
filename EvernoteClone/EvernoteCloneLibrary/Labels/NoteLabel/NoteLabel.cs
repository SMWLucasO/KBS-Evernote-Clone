using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Notebooks.Notes;

namespace EvernoteCloneLibrary.Labels.NoteLabel
{
    public class NoteLabel : NoteLabelModel
    {
        /// <summary>
        /// Get all the NoteLabel records given the note (note)
        /// </summary>
        /// <param name="note">The note all corresponding labels should be retrieved from</param>
        /// <returns>A list containing NoteLabel records</returns>
        public static List<NoteLabel> GetAllNoteLabelsFromNote(Note note) =>
            GetAllNoteLabelsFromNote(note.Id);
        
        /// <summary>
        /// Get all the NoteLabel records given the note id(noteId)
        /// </summary>
        /// <param name="noteId">The id of the note all corresponding labels should be retrieved from</param>
        /// <returns>A list containing NoteLabel records</returns>
        public static List<NoteLabel> GetAllNoteLabelsFromNote(int noteId)
        {
            NoteLabelRepository noteLabelRepository = new NoteLabelRepository();
            return noteLabelRepository.GetBy(
                new[] { "NoteID = @NoteID" },
                new Dictionary<string, object>() { { "@NoteID", noteId } }
            ).Select((el) => ((NoteLabel)el)).ToList();
        }
        
        /// <summary>
        /// Get all the NoteLabel records given the Label (labelTitle)
        /// </summary>
        /// <param name="labelId">The id of the label all notes that are returned should have</param>
        /// <returns>A list containing NoteLabel records</returns>
        public static List<NoteLabel> GetAllNoteLabelsFromLabel(int labelId) // TODO change this with Label.Id when branches are merged
        {
            NoteLabelRepository noteLabelRepository = new NoteLabelRepository();
            return noteLabelRepository.GetBy(
                new[] { "LabelID = @LabelID" },
                new Dictionary<string, object>() { { "@LabelID", labelId } }
            ).Select((el) => ((NoteLabel)el)).ToList();
        }

        /// <summary>
        /// Insert a new link between a note and a label
        /// </summary>
        /// <param name="noteLabel">The noteLabel to be inserted</param>
        /// <returns>A boolean indicating if the operation was successful (true) or not (false)</returns>
        public static bool AddNewNoteLabel(NoteLabel noteLabel) =>
            new NoteLabelRepository().Insert(noteLabel);
    }
}