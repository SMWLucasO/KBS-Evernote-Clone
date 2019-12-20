using EvernoteCloneLibrary.Labels.NoteLabel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes.Labels
{
    public class Label : LabelModel
    {

        private string _title;

        public override string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value.Length >= 2 && value.Length <= 64)
                {
                    _title = value;
                }
            }
        }

        public List<LabelModel> GetLabels()
        {
            List<LabelModel> labelOutput = new List<LabelModel>();
            LabelRepository labelRepository = new LabelRepository();
         
            return labelRepository.GetBy(null, null)
                .Select((el) => ((LabelModel)el)).ToList();

        }

        public LabelModel GetLabel(int id)
        {
            LabelRepository labelRepository = new LabelRepository();
            List<LabelModel> labelModels = labelRepository.GetBy(
                    new[] { "Id = @Id" },
                    new Dictionary<string, object>() { { "@Id", id } }
            ).Select((el) => ((LabelModel)el)).ToList();
        
            if (labelModels.Count > 0)
            {
                return labelModels[0];
            }
            
            return null;
        }

        public bool InsertLabel(LabelModel labelModel, Note note)
        {
            LabelRepository labelRepository = new LabelRepository();
            bool inserted = labelRepository.Insert(labelModel);

            if (inserted)
            {
                NoteLabel noteLabel = new NoteLabel();
                noteLabel.NoteId = note.Id;
                noteLabel.LabelId = labelModel.Id;
                inserted = NoteLabel.AddNewNoteLabel(noteLabel);
            }

            return inserted;

        }

        /// <summary>
        /// Since a label 'cant' be removed, return false
        /// </summary>
        /// <param name="labelModel"></param>
        /// <returns></returns>
        public bool DeleteLabel(LabelModel labelModel) =>
            false;
    }
}
