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

        // TODO: add summary
        public static NotebookLocation GetNotebookLocationById(int Id) => 
            GetNotebookLocationById(Id, new NotebookLocationRepository());
        public static NotebookLocation GetNotebookLocationById(int Id, NotebookLocationRepository notebookLocationRepository)
        {
            return notebookLocationRepository.GetBy(
                    new string[] { "Id = @Id" },
                    new Dictionary<string, object>() { { "@Id", Id } }
                ).Select((el) => ((NotebookLocation)el)).ToList()[0];
        }

        public static string GetNotebookLocationPathById(int Id) =>
            GetNotebookLocationPathById(Id, new NotebookLocationRepository());
        public static string GetNotebookLocationPathById(int Id, NotebookLocationRepository notebookLocationRepository)
        {
            return notebookLocationRepository.GetBy(
                new string[] { "Id = @Id" },
                new Dictionary<string, object>() { { "@Id", Id } }
            ).Select((el) => ((NotebookLocation)el)).ToList()[0].Path;
        }

        public static NotebookLocation GetNotebookLocationByPath(string Path)
        {
            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            return notebookLocationRepository.GetBy(
                new[] { "Path = @Path" },
                new Dictionary<string, object> { { "@Path", Path } }
            ).Select(notebookLocation => ((NotebookLocation)notebookLocation)).ToList()[0];
        }

        public static bool AddNewNotebookLocation(NotebookLocation notebookLocation, int UserID)
        {
            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            if (notebookLocationRepository.Insert(notebookLocation))
                return LocationUser.LocationUser.AddNewLocationUser(new LocationUser.LocationUser() { LocationID = notebookLocation.Id, UserID = UserID});
            return false;
        }
    }
}
