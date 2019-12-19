using EvernoteCloneLibrary.Database;
using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

// TODO change comments to summary @Chino or @Mart
namespace EvernoteCloneLibrary.Users
{
    public class UserRepository : IRepository<UserModel>
    {
        /// <summary>
        /// Delete the user by its Id, uses a UserModel to identify the user.
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(UserModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [User] WHERE Id = @Id", parameters);
            }

            return false;
        }

        /// <summary>
        /// Generate the query parameters for usage when executing sql queries
        /// </summary>
        /// <param name="toExtractFrom"></param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(UserModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Username", toExtractFrom.Username },
                    { "@Password", toExtractFrom.Password },
                    { "@IsGoogleAccount", toExtractFrom.IsGoogleAccount },
                    { "@CreationDate", toExtractFrom.CreationDate }
                };

                if (!string.IsNullOrWhiteSpace(toExtractFrom.FirstName))
                {
                    parameters.Add("@FirstName", toExtractFrom.FirstName);
                }

                if (!string.IsNullOrWhiteSpace(toExtractFrom.LastName))
                {
                    parameters.Add("@LastName", toExtractFrom.LastName);
                }

                if (toExtractFrom.LastLogin != null)
                {
                    parameters.Add("@LastLogin", toExtractFrom.LastLogin);
                }

                return parameters;
            }
            return null;
        }

        /// <summary>
        /// Generate the parameters for the login query 
        /// </summary>
        /// <param name="toExtractFrom"></param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateLoginParameters(UserModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Username", toExtractFrom.Username },
                    { "@Password", toExtractFrom.Password }

                };

                return parameters;
            }
            return null;
        }

        /// <summary>
        /// Select user models based upon given conditions
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<UserModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<User> usersList = new List<User>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("User", conditions, parameters);

            NotebookRepository notebookRepository = new NotebookRepository();

            while (fetchedSqlDataReader.Read())
            {
                User user = new User()
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    Username = (string)fetchedSqlDataReader["Username"],
                    Password = (string)fetchedSqlDataReader["Password"],
                    FirstName = !(fetchedSqlDataReader["FirstName"] is DBNull) ? (string)fetchedSqlDataReader["FirstName"] : null,
                    LastName = !(fetchedSqlDataReader["LastName"] is DBNull) ? (string)fetchedSqlDataReader["LastName"] : null,
                    IsGoogleAccount = (bool)fetchedSqlDataReader["IsGoogleAccount"],
                    CreationDate = (DateTime)fetchedSqlDataReader["CreationDate"],
                    LastLogin = !(fetchedSqlDataReader["LastLogin"] is DBNull) ? (DateTime)fetchedSqlDataReader["LastLogin"] : DateTime.Now,
                    Notebooks = notebookRepository.GetBy(
                            new string[] { "UserID = @UserID" },
                            new Dictionary<string, object>()
                            {
                                {"@UserID", (int)fetchedSqlDataReader ["Id"] }
                            })
                    .Select((el) => ((Notebook)el)).ToList()
                };

                usersList.Add(user);
            }

            DataAccess.Instance.CloseSqlConnection();
            fetchedSqlDataReader.Close();

            return usersList.AsEnumerable();
        }

        /// <summary>
        /// Insert a new user into the database, where the UserModel represents the data to insert
        /// </summary>
        /// <param name="toInsert"></param>
        /// <returns></returns>
        public bool Insert(UserModel toInsert)
        {
            if (toInsert != null)
            {
                if (string.IsNullOrWhiteSpace(toInsert.Username) || string.IsNullOrWhiteSpace(toInsert.Password) || toInsert.CreationDate == null)
                {
                    return false;
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId(
                    "INSERT INTO [User] ([Username], [Password]," +
                        (string.IsNullOrWhiteSpace(toInsert.FirstName) ? "" : " [FirstName],") +
                        (string.IsNullOrWhiteSpace(toInsert.LastName) ? "" : " [LastName],") +
                        " [IsGoogleAccount], [CreationDate]" +
                        (toInsert.LastLogin == null ? "" : ", [LastLogin]") +
                        ") VALUES (@Username, @Password," +
                        (string.IsNullOrWhiteSpace(toInsert.FirstName) ? "" : " @FirstName,") +
                        (string.IsNullOrWhiteSpace(toInsert.LastName) ? "" : " @LastName,") +
                        " @IsGoogleAccount, @CreationDate" +
                        (toInsert.LastLogin == null ? "" : ", @LastLogin") +
                        ")", parameters);

                if (id != -1)
                {
                    toInsert.Id = id;
                }

                return id != -1;
            }
            return false;
        }

        /// <summary>
        /// Update the given user by its UserModel
        /// </summary>
        /// <param name="toUpdate"></param>
        /// <returns></returns>
        public bool Update(UserModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (string.IsNullOrWhiteSpace(toUpdate.FirstName) || string.IsNullOrWhiteSpace(toUpdate.LastName) || string.IsNullOrWhiteSpace(toUpdate.Password) || string.IsNullOrWhiteSpace(toUpdate.Username))
                {
                    return false;
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                parameters.Add("@Id", toUpdate.Id);

                return DataAccess.Instance.Execute("UPDATE [User] SET [FirstName] = @FirstName, [LastName] = @LastName, "
                    + "[Username] = @Username, [Password] = @Password WHERE Id = @Id", parameters);
            }

            return false;
        }

        /// <summary>
        /// @Chino TODO add summary
        /// </summary>
        /// <param name="Comparedb"></param>
        /// <returns></returns>
        public UserModel CompareDB(UserModel Comparedb)
        {
            if (Comparedb != null)
            {
                if (string.IsNullOrWhiteSpace(Comparedb.Username) || string.IsNullOrWhiteSpace(Comparedb.Password))
                {
                    return null;
                }
                    
                Dictionary<string, object> parameters = GenerateLoginParameters(Comparedb);
                var user = this.GetBy(new[] { "Username = @Username", "Password = @Password" }, parameters).ToList();

                if (user.Count > 0)
                {
                    return user[0];
                }

            }

            return null;
        }
    }
}
