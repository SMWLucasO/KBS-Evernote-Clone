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
                if(value.Length >= 2 && value.Length <= 64)
                {
                    _title = value;
                }
            }
        }


    }
}
