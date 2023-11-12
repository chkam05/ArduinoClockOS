using ArduinoConnectWeb.Models.Users;
using Newtonsoft.Json;

namespace ArduinoConnectWeb.DataContexts
{
    public class UsersDataContext
    {

        //  CONST

        private const string DEFAULT_FILE_PATH = "users_config.json";


        //  VARIABLES

        private string _storageFilePath;
        private object _storageFilePathLock = new object();

        private List<UserDataModel> _users;
        private object _usersLock = new object();


        //  GETTERS & SETTERS

        public string StorageFilePath
        {
            get
            {
                lock (_storageFilePathLock)
                {
                    return _storageFilePath;
                }
            }
            set
            {
                lock (_storageFilePathLock)
                {
                    _storageFilePath = value;
                }
            }
        }

        public List<UserDataModel> Users
        {
            get
            {
                lock (_usersLock)
                {
                    return _users;
                }
            }
            set
            {
                lock (_usersLock)
                {
                    _users = value;
                }
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UsersDataContext class constructor. </summary>
        /// <param name="storageFilePath"> Storage file path. </param>
        public UsersDataContext(string? storageFilePath = null)
        {
            _storageFilePath = storageFilePath ?? DEFAULT_FILE_PATH;
            _users = new List<UserDataModel>();
        }

        #endregion CLASS METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load data from file. </summary>
        /// <returns> True - data loaded successfully; False - otherwise. </returns>
        public bool LoadData()
        {
            var loadFilePath = StorageFilePath;

            if (!File.Exists(loadFilePath))
                return false;

            try
            {
                var fileContent = File.ReadAllText(loadFilePath);
                var users = JsonConvert.DeserializeObject<List<UserDataModel>>(fileContent);

                if (users == null || !users.Any())
                    return false;

                Users = users;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save data to file. </summary>
        /// <returns> True - data saved successfully; False - otherwise. </returns>
        public bool SaveData()
        {
            var saveFilePath = StorageFilePath;

            try
            {
                var fileContent = JsonConvert.SerializeObject(Users, Formatting.Indented);
                File.WriteAllText(saveFilePath, fileContent);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion LOAD & SAVE METHODS

        #region USERS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add user. </summary>
        /// <param name="user"> User data model to add. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> User already exists. </exception>
        public void AddUser(UserDataModel user)
        {
            if (user is null)
                throw new ArgumentNullException($"{nameof(user)} parameter is null.");

            if (Users.Any(u => u.Equals(user)))
                throw new ArgumentException("User already exists.");

            Users.Add(user);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if user exists. </summary>
        /// <param name="user"> User data model. </param>
        /// <returns> True - user exists; False - otherwise. </returns>
        public bool HasUser(UserDataModel user)
        {
            return Users != null && Users.Any(u => u.Equals(user));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if any user exists. </summary>
        /// <returns> True - any user exists; False - otherwise. </returns>
        public bool HasUsers()
        {
            return Users != null && Users.Any();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove user. </summary>
        /// <param name="user"> User data model to remove. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> User does not exist. </exception>
        public void RemoveUser(UserDataModel user)
        {
            if (user is null)
                throw new ArgumentNullException($"{nameof(user)} parameter is null.");

            int userIndex = Users.FindIndex(u => u.Equals(user));

            if (userIndex < 0)
                throw new ArgumentException("User does not exist.");

            Users.RemoveAt(userIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update user by replacing it. </summary>
        /// <param name="user"> User data model to update. </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateUser(UserDataModel user)
        {
            if (user is null)
                throw new ArgumentNullException($"{nameof(user)} parameter is null.");

            int userIndex = Users.FindIndex(u => u.Id == user.Id);

            if (userIndex < 0)
                throw new ArgumentException("User does not exist.");

            Users.RemoveAt(userIndex);
            Users.Add(user);
        }

        #endregion USERS MANAGEMENT METHODS

    }
}
