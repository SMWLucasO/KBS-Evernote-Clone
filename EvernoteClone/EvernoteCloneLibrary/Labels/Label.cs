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

        public LabelModel GetLabel(string title)
        {
            LabelRepository labelRepository = new LabelRepository();
            List<LabelModel> labelModels = labelRepository.GetBy(
                    new[] { "Title = @Title" },
                    new Dictionary<string, object>() { { "@Title", title } }
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
            bool inserted = false;

            LabelModel existingLabelModel = GetLabel(labelModel.Title);

            if (existingLabelModel == null)
            {
                inserted = labelRepository.Insert(labelModel);
            }

            NoteLabel noteLabel = new NoteLabel();
            noteLabel.NoteId = note.Id;
            noteLabel.LabelId = existingLabelModel == null ? labelModel.Id : existingLabelModel.Id;
            inserted = NoteLabel.AddNewNoteLabel(noteLabel);

            return inserted;
        }
    }
}
