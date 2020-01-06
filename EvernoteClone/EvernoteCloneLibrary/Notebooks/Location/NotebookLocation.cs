using System;
using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Extensions;
using EvernoteCloneLibrary.Files.Parsers;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    /// <summary>
    /// This class handles the logic for all NotebookLocations
    /// </summary>
    public class NotebookLocation : NotebookLocationModel, IParseable
    {
        #region Variables
        
        /// <value>
        /// This contains the path
        /// </value>
        private string _path;
        
        #endregion

        #region Properties
        
        /// <value>
        /// This sets or gets the path
        /// </value>
        public override string Path
        {
            get
            {
                // Less code by calling the set method in the case where the path is null or consists out of white spaces.
                if (string.IsNullOrWhiteSpace(_path))
                    Path = null;
                return _path;
            }
            set => _path = string.IsNullOrWhiteSpace(value) ? "/" : value;
        }
        
        #endregion

        #region Logic specific for Database interaction
        
        /// <summary>
        /// Returns a NotebookLocation by a given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notebookLocationRepository"></param>
        /// <returns></returns>
        private static NotebookLocation GetNotebookLocationById(int id,
            NotebookLocationRepository notebookLocationRepository)
        {
            List<NotebookLocation> notebookLocations = notebookLocationRepository.GetBy(
                new[] {"Id = @Id"},
                new Dictionary<string, object>() {{"@Id", id}}
            ).Select(el => (NotebookLocation) el).ToList();

            return notebookLocations.Count > 0 ? notebookLocations.First() : null;
        }
        
        /// <summary>
        /// Deletes this NotebookLocation from the database
        /// </summary>
        /// <returns></returns>
        public bool DeleteFromDatabase() =>
            DeleteFromDatabase(new NotebookLocationRepository());
        
        /// <summary>
        /// Deletes this NotebookLocation from the database
        /// </summary>
        /// <param name="notebookLocationRepository"></param>
        /// <returns></returns>
        public bool DeleteFromDatabase(NotebookLocationRepository notebookLocationRepository)
        {
            if (Constant.User.Id != -1)
            {
                if (LocationUser.LocationUser.DeleteLocationUser(
                    LocationUser.LocationUser.GetLocationUserByNotebookLocation(this)))
                {
                    return notebookLocationRepository.Delete(this);
                }
            }

            return false;
        }
        
        /// <summary>
        /// Add a new NotebookLocation to the Database and return the Id that is returned from the Database
        /// </summary>
        /// <param name="notebookLocation"></param>
        /// <returns></returns>
        public static int AddNewNotebookLocationToDatabaseAndGetId(NotebookLocation notebookLocation)
        {
            int userId = Constant.User.Id;
            
            if (userId != -1)
            {
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                if (notebookLocationRepository.Insert(notebookLocation))
                    LocationUser.LocationUser.AddNewLocationUser(new LocationUser.LocationUser
                        {LocationId = notebookLocation.Id, UserId = userId});
            }
            return notebookLocation.Id;
        }

        /// <summary>
        /// Return all NotebookLocations retrieved from the Database
        /// </summary>
        /// <returns></returns>
        public static List<NotebookLocation> GetAllNotebookLocationsFromDatabase()
        {
            if (Constant.User.Id == -1)
                return null;
            
            // Load all LocationUser records from UserID
            List<LocationUser.LocationUser> locationUserRecords = LocationUser.LocationUser.GetAllLocationsFromUser();

            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            List<NotebookLocation> notebookLocationsFromDatabase = new List<NotebookLocation>();
            foreach (LocationUser.LocationUser locationUser in locationUserRecords)
                notebookLocationsFromDatabase.Add(GetNotebookLocationById(locationUser.LocationId, notebookLocationRepository));
            return notebookLocationsFromDatabase;
        }
        
        #endregion
        
        #region Logic specific for Filesystem interaction
        
        /// <summary>
        /// Add a new NotebookLocation to the local storage
        /// </summary>
        /// <param name="notebookLocation"></param>
        /// <returns></returns>
        public static bool AddNotebookLocationToLocalStorage(NotebookLocation notebookLocation) =>
            XmlExporter.Export(StaticMethods.GetUserDataStoragePath(), "NotebookLocations.enex",
                GetNewXmlRepresentation(notebookLocation));

        /// <summary>
        /// Remove a NotebookLocation from the local storage
        /// </summary>
        /// <param name="notebookLocation"></param>
        /// <returns></returns>
        public static bool RemoveNotebookLocationFromLocalStorage(NotebookLocation notebookLocation) =>
            XmlExporter.Export(StaticMethods.GetUserDataStoragePath(), "NotebookLocations.enex",
                GetXmlRepresentationWithout(notebookLocation));
        
        /// <summary>
        /// Add this NotebookLocation locally
        /// </summary>
        /// <returns></returns>
        private NotebookLocation AddLocally() =>
            AddNotebookLocationToLocalStorage(this) ? this : null;
        
        /// <summary>
        /// Update the id of the local NotebookLocation
        /// </summary>
        /// <param name="oldId"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        private NotebookLocation Update(int oldId, int newId)
        {
            List<NotebookLocation> notebookLocations = XmlImporter.ImportNotebookLocations(StaticMethods.GetUserDataStoragePath() + @"/NotebookLocations.enex");
            notebookLocations.First(notebookLocation => notebookLocation.Id == oldId).Id = newId;
            
            Id = newId;
            
            return XmlExporter.Export(StaticMethods.GetUserDataStoragePath(), "NotebookLocations.enex", GetXmlRepresentation(notebookLocations)) ? this : null;
        }
        
        #endregion
        
        #region Other logics
        
        /// <summary>
        /// Return a NotebookLocation retrieved by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static NotebookLocation GetNotebookLocationById(int id)
        {
            List<NotebookLocation> notebookLocations = Load();
            
            foreach (NotebookLocation notebookLocation in notebookLocations)
                if (notebookLocation.Id == id)
                    return notebookLocation;

            return null;
        }
        
        /// <summary>
        /// Delete this NotebookLocation (locally as well as in the Database)
        /// </summary>
        /// <returns></returns>
        public bool Delete() =>
            Delete(new NotebookLocationRepository());
        
        /// <summary>
        /// Delete this NotebookLocation (locally as well as in the Database)
        /// </summary>
        /// <param name="notebookLocationRepository"></param>
        /// <returns></returns>
        public bool Delete(NotebookLocationRepository notebookLocationRepository)
        {
            bool removedCloud = false;

            if (Constant.User.Id != -1)
            {
                if (LocationUser.LocationUser.DeleteLocationUser(
                    LocationUser.LocationUser.GetLocationUserByNotebookLocation(this)))
                {
                    removedCloud = notebookLocationRepository.Delete(this);
                }
            }
            
            bool removedLocally = RemoveNotebookLocationFromLocalStorage(this);
            
            return removedCloud || removedLocally;
        }

        /// <summary>
        /// Return a NotebookLocation by path name
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static NotebookLocation GetNotebookLocationByPath(string path)
        {
            List<NotebookLocation> notebookLocations = Load();
            
            foreach (NotebookLocation notebookLocation in notebookLocations)
                if (notebookLocation.Path == path)
                    return notebookLocation;

            return null;
        }
        
        /// <summary>
        /// Add a new NotebookLocation locally (and try to add it to the database if UserID is not -1)
        /// </summary>
        /// <param name="notebookLocation"></param>
        /// <returns></returns>
        public static bool AddNewNotebookLocation(NotebookLocation notebookLocation)
        {
            bool addedToDatabase = AddNewNotebookLocationToDatabaseAndGetId(notebookLocation) != -1;
            bool addedLocally = AddNotebookLocationToLocalStorage(notebookLocation);

            return addedToDatabase || addedLocally;
        }

        /// <summary>
        /// Add a new NotebookLocation and return the id
        /// </summary>
        /// <param name="notebookLocation"></param>
        /// <returns></returns>
        public static int AddNewNotebookLocationAndGetId(NotebookLocation notebookLocation)
        {
            AddNewNotebookLocationToDatabaseAndGetId(notebookLocation);
            AddNotebookLocationToLocalStorage(notebookLocation);

            return notebookLocation.Id;
        }
        
        /// <summary>
        /// Return a list of all the NotebookLocations (this method also synchronizes all NotebookLocations)
        /// </summary>
        /// <returns></returns>
        public static List<NotebookLocation> Load(bool withoutSynchronize = false)
        {
            List<NotebookLocation> notebookLocations = new List<NotebookLocation>();
            
            // Load all the notebook locations stored in the local storage
            List<NotebookLocation> notebookLocationsFromFileSystem = XmlImporter.ImportNotebookLocations(StaticMethods.GetUserDataStoragePath() + @"/NotebookLocations.enex");
            
            // Load all the notebook locations stored in the database, if the user has a proper ID.
            if (Constant.User.Id != -1 && !withoutSynchronize)
            {
                List<NotebookLocation> notebookLocationsFromDatabase = GetAllNotebookLocationsFromDatabase();
                if (notebookLocationsFromFileSystem != null && notebookLocationsFromDatabase != null)
                {
                    // Check if db and fs id's are identical, if so, add. If not but paths are, update fs id.
                    foreach (NotebookLocation dbNotebookLocation in notebookLocationsFromDatabase)
                        foreach (NotebookLocation fsNotebookLocation in notebookLocationsFromFileSystem)
                            LoadNotebookLocation(fsNotebookLocation, dbNotebookLocation, notebookLocations);
                }

                if (notebookLocationsFromFileSystem != null)
                {
                    // If path from fs is not in db, add to db (and update local id)
                    foreach (NotebookLocation notebookLocation in notebookLocationsFromFileSystem.Where(notebookLocation => !notebookLocations.Contains(notebookLocation)))
                        notebookLocations.AddIfNotPresent(notebookLocation.Update(notebookLocation.Id, AddNewNotebookLocationToDatabaseAndGetId(notebookLocation)));
                }
                if (notebookLocationsFromDatabase != null)
                {
                    // If path from db is not in fs, add to fs
                    foreach (NotebookLocation notebookLocation in notebookLocationsFromDatabase.Where(notebookLocation => !notebookLocations.Contains(notebookLocation)))
                        notebookLocations.AddIfNotPresent(notebookLocation.AddLocally());
                }
            }
            else if (notebookLocationsFromFileSystem != null)
            {
                notebookLocations.AddRange(notebookLocationsFromFileSystem);
            }

            return notebookLocations;
        }
        
        /// <summary>
        /// Add the NotebookLocation to the list, if the local NotebookLocation doesn't have the correct id, update it 
        /// </summary>
        /// <param name="fsNotebookLocation"></param>
        /// <param name="dbNotebookLocation"></param>
        /// <param name="listToAddTo"></param>
        private static void LoadNotebookLocation(NotebookLocation fsNotebookLocation, NotebookLocation dbNotebookLocation, List<NotebookLocation> listToAddTo)
        {
            // If they are both the same Id, we update the filesystem path with the correct id
            if (dbNotebookLocation.Id == fsNotebookLocation.Id)
                listToAddTo.AddIfNotPresent(fsNotebookLocation);
            else if (dbNotebookLocation.Path == fsNotebookLocation.Path)
                listToAddTo.AddIfNotPresent(fsNotebookLocation.Update(fsNotebookLocation.Id, dbNotebookLocation.Id));
        }
        
        #endregion
        
        #region IParseable methods
        
        /// <summary>
        /// Get the Xml representation of all the NotebookLocations with the newly added NotebookLocation
        /// </summary>
        /// <param name="notebookLocation">The newly added NotebookLocation</param>
        /// <returns></returns>
        private static string[] GetNewXmlRepresentation(NotebookLocation notebookLocation) => 
            GetXmlRepresentation(XmlImporter.ImportNotebookLocations(StaticMethods.GetUserDataStoragePath() + @"/NotebookLocations.enex"), notebookLocation);
        
        /// <summary>
        /// Get the Xml representation of all the NotebookLocations currently loaded
        /// </summary>
        /// <param name="notebookLocations"></param>
        /// <param name="newNotebookLocation"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the Xml representation of all NotebookLocations without a specified NotebookLocation
        /// </summary>
        /// <param name="notebookLocation">The NotebookLocation that should not be in the representation</param>
        /// <returns></returns>
        private static string[] GetXmlRepresentationWithout(NotebookLocation notebookLocation)
        {
            List<NotebookLocation> notebookLocations =
                XmlImporter.ImportNotebookLocations(StaticMethods.GetUserDataStoragePath() + @"/NotebookLocations.enex");
                
            if (notebookLocations.Any(location => location.Path == notebookLocation.Path))
            {
                notebookLocations.Remove(notebookLocations.First(location => location.Path == notebookLocation.Path));
            }

            return GetXmlRepresentation(notebookLocations);
        }
        
        /// <summary>
        /// Get the Xml representation of this NotebookLocation
        /// </summary>
        /// <returns></returns>
        public string[] ToXmlRepresentation()
        {
            return new[] {
                "<location>",
                $"<id>{Id}</id>",
                $"<path>{Path}</path>",
                "</location>"
            };
        }
        
        #endregion

        #region Helper methods

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

        #endregion
    }
}
