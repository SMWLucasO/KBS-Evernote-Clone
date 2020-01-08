using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Notebooks.Notes;

namespace EvernoteCloneLibrary.Labels
{
    /// <summary>
    /// This class manages all logic for Labels
    /// </summary>
    public class Label : LabelModel
    {
        #region Variables
        
        /// <value>
        /// This contains the title of the label
        /// </value>
        private string _title;

        #endregion
        
        #region Properties
        
        /// <value>
        /// This properties is used to get and set the Label's title.
        /// </value>
        public override string Title
        {
            get => _title;
            set
            {
                if (value.Length >= 2 && value.Length <= 64)
                {
                    _title = value;
                }
            }
        }
        
        #endregion

        /// <summary>
        /// This method returns all labels in a list
        /// </summary>
        /// <returns></returns>
        public static List<LabelModel> GetLabels()
        {
            LabelRepository labelRepository = new LabelRepository();
         
            return labelRepository.GetBy(null, null)
                .Select(el => el).ToList();
        }

        /// <summary>
        /// This returns one label that is retrieved by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LabelModel GetLabel(int id)
        {
            LabelRepository labelRepository = new LabelRepository();
            List<LabelModel> labelModels = labelRepository.GetBy(
                    new[] { "Id = @Id" },
                    new Dictionary<string, object> { { "@Id", id } }
            ).Select(el => el).ToList();
        
            if (labelModels.Count > 0)
            {
                return labelModels[0];
            }
            
            return null;
        }

        /// <summary>
        /// This returns one label that is retrieved by its title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static LabelModel GetLabel(string title)
        {
            LabelRepository labelRepository = new LabelRepository();
            List<LabelModel> labelModels = labelRepository.GetBy(
                    new[] { "Title = @Title" },
                    new Dictionary<string, object> { { "@Title", title } }
            ).Select(el => el).ToList();

            if (labelModels.Count > 0)
            {
                return labelModels[0];
            }

            return null;
        }

        /// <summary>
        /// This method inserts a new label into the database.
        /// It also adds a link between the note and the label
        /// </summary>
        /// <param name="labelModel">The label that should be inserted into the database</param>
        /// <param name="note">The note to which this label is linked</param>
        /// <returns></returns>
        public static bool InsertLabel(LabelModel labelModel, Note note)
        {
            bool inserted = InsertLabel(labelModel);

            if (inserted)
            {
                NoteLabel.NoteLabel noteLabel = new NoteLabel.NoteLabel
                {
                    NoteId = note.Id,
                    LabelId = labelModel.Id
                };
                inserted = NoteLabel.NoteLabel.AddNewNoteLabel(noteLabel);
            }

            return inserted;
        }

        /// <summary>
        /// This inserts a LabelModel into the database.
        /// If it already exists, it returns true (cause it already exists) and sets the labelModel id equal to the one already in the database
        /// </summary>
        /// <param name="labelModel"></param>
        /// <returns></returns>
        public static bool InsertLabel(LabelModel labelModel)
        {
            LabelRepository labelRepository = new LabelRepository();

            LabelModel existingLabelModel = GetLabel(labelModel.Title);

            if (existingLabelModel == null)
            {
                return labelRepository.Insert(labelModel);
            }

            labelModel.Id = existingLabelModel.Id;
            
            return true;
        }
    }
}
