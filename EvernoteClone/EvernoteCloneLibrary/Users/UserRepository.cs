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
        //Deletes user
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
        
        //Gets data from database
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
                    parameters.Add("@FirstName", toExtractFrom.FirstName);
                if (!string.IsNullOrWhiteSpace(toExtractFrom.LastName))
                    parameters.Add("@LastName", toExtractFrom.LastName);
                if (toExtractFrom.LastLogin != null)
                    parameters.Add("@LastLogin", toExtractFrom.LastLogin);

                return parameters;
            }
            return null;
        }

        //Fetch data
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
                    FirstName = (string)fetchedSqlDataReader["FirstName"],
                    LastName = (string)fetchedSqlDataReader["LastName"],
                    IsGoogleAccount = (bool)fetchedSqlDataReader["IsGoogleAccount"],
                    CreationDate = (DateTime)fetchedSqlDataReader["CreationDate"],
                    LastLogin = (DateTime)fetchedSqlDataReader["LastLogin"],
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

        // Insert data in database
        public bool Insert(UserModel toInsert)
        {
            if (toInsert != null)
            {
                if (string.IsNullOrEmpty(toInsert.Username) || string.IsNullOrEmpty(toInsert.Password) || toInsert.CreationDate == null)
                    return false;

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
                    toInsert.Id = id;
                return id != -1;
            }
            return false;
        }

        //Update user
        public bool Update(UserModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (string.IsNullOrEmpty(toUpdate.FirstName) || string.IsNullOrEmpty(toUpdate.LastName) || string.IsNullOrEmpty(toUpdate.Password) || string.IsNullOrEmpty(toUpdate.Username))
                    return false;

                Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                parameters.Add("@Id", toUpdate.Id);

                return DataAccess.Instance.Execute("UPDATE [User] SET [FirstName] = @FirstName, [LastName] = @LastName, "
                    + "[Username] = @Username, [Password] = @Password WHERE Id = @Id", parameters);
            }
            return false;
        }
    }
}
