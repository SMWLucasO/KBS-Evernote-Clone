using System;
using System.Collections.Generic;
using System.Linq;
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
                // Less code by calling the set method in the case where the path is null or consists out of white spaces.
                if (string.IsNullOrWhiteSpace(_path))
                    Path = null;
                return _path;
            }
            set => _path = string.IsNullOrEmpty(value) ? "/" : value;
        }

        // TODO: add summary
        private static NotebookLocation GetNotebookLocationById(int id,
            NotebookLocationRepository notebookLocationRepository)
        {
            List<NotebookLocation> notebookLocations = notebookLocationRepository.GetBy(
                new[] {"Id = @Id"},
                new Dictionary<string, object>() {{"@Id", id}}
            ).Select(el => (NotebookLocation) el).ToList();

            return notebookLocations.Count > 0 ? notebookLocations.First() : null;
        }

        public static NotebookLocation GetNotebookLocationById(int id, int userId)
        {
            List<NotebookLocation> notebookLocations = Load(userId);
            
            foreach (NotebookLocation notebookLocation in notebookLocations)
                if (notebookLocation.Id == id)
                    return notebookLocation;

            return null;
        }

        public bool Delete() =>
            Delete(new NotebookLocationRepository());
        
        public bool Delete(NotebookLocationRepository notebookLocationRepository)
        {
            bool removedCloud = notebookLocationRepository.Delete(this);
            bool removedLocally = RemoveNotebookLocationFromLocalStorage(this);
            
            return removedCloud || removedLocally;
        }

        public static string GetNotebookLocationPathById(int id, int userId) =>
            GetNotebookLocationById(id, userId)?.Path;

        public static NotebookLocation GetNotebookLocationByPath(string path, int userId)
        {
            List<NotebookLocation> notebookLocations = Load(userId);
            
            foreach (NotebookLocation notebookLocation in notebookLocations)
                if (notebookLocation.Path == path)
                    return notebookLocation;

            return null;
        }

        public static bool AddNewNotebookLocation(NotebookLocation notebookLocation, int userId)
        {
            bool addedToDatabase = AddNewNotebookLocationToDatabaseAndGetId(notebookLocation, userId) != -1;
            bool addedLocally = AddNotebookLocationToLocalStorage(notebookLocation);

            return addedToDatabase || addedLocally;
        }

        public static int AddNewNotebookLocationAndGetId(NotebookLocation notebookLocation, int userId)
        {
            AddNewNotebookLocationToDatabaseAndGetId(notebookLocation, userId);
            AddNotebookLocationToLocalStorage(notebookLocation);

            return notebookLocation.Id;
        }

        public static int AddNewNotebookLocationToDatabaseAndGetId(NotebookLocation notebookLocation, int userId)
        {
            if (userId != -1)
            {
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                if (notebookLocationRepository.Insert(notebookLocation))
                    LocationUser.LocationUser.AddNewLocationUser(new LocationUser.LocationUser
                        {LocationId = notebookLocation.Id, UserId = userId});
            }
            return notebookLocation.Id;
        }

        public static List<NotebookLocation> GetAllNotebookLocationsFromDatabase(int userId)
        {
            // Load all LocationUser records from UserID
            List<LocationUser.LocationUser> locationUserRecords = LocationUser.LocationUser.GetAllLocationsFromUser(userId);

            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            List<NotebookLocation> notebookLocationsFromDatabase = new List<NotebookLocation>();
            foreach (LocationUser.LocationUser locationUser in locationUserRecords)
                notebookLocationsFromDatabase.Add(GetNotebookLocationById(locationUser.LocationId, notebookLocationRepository));
            return notebookLocationsFromDatabase;
        }

        public static bool AddNotebookLocationToLocalStorage(NotebookLocation notebookLocation) =>
            XmlExporter.Export(GetUserDataStoragePath(), "NotebookLocations.enex",
                GetNewXmlRepresentation(notebookLocation));

        public static bool RemoveNotebookLocationFromLocalStorage(NotebookLocation notebookLocation) =>
            XmlExporter.Export(GetUserDataStoragePath(), "NotebookLocations.enex",
                GetXmlRepresentationWithout(notebookLocation));

        private static string[] GetNewXmlRepresentation(NotebookLocation notebookLocation) => 
            GetXmlRepresentation(XmlImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex"), notebookLocation);
        
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
                {
                    xmlRepresentation.AddRange(nbLocation.ToXmlRepresentation());
                }
            }

            if (newNotebookLocation != null) 
                xmlRepresentation.AddRange(newNotebookLocation.ToXmlRepresentation());
            xmlRepresentation.Add("</en-export>");

            return xmlRepresentation.ToArray();
        }

        private static string[] GetXmlRepresentationWithout(NotebookLocation notebookLocation)
        {
            List<NotebookLocation> notebookLocations =
                XmlImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex");
                
            if (notebookLocations.Any(location => location.Path == notebookLocation.Path))
            {
                notebookLocations.Remove(notebookLocations.First(location => location.Path == notebookLocation.Path));
            }

            return GetXmlRepresentation(notebookLocations);
        }

        public static List<NotebookLocation> Load(int userId = -1)
        {
            List<NotebookLocation> notebookLocations = new List<NotebookLocation>();
            
            // Load all the notebook locations stored in the local storage
            List<NotebookLocation> notebookLocationsFromFileSystem = XmlImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex");
            
            // Load all the notebook locations stored in the database, if the user has a proper ID.
            // Note: Should also verify using password hash, but that is a TODO. This part will be rewritten later on.
            if (userId != -1)
            {
                List<NotebookLocation> notebookLocationsFromDatabase = GetAllNotebookLocationsFromDatabase(userId);
                if (notebookLocationsFromFileSystem != null && notebookLocationsFromDatabase != null)
                {
                    // Check if db and fs id's are identical, if so, add. If not but paths are, update fs id.
                    foreach (NotebookLocation dbNotebookLocation in notebookLocationsFromDatabase)
                        foreach (NotebookLocation fsNotebookLocation in notebookLocationsFromFileSystem)
                            LoadNotebookLocation(fsNotebookLocation, dbNotebookLocation, notebookLocations);
                    foreach (NotebookLocation fsNotebookLocation in notebookLocationsFromFileSystem) // TODO check if second foreach is redundant
                        foreach (NotebookLocation dbNotebookLocation in notebookLocationsFromDatabase)
                            LoadNotebookLocation(fsNotebookLocation, dbNotebookLocation, notebookLocations);
                }

                if (notebookLocationsFromFileSystem != null)
                {
                    // If path from fs is not in db, add to db (and update local id)
                    foreach (NotebookLocation notebookLocation in notebookLocationsFromFileSystem.Where(notebookLocation => !notebookLocations.Contains(notebookLocation)))
                        notebookLocations.AddIfNotPresent(notebookLocation.Update(notebookLocation.Id, AddNewNotebookLocationToDatabaseAndGetId(notebookLocation, userId)));
                }
                if (notebookLocationsFromDatabase != null)
                {
                    // If path from db is not in fs, add to fs
                    foreach (NotebookLocation notebookLocation in notebookLocationsFromDatabase.Where(notebookLocation => !notebookLocations.Contains(notebookLocation)))
                        notebookLocations.AddIfNotPresent(notebookLocation.AddLocally());
                }
            }
            else if (notebookLocationsFromFileSystem != null)
                notebookLocations.AddRange(notebookLocationsFromFileSystem);
            return notebookLocations;
        }

        private NotebookLocation AddLocally() =>
            AddNotebookLocationToLocalStorage(this) ? this : null;

        private static void LoadNotebookLocation(NotebookLocation fsNotebookLocation, NotebookLocation dbNotebookLocation, List<NotebookLocation> listToAddTo)
        {
            // If they are both the same Id, we load the last updated one.
            if (dbNotebookLocation.Id == fsNotebookLocation.Id)
                listToAddTo.AddIfNotPresent(fsNotebookLocation);
            else if (dbNotebookLocation.Path == fsNotebookLocation.Path)
                listToAddTo.AddIfNotPresent(fsNotebookLocation.Update(fsNotebookLocation.Id, dbNotebookLocation.Id));
        }

        private NotebookLocation Update(int oldId, int newId)
        {
            List<NotebookLocation> notebookLocations = XmlImporter.ImportNotebookLocations(GetUserDataStoragePath() + @"/NotebookLocations.enex");
            notebookLocations.First(notebookLocation => notebookLocation.Id == oldId).Id = newId;
            
            Id = newId;
            
            return XmlExporter.Export(GetUserDataStoragePath(), "NotebookLocations.enex", GetXmlRepresentation(notebookLocations)) ? this : null;
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

        public override bool Equals(object obj)
        {
            if (obj is NotebookLocation compareTo)
                return Id == compareTo.Id && Path == compareTo.Path;
            return false;
        }

        public bool Equals(NotebookLocation other) =>
            Id == other.Id && Path == other.Path;

        public override int GetHashCode() =>
            (Path != null ? Path.GetHashCode() : 0);

        private static string GetUserDataStoragePath()
        {
            string path = Constant.TEST_MODE ? Constant.TEST_USERDATA_STORAGE_PATH : Constant.PRODUCTION_USERDATA_STORAGE_PATH;
            string[] splittedPath = path.Split('<', '>');

            if (splittedPath.Length == 3)
            {
                splittedPath[1] = Constant.User.Username;

                return splittedPath[0] + splittedPath[1] + splittedPath[2];
            }

            return null;
        }
    }
}
