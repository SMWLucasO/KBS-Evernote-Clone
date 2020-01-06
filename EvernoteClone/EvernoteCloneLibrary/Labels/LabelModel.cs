using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes.Labels
{
    public class LabelModel : IModel
    {
        public int Id { get; set; } = -1;
        public virtual string Title { get; set; }

    }
}