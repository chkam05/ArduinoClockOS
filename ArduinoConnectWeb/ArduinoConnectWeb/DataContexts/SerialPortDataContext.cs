using ArduinoConnectWeb.Models.Serial;
using Microsoft.AspNetCore.Http;
using System.IO.Ports;

namespace ArduinoConnectWeb.DataContexts
{
    public class SerialPortDataContext
    {

        //  VARIABLES

        private List<SerialPortHandler> _connections;
        private object _connectionsLock = new object();


        //  GETTERS & SETTERS

        public List<SerialPortHandler> Connections
        {
            get
            {
                lock (_connectionsLock)
                {
                    return _connections;
                }
            }
            set
            {
                lock (_connectionsLock)
                {
                    _connections = value;
                }
            }
        }


        //  METHDOS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialPortDataContext class constructor. </summary>
        public SerialPortDataContext()
        {
            _connections = new List<SerialPortHandler>();
        }

        #endregion CLASS METHODS

        #region SERIAL PORT HANDLERS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add connection. </summary>
        /// <param name="handler"> Connection to add. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> Session already exists. </exception>
        public void AddConnection(SerialPortHandler handler)
        {
            if (handler is null)
                throw new ArgumentNullException($"{nameof(handler)} parameter is null.");

            if (Connections.Any(s => s.Equals(handler)))
                throw new ArgumentException("Connection already exists.");

            Connections.Add(handler);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if connection exists. </summary>
        /// <param name="handler"> Serial port handler. </param>
        /// <returns> True - session exists; False - otherwise. </returns>
        public bool HasConnection(SerialPortHandler handler)
        {
            return Connections != null && Connections.Any(s => s.Equals(handler));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if any connection exists. </summary>
        /// <returns> True - any connection exists; False - otherwise. </returns>
        public bool HasConnections()
        {
            return Connections != null && Connections.Any();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove connection. </summary>
        /// <param name="handler"> Connection to remove. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> Connection does not exist. </exception>
        public void RemoveConnection(SerialPortHandler handler)
        {
            if (handler is null)
                throw new ArgumentNullException($"{nameof(handler)} parameter is null.");

            int connectionIndex = Connections.FindIndex(s => s.Equals(handler));

            if (connectionIndex < 0)
                throw new ArgumentException("Connection does not exist.");

            Connections.RemoveAt(connectionIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update connection by replacing it. </summary>
        /// <param name="connection"> Connection to update. </param>
        /// <exception cref="ArgumentNullException"> One or more parameters are null. </exception>
        /// <exception cref="ArgumentException"> Connection does not exist. </exception>
        public void UpdateConnection(SerialPortHandler connection)
        {
            if (connection is null)
                throw new ArgumentNullException($"{nameof(connection)} parameter is null.");

            int connectionIndex = Connections.FindIndex(s => s.Port == connection.Port);

            if (connectionIndex < 0)
                throw new ArgumentException("Connection does not exist.");

            Connections.RemoveAt(connectionIndex);
            Connections.Add(connection);
        }

        #endregion SERIAL PORT HANDLERS MANAGEMENT METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get available serial ports. </summary>
        /// <returns> List of available serial ports. </returns>
        public List<string> GetAvailableSerialPorts()
        {
            return SerialPort.GetPortNames().Select(p => p.ToUpper()).ToList();
        }

        #endregion UTILITY METHODS

    }
}
