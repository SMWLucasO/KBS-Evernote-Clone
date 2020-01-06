using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.Notebooks.Notes.Labels;

namespace EvernoteCloneLibrary.Labels.NoteLabel
{
    public class NoteLabel : NoteLabelModel
    {
        /// <summary>
        /// Get all the NoteLabel records given the note (note)
        /// </summary>
        /// <param name="note">The note all corresponding labels should be retrieved from</param>
        /// <returns>A list containing NoteLabel records</returns>
        public static List<NoteLabelModel> GetAllNoteLabelsFromNote(Note note) =>
            GetAllNoteLabelsFromNote(note.Id);
        
        /// <summary>
        /// Get all the NoteLabel records given the note id(noteId)
        /// </summary>
        /// <param name="noteId">The id of the note all corresponding labels should be retrieved from</param>
        /// <returns>A list containing NoteLabel records</returns>
        public static List<NoteLabelModel> GetAllNoteLabelsFromNote(int noteId)
        {
            NoteLabelRepository noteLabelRepository = new NoteLabelRepository();
            return noteLabelRepository.GetBy(
                new[] { "NoteID = @NoteID" },
                new Dictionary<string, object>() { { "@NoteID", noteId } }
            ).Select((el) => ((NoteLabelModel)el)).ToList();
        }
        
        /// <summary>
        /// Get all the NoteLabel records given the Label
        /// </summary>
        /// <param name="label">The label that from which all notes should be returned</param>
        /// <returns>A list containing NoteLabel records</returns>
        public static List<NoteLabelModel> GetAllNoteLabelFromLabel(LabelModel label)
        {
            NoteLabelRepository noteLabelRepository = new NoteLabelRepository();
            return noteLabelRepository.GetBy(
                new[] { "LabelID = @LabelID" },
                new Dictionary<string, object>() { { "@LabelID", label.Id } }
            ).Select((el) => ((NoteLabelModel)el)).ToList();
        }

        /// <summary>
        /// Returns a NoteLabel record. This is used to check that a NoteLabel records does exist in the database
        /// </summary>
        /// <param name="note"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static NoteLabelModel GetNoteLabelFromLabelAndNote(Note note, LabelModel label) =>
            GetNoteLabelFromLabelAndNote(note.Id, label.Id);

        /// <summary>
        /// Returns a NoteLabel record. This is used to check that a NoteLabel records does exist in the database
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="labelId"></param>
        /// <returns></returns>
        public static NoteLabelModel GetNoteLabelFromLabelAndNote(int noteId, int labelId)
        {
            NoteLabelRepository noteLabelRepository = new NoteLabelRepository();
            List<NoteLabelModel> noteLabelModels = noteLabelRepository.GetBy(
                new[] { "LabelID = @LabelID", "NoteID = @NoteID" },
                new Dictionary<string, object>() { { "@LabelID", labelId }, { "@NoteID", noteId } }
            ).Select((el) => ((NoteLabelModel)el)).ToList();

            if (noteLabelModels.Count > 0)
            {
                return noteLabelModels[0];
            }

            return null;
        }

        /// <summary>
        /// Insert a new link between a note and a label
        /// </summary>
        /// <param name="noteLabel">The noteLabel to be inserted</param>
        /// <returns>A boolean indicating if the operation was successful (true) or not (false)</returns>
        public static bool AddNewNoteLabel(NoteLabel noteLabel) =>
            new NoteLabelRepository().Insert(noteLabel);

        /// <summary>
        /// Delete a NoteLabel record
        /// </summary>
        /// <param name="noteLabel">The NoteLabel that should be removed</param>
        /// <returns>A boolean indicating if the delete was successful</returns>
        public static bool RemoveNoteLabel(NoteLabelModel noteLabel) =>
            new NoteLabelRepository().Delete(noteLabel);
    }
}