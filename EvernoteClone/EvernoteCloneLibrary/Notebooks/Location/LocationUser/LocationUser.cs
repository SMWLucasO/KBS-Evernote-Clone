using System.Collections.Generic;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    public class LocationUser : LocationUserModel
    {
        // TODO: add summary
        public static List<LocationUser> GetAllLocationsFromUser(int userId)
        {
            LocationUserRepository locationUserRepository = new LocationUserRepository();
            return locationUserRepository.GetBy(
                new[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", userId } }
            ).Select((el) => ((LocationUser)el)).ToList();
        }

        public static bool AddNewLocationUser(LocationUser locationUser) =>
            new LocationUserRepository().Insert(locationUser);
    }
}
