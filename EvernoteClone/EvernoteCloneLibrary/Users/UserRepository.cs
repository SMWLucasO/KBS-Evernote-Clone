using EvernoteCloneLibrary.Database;
using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Users
{
    public class UserRepository : IRepository<UserModel>
    {

        //Deletes user
        public bool Delete(UserModel ToDelete)
        {
            if (ToDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"@Id", ToDelete.Id }
                };
                return DataAccess.Instance.Execute("DELETE FROM [User] WHERE Id = @Id", parameters);
            }
            return false;
        }


        //Gets data from database
        public Dictionary<string, object> GenerateQueryParameters(UserModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Username", ToExtractFrom.Username },
                    { "@Password", ToExtractFrom.Password },
                    { "@IsGoogleAccount", ToExtractFrom.IsGoogleAccount },
                    { "@CreationDate", ToExtractFrom.CreationDate }
                };

                if (!string.IsNullOrWhiteSpace(ToExtractFrom.FirstName))
                    parameters.Add("@FirstName", ToExtractFrom.FirstName);
                if (!string.IsNullOrWhiteSpace(ToExtractFrom.LastName))
                    parameters.Add("@LastName", ToExtractFrom.LastName);
                if (ToExtractFrom.LastLogin != null)
                    parameters.Add("@LastLogin", ToExtractFrom.LastLogin);

                return parameters;
            }
            return null;
        }

        public Dictionary<string, object> GenerateLoginParameters(UserModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Username", ToExtractFrom.Username },
                    { "@Password", ToExtractFrom.Password }

                };

                return parameters;
            }
            return null;
        }

        //Fetch data
        public IEnumerable<UserModel> GetBy(string[] Conditions, Dictionary<string, object> Parameters)
        {
            List<User> usersList = new List<User>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("User", Conditions, Parameters);

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

        // Insert data in database
        public bool Insert(UserModel ToInsert)
        {

            if (ToInsert != null)
            {
                if (string.IsNullOrEmpty(ToInsert.Username) || string.IsNullOrEmpty(ToInsert.Password)
                    || ToInsert.CreationDate == null)
                {
                    return false;
                }

                Dictionary<string, object> Parameters = GenerateQueryParameters(ToInsert);
                int id = DataAccess.Instance.ExecuteAndReturnId(
                    "INSERT INTO [User] ([Username], [Password]," +
                        (string.IsNullOrWhiteSpace(ToInsert.FirstName) ? "" : " [FirstName],") +
                        (string.IsNullOrWhiteSpace(ToInsert.LastName) ? "" : " [LastName],") +
                        " [IsGoogleAccount], [CreationDate]" +
                        (ToInsert.LastLogin == null ? "" : ", [LastLogin]") +
                    ") VALUES (@Username, @Password," +
                        (string.IsNullOrWhiteSpace(ToInsert.FirstName) ? "" : " @FirstName,") +
                        (string.IsNullOrWhiteSpace(ToInsert.LastName) ? "" : " @LastName,") +
                        " @IsGoogleAccount, @CreationDate" +
                        (ToInsert.LastLogin == null ? "" : ", @LastLogin") +
                    ")", Parameters);

                if (id != -1)
                {
                    ToInsert.Id = id;
                }

                return id != -1;
            }

            return false;
        }

        //Update user
        public bool Update(UserModel ToUpdate)
        {
            if (ToUpdate != null)
            {
                if (string.IsNullOrEmpty(ToUpdate.FirstName) || string.IsNullOrEmpty(ToUpdate.LastName) || string.IsNullOrEmpty(ToUpdate.Password) || string.IsNullOrEmpty(ToUpdate.Username))
                {
                    return false;
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(ToUpdate);
                parameters.Add("@Id", ToUpdate.Id);

                return DataAccess.Instance.Execute("UPDATE [User] SET [FirstName] = @FirstName, [LastName] = @LastName, "
                    + "[Username] = @Username, [Password] = @Password WHERE Id = @Id", parameters);

            }
            return false;
        }

        public UserModel CompareDB(UserModel Comparedb)
        {
            if (Comparedb != null)
            {
                if (string.IsNullOrEmpty(Comparedb.Username) || string.IsNullOrEmpty(Comparedb.Password))
                    return null;
                
                Dictionary<string, object> parameters = GenerateLoginParameters(Comparedb);
                var user = this.GetBy(new[] { "Username = @Username", "Password = @Password" }, parameters).ToList();

                if (user.Count > 0)
                    return user[0];
                return null;

            }
            return null;
        }
    }
}
