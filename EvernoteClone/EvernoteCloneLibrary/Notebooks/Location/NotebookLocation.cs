using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Extensions;
using EvernoteCloneLibrary.Files.Parsers;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    public class NotebookLocation : NotebookLocationModel, IParseable
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

        public static bool AddNewNotebookLocation(NotebookLocation NotebookLocation, int UserID)
        {
            bool addedToCloud = false;
            if (UserID != -1)
            {
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                if (notebookLocationRepository.Insert(NotebookLocation))
                    addedToCloud = LocationUser.LocationUser.AddNewLocationUser(new LocationUser.LocationUser()
                        {LocationID = NotebookLocation.Id, UserID = UserID});
            }

            return AddNotebookLocationToLocalStorage(NotebookLocation) || addedToCloud;
        }

        public static int AddNewNotebookLocationAndGetId(NotebookLocation NotebookLocation, int UserID)
        {
            if (UserID != -1)
                AddNewNotebookLocationToDatabaseAndGetId(NotebookLocation, UserID);
            AddNotebookLocationToLocalStorage(NotebookLocation);

            return NotebookLocation.Id;
        }

        public static int AddNewNotebookLocationToDatabaseAndGetId(NotebookLocation NotebookLocation, int UserID)
        {
            if (UserID != -1)
            {
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                if (notebookLocationRepository.Insert(NotebookLocation))
                    LocationUser.LocationUser.AddNewLocationUser(new LocationUser.LocationUser
                        {LocationID = NotebookLocation.Id, UserID = UserID});
            }
            return NotebookLocation.Id;
        }

        public static List<NotebookLocation> GetAllNotebookLocationsFromDatabase(int UserID)
        {
            // Load all LocationUser records from UserID
            List<LocationUser.LocationUser> locationUserRecords = LocationUser.LocationUser.GetAllLocationsFromUser(UserID);

            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            List<NotebookLocation> notebookLocationsFromDatabase = new List<NotebookLocation>();
            foreach (LocationUser.LocationUser locationUser in locationUserRecords)
                notebookLocationsFromDatabase.Add(GetNotebookLocationById(locationUser.LocationID, notebookLocationRepository));
            return notebookLocationsFromDatabase;
        }

        public static bool AddNotebookLocationToLocalStorage(NotebookLocation notebookLocation) =>
            XMLExporter.Export(GetUserDataStoragePath(), "NotebookLocations.enex", GetNewXmlRepresentation(notebookLocation));

        private static string[] GetNewXmlRepresentation(NotebookLocation notebookLocation) => 
            GetXmlRepresentation(XMLImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex"), notebookLocation);
        private static string[] GetXmlRepresentation(List<NotebookLocation> notebookLocations, NotebookLocation newNotebookLocation = null)
        {
            List<string> xmlRepresentation = new List<string>()
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                $"<en-export export-date=\"{DateTime.Now:yyyyMMdd}T{DateTime.Now:HHmmss}Z\"",
                " application=\"EvernoteClone/Windows\" version=\"6.x\">"
            };

            if (notebookLocations != null)
            {
                foreach (NotebookLocation nbLocation in notebookLocations)
                    xmlRepresentation.AddRange(nbLocation.ToXmlRepresentation());
            }

            if (newNotebookLocation != null) 
                xmlRepresentation.AddRange(newNotebookLocation.ToXmlRepresentation());

            xmlRepresentation.Add("</en-export>");

            return xmlRepresentation.ToArray();
        }

        public static List<NotebookLocation> Load(int UserID = -1)
        {
            List<NotebookLocation> notebookLocations = new List<NotebookLocation>();
            
            // Load all the notebook locations stored in the local storage
            List<NotebookLocation> notebookLocationsFromFileSystem = XMLImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex");
            
            // Load all the notebook locations stored in the database, if the user has a proper ID.
            // Note: Should also verify using password hash, but that is a TODO. This part will be rewritten later on.
            if (UserID != -1)
            {
                List<NotebookLocation> notebookLocationsFromDatabase = GetAllNotebookLocationsFromDatabase(UserID);
                if (notebookLocationsFromFileSystem != null && notebookLocationsFromDatabase != null)
                {
                    // Check if db and fs id's are identical, if so, add. If not but paths are, update fs id.
                    foreach (NotebookLocation dbNotebookLocation in notebookLocationsFromDatabase)
                        foreach (NotebookLocation fsNotebookLocation in notebookLocationsFromFileSystem)
                            LoadNotebookLocation(fsNotebookLocation, dbNotebookLocation, notebookLocations);
                    foreach (NotebookLocation fsNotebookLocation in notebookLocationsFromFileSystem)
                        foreach (NotebookLocation dbNotebookLocation in notebookLocationsFromDatabase)
                            LoadNotebookLocation(fsNotebookLocation, dbNotebookLocation, notebookLocations);
                }

                if (notebookLocationsFromFileSystem != null)
                {
                    // If path from fs is not in db, add to db (and update local id)
                    foreach (NotebookLocation notebookLocation in notebookLocationsFromFileSystem.Where(notebookLocation => !notebookLocations.Contains(notebookLocation)))
                        notebookLocations.AddIfNotPresent(notebookLocation.Update(notebookLocation.Id, AddNewNotebookLocationToDatabaseAndGetId(notebookLocation, UserID)));
                }
                if (notebookLocationsFromDatabase != null)
                {
                    // If path from db is not in fs, add to fs
                    foreach (NotebookLocation notebookLocation in notebookLocationsFromDatabase.Where(notebookLocation => !notebookLocations.Contains(notebookLocation)))
                        notebookLocations.AddIfNotPresent(notebookLocation.AddLocally());
                }
            }
            else
                notebookLocations.AddRange(notebookLocationsFromFileSystem);
            return notebookLocations;
        }

        private NotebookLocation AddLocally()
        {
            AddNotebookLocationToLocalStorage(this);
            return this;
        }

        private static void LoadNotebookLocation(NotebookLocation FSNotebookLocation, NotebookLocation DBNotebookLocation, List<NotebookLocation> ListToAddTo)
        {
            // If they are both the same Id, we load the last updated one.
            if (DBNotebookLocation.Id == FSNotebookLocation.Id)
                ListToAddTo.AddIfNotPresent(FSNotebookLocation);
            else if (DBNotebookLocation.Path == FSNotebookLocation.Path)
                ListToAddTo.AddIfNotPresent(FSNotebookLocation.Update(FSNotebookLocation.Id, DBNotebookLocation.Id));
        }

        private NotebookLocation Update(int OldID, int NewID)
        {
            List<NotebookLocation> notebookLocations = XMLImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex");
            notebookLocations.First(notebookLocation => notebookLocation.Id == OldID).Id = NewID;
            Id = NewID;
            XMLExporter.Export(GetUserDataStoragePath(), "NotebookLocations.enex", GetXmlRepresentation(notebookLocations));
            return this;
        }

        public string[] ToXmlRepresentation()
        {
            return new[] {
                "<location>",
                    $"<id>{Id}</id>",
                    $"<path>{Path}</path>",
                "</location>"
            };
        }

        public override bool Equals(object Obj)
        {
            if (Obj is NotebookLocation compareTo)
                return Id == compareTo.Id && Path == compareTo.Path;
            return false;
        }

        /// <summary>
        /// Get the storage path for saving notes and notebooks locally.
        /// </summary>
        /// <returns></returns>
        private static string GetNotebookStoragePath() =>
            Constant.TEST_MODE ? Constant.TEST_NOTEBOOK_STORAGE_PATH : Constant.PRODUCTION_NOTEBOOK_STORAGE_PATH;

        private static string GetUserDataStoragePath() => 
            Constant.TEST_MODE ? Constant.TEST_USERDATA_STORAGE_PATH : Constant.PRODUCTION_USERDATA_STORAGE_PATH;
    }
}
