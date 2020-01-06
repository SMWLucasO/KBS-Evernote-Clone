using System.Collections.Generic;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    public class LocationUser : LocationUserModel
    {
        
        /// <summary>
        /// Fetch all the links between users and notebooklocations
        /// </summary>
        /// <returns></returns>
        public static List<LocationUser> GetAllLocationsFromUser()
        {
            LocationUserRepository locationUserRepository = new LocationUserRepository();
            return locationUserRepository.GetBy(
                new[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", Constants.Constant.User.Id } }
            ).Select((el) => ((LocationUser)el)).ToList();
        }

        /// <summary>
        /// Insert a new link between a user and a notebooklocation
        /// </summary>
        /// <param name="locationUser"></param>
        /// <returns></returns>
        public static bool AddNewLocationUser(LocationUser locationUser) =>
            new LocationUserRepository().Insert(locationUser);
    }
}
