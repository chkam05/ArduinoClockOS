using ArduinoConnectWeb.Models.Auth;

namespace ArduinoConnectWeb.DataContexts
{
    public class AuthDataContext
    {

        //  VARIABLES

        private List<SessionDataModel> _sessions;
        private object _sessionsLock = new object();


        //  GETTERS & SETTERS

        public List<SessionDataModel> Sessions
        {
            get
            {
                lock (_sessionsLock)
                {
                    return _sessions;
                }
            }
            set
            {
                lock (_sessionsLock)
                {
                    _sessions = value;
                }
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AuthDataContext class constructor. </summary>
        public AuthDataContext()
        {
            _sessions = new List<SessionDataModel>();
        }

        #endregion CLASS METHODS

        #region SESSIONS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add session. </summary>
        /// <param name="session"> Session data model to add. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> Session already exists. </exception>
        public void AddSession(SessionDataModel session)
        {
            if (session is null)
                throw new ArgumentNullException($"{nameof(session)} parameter is null.");

            if (Sessions.Any(s => s.Equals(session)))
                throw new ArgumentException("Session already exists.");

            Sessions.Add(session);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if session exists. </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> True - session exists; False - otherwise. </returns>
        public bool HasSession(SessionDataModel session)
        {
            return Sessions != null && Sessions.Any(s => s.Equals(session));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if any session exists. </summary>
        /// <returns> True - any session exists; False - otherwise. </returns>
        public bool HasSessions()
        {
            return Sessions != null && Sessions.Any();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove session. </summary>
        /// <param name="session"> Session data model to remove. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> Session does not exist. </exception>
        public void RemoveSession(SessionDataModel session)
        {
            if (session is null)
                throw new ArgumentNullException($"{nameof(session)} parameter is null.");

            int sessionIndex = Sessions.FindIndex(s => s.Equals(session));

            if (sessionIndex < 0)
                throw new ArgumentException("Session does not exist.");

            Sessions.RemoveAt(sessionIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove sessions. </summary>
        /// <param name="sessions"> Sessions collection to remove. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        public void RemoveSessions(IEnumerable<SessionDataModel> sessions)
        {
            if (sessions is null)
                throw new ArgumentNullException($"{nameof(sessions)} parameter is null.");

            Sessions.RemoveAll(s => sessions.Contains(s));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update session by replacing it. </summary>
        /// <param name="session"> Session data model to update. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> Session does not exist. </exception>
        public void UpdateSession(SessionDataModel session)
        {
            if (session is null)
                throw new ArgumentNullException($"{nameof(session)} parameter is null.");

            int sessionIndex = Sessions.FindIndex(s => s.Id == session.Id);

            if (sessionIndex < 0)
                throw new ArgumentException("Session does not exist.");

            Sessions.RemoveAt(sessionIndex);
            Sessions.Add(session);
        }

        #endregion SESSIONS MANAGEMENT METHODS

    }
}
