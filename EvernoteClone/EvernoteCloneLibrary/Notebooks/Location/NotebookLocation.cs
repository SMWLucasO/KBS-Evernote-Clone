using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    public class NotebookLocation : NotebookLocationModel
    {

        private string _path;

        public override string Path
        {
            get
            {
                // Less code by calling the set method in the case where the path is null or empty.
                if (string.IsNullOrEmpty(_path))
                    Path = null;
                return _path;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _path = "/";
                } else
                {
                    _path = value;
                }
            } 
        }

    }
}
