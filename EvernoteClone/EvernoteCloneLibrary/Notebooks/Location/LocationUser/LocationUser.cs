using System.Collections.Generic;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    public class LocationUser : LocationUserModel
    {
        // TODO: add summary
        public static List<LocationUser> GetAllLocationsFromUser(int UserID)
        {
            LocationUserRepository locationUserRepository = new LocationUserRepository();
            return locationUserRepository.GetBy(
                new string[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", UserID } }
            ).Select((el) => ((LocationUser)el)).ToList();
        }

        public static bool AddNewLocationUser(LocationUser locationUser)
        {
            LocationUserRepository locationUserRepository = new LocationUserRepository();
            return locationUserRepository.Insert(locationUser);
        }
    }
}
