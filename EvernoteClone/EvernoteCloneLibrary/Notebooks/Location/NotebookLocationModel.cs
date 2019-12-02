using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    public class NotebookLocationModel : IModel
    {
        public int Id { get; set; } = -1;
        public virtual string Path { get; set; }
    }
}
