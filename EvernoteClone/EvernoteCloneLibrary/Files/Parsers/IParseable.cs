using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Files.Parsers
{
    public interface IParseable
    {
        public string[] ToXMLRepresentation();
    }
}
