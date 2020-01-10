using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    /// <summary>
    /// This class handles all logic for LocationUsers
    /// </summary>
    public class LocationUser : LocationUserModel
    {
        /// <summary>
        /// Returns all the LocationUsers linked to the current User
        /// </summary>
        /// <returns>A List from LocationUsers</returns>
        public static List<LocationUser> GetAllLocationsFromUser()
        {
            return new LocationUserRepository().GetBy(
                new[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", Constant.User.Id } }
            ).Select(el => (LocationUser)el).ToList();
        }

        /// <summary>
        /// Insert a new LocationUser
        /// </summary>
        /// <param name="locationUser">The to be inserted LocationUser</param>
        /// <returns>A boolean indicating if the insert went successfully</returns>
        public static bool AddNewLocationUser(LocationUser locationUser) =>
            new LocationUserRepository().Insert(locationUser);
        
        /// <summary>
        /// Delete a LocationUser record
        /// </summary>
        /// <param name="locationUser">The record that needs to be removed</param>
        /// <returns>A boolean indicating if it has been removed</returns>
        public static bool DeleteLocationUser(LocationUser locationUser) =>
            new LocationUserRepository().Delete(locationUser);

        /// <summary>
        /// Returns a LocationUser based on a NotebookLocation.Path and a UserId
        /// </summary>
        /// <param name="notebookLocation">The NotebookLocation used to retrieve a specific LocationUser</param>
        /// <returns>The requested LocationUser</returns>
        public static LocationUser GetLocationUserByNotebookLocation(NotebookLocation notebookLocation)
        {
            List<LocationUser> locationUsers = new LocationUserRepository().GetBy(
                new[] { "UserID = @UserID", "LocationID = @LocationID" },
                new Dictionary<string, object> { { "@UserID", Constant.User.Id }, { "@LocationID", notebookLocation.Id } }
            ).Select(el => (LocationUser)el).ToList();

            if (locationUsers.Count == 1)
            {
                return locationUsers[0];
            }
            
            return null;
        }
    }
}
