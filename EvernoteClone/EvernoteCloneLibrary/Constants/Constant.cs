using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// All constants.
    /// </summary>
    public static class Constant
    {
        // A determinant for whether we are in the test mode.
        public const bool TEST_MODE = true;

        // Storage constants (TEST mode)
        public const string TEST_CONNECTION_STRING = "Data Source=.;Initial Catalog=NoteFever_EvernoteClone;Integrated Security=True";
        public const string TEST_STORAGE_PATH = "tests/local/";

        // Storage constants (PRODUCTION mode)
        public const string PRODUCTION_CONNECTION_STRING = "Data Source=tcp:145.44.234.54;Initial Catalog=NoteFever_EvernoteClone;Integrated Security=True";
        public const string PRODUCTION_STORAGE_PATH = "";

    }
}
