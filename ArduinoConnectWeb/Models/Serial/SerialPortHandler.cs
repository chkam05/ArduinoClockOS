using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Models.Users;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO.Ports;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArduinoConnectWeb.Models.Serial
{
    public class SerialPortHandler : BaseUniqueDataModel, ICloneable
    {

        //  VARIABLES

        private List<MessageDataModel> _messages;
        private BackgroundWorker? _receiver;

        public int BaudRate { get; set; }
        public string Port { get; set; }
        public TimeSpan ProcessingTimeout { get; set; }
        public TimeSpan ReadTimeout { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public SerialPort? SerialPort { get; set; }
        public string UserId { get; set; }


        //  GETTERS & SETTERS

        private TimeSpan FullTimeout
        {
            get => ProcessingTimeout + ReadTimeout + SendTimeout;
        }

        public bool IsAnyMessage
        {
            get => _messages != null && _messages.Any();
        }

        public bool IsConnected
        {
            get => SerialPort != null && SerialPort.IsOpen;
        }

        public bool IsReceiverWorking
        {
            get => _receiver != null && _receiver.IsBusy;
        }

        public int MessagesCount
        {
            get => _messages?.Count ?? 0;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialPortHandler class constructor. </summary>
        /// <param name="port"> Port. </param>
        /// <param name="baudRate"> Baud rate. </param>
        /// <param name="userId"> User identifier that created connection. </param>
        /// <param name="processingTimeout"> Processing timeout between send and receive. </param>
        /// <param name="readTimeout"> Read timeout. </param>
        /// <param name="sendTimeout"> Send timeout. </param>
        public SerialPortHandler(string port, int baudRate, string userId,
            TimeSpan? processingTimeout = null, TimeSpan? readTimeout = null, TimeSpan? sendTimeout = null) : base(null)
        {
            _messages = new List<MessageDataModel>();

            Port = port;
            BaudRate = baudRate;
            UserId = userId;

            ProcessingTimeout = processingTimeout ?? TimeSpan.FromMilliseconds(1000);
            ReadTimeout = readTimeout ?? TimeSpan.FromMilliseconds(1000);
            SendTimeout = sendTimeout ?? TimeSpan.FromMilliseconds(1000);
        }

        #endregion CLASS METHODS

        #region CLONE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public object Clone()
        {
            var serializedObject = JsonConvert.SerializeObject(this);
            var clonedObject = JsonConvert.DeserializeObject<SerialPortHandler>(serializedObject);

            return clonedObject;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public SerialPortHandler? CloneWithType()
        {
            return (SerialPortHandler)Clone();
        }

        #endregion CLONE METHODS

        #region COMMUNICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Open serial port connection. </summary>
        /// <exception cref="Exception"> Could not connect or connection is active. </exception>
        public void Connect()
        {
            if (SerialPort == null)
            {
                SerialPort = new SerialPort(Port, BaudRate);
                SerialPort.ReadTimeout = Convert.ToInt32(ReadTimeout.TotalMilliseconds);
                SerialPort.WriteTimeout = Convert.ToInt32(SendTimeout.TotalMilliseconds);
            }

            if (IsConnected)
                throw new Exception($"{Port}: Connection is already established");

            SerialPort.Open();
            SetupReceiver();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Close serial port connection. </summary>
        /// <exception cref="Exception"> Could not disconnect or connection is closed. </exception>
        public void Disconnect()
        {
            if (SerialPort == null || !IsConnected)
                throw new Exception($"{Port}: Connection is already closed");

            if (IsReceiverWorking)
                _receiver?.CancelAsync();

            SerialPort.Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get last received message. </summary>
        /// <returns> Message data model. </returns>
        public MessageDataModel? GetLastMessage()
        {
            var index = MessagesCount - 1;
            var message = index >= 0 ? _messages[index] : null;

            if (message != null)
            {
                _messages.RemoveAt(index);
                return message;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get received message in the order received. </summary>
        /// <returns> Message data model. </returns>
        public MessageDataModel? GetMessage()
        {
            var index = 0;
            var message = MessagesCount > index ? _messages[0] : null;

            if (message != null)
            {
                _messages.RemoveAt(index);
                return message;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send message. </summary>
        /// <param name="message"> Message to send. </param>
        /// <exception cref="Exception"> Could not send message or connection is closed. </exception>
        public void SendMessage(string message)
        {
            if (SerialPort == null || !IsConnected)
                throw new Exception($"{Port}: Connection closed");

            SerialPort.WriteLine(message);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send message and wait for response. </summary>
        /// <param name="message"> Message to send. </param>
        /// <param name="additionalWaitTime"> Additional wait time to timeout. </param>
        /// <returns> List of messages data model. </returns>
        /// <exception cref="Exception"> Connection closed or time out. </exception>
        public List<MessageDataModel> SendAndReceiveMessage(string message, TimeSpan? additionalWaitTime)
        {
            StopReceiver();

            Exception? lastException = null;

            var sendTime = DateTime.Now;
            var timeOut = FullTimeout + (additionalWaitTime ?? TimeSpan.Zero);
            var result = new List<MessageDataModel>();
            var waitForResponse = true;

            SendMessage(message);

            while (waitForResponse)
            {
                if (DateTime.Now - sendTime > timeOut)
                {
                    waitForResponse = false;
                    break;
                }

                if (SerialPort == null || !IsConnected)
                    throw new Exception($"{Port}: Connection closed unexpectedly");

                if (SerialPort != null && IsConnected)
                {
                    try
                    {
                        var responseMessage = SerialPort.ReadLine();

                        if (!string.IsNullOrEmpty(responseMessage))
                        {
                            result.Add(new MessageDataModel(responseMessage));
                            sendTime = DateTime.Now;
                        }
                    }
                    catch (TimeoutException texc)
                    {
                        lastException = texc;
                    }
                    catch (IOException exc)
                    {
                        lastException = exc;
                    }
                }
            }

            if (IsConnected && !IsReceiverWorking)
                SetupReceiver();

            if (!result.Any() && lastException != null)
                throw lastException;

            return result;
        }

        #endregion COMMUNICATION METHODS

        #region OVERWRITTEN OPERATOR METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Determines whether the specified object is equal to the current object. </summary>
        /// <param name="obj"> Object to compare. </param>
        /// <returns> True - if the specified object is equal to the current object; False - otherwise. </returns>
        public override bool Equals(object? obj)
        {
            if (obj is SerialPortHandler serialPortHandler)
            {
                return Port == serialPortHandler.Port;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serves as the default hash function. </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return Port.GetHashCode();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if the objects are equal. </summary>
        /// <param name="left"> Left side object. </param>
        /// <param name="right"> Right side object. </param>
        /// <returns> True - objects are equal; False - otherwise. </returns>
        public static bool operator ==(SerialPortHandler left, SerialPortHandler right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if the objects are not equal. </summary>
        /// <param name="left"> Left side object. </param>
        /// <param name="right"> Right side object. </param>
        /// <returns> True - objects are not equal; False - otherwise. </returns>
        public static bool operator !=(SerialPortHandler left, SerialPortHandler right)
        {
            return !(left == right);
        }

        #endregion OVERWRITTEN OPERATOR METHODS

        #region RECEIVER MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup serial port messages receiver. </summary>
        private void SetupReceiver()
        {
            if (IsReceiverWorking)
                _receiver?.CancelAsync();

            _receiver = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _receiver.DoWork += ReceiverDoWork;
            _receiver.ProgressChanged += ReceiverReportProgress;
            _receiver.RunWorkerCompleted += ReceiverWorkComplete;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop serial port messages receiver. </summary>
        /// <exception cref="Exception"> Stopping receiver time out. </exception>
        private void StopReceiver()
        {
            if (IsReceiverWorking)
            {
                var sendTime = DateTime.Now;

                _receiver?.CancelAsync();

                while (IsReceiverWorking)
                {
                    if (DateTime.Now - sendTime > FullTimeout)
                        throw new Exception($"{Port}: Receiving message timed out");
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Messages receiver work method. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Do Work Event Arguments. </param>
        private void ReceiverDoWork(object? sender, DoWorkEventArgs e)
        {
            bool enabled = true;
            int messagesCount = 0;
            var receiver = sender as BackgroundWorker;

            while(enabled)
            {
                try
                {
                    if (e.Cancel || (receiver?.CancellationPending ?? false))
                    {
                        enabled = false;
                        break;
                    }

                    if (SerialPort == null || !IsConnected)
                    {
                        enabled = false;
                        break;
                    }

                    var message = SerialPort.ReadLine();

                    if (!string.IsNullOrEmpty(message))
                    {
                        messagesCount = messagesCount == int.MaxValue ? 0 : messagesCount + 1;
                        receiver?.ReportProgress(messagesCount, new object[] { message });
                    }
                }
                catch (Exception exc)
                {
                    receiver?.ReportProgress(messagesCount, new object[] { exc.Message });
                    enabled = false;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Messages receiver progress reporter. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Progress Changed Event Arguments. </param>
        private void ReceiverReportProgress(object? sender, ProgressChangedEventArgs e)
        {
            var userState = e.UserState as object[];
            var message = userState?[0] as string;

            if (!string.IsNullOrEmpty(message))
                _messages.Add(new MessageDataModel(message));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Messages receiver final method. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Run Worker Completed Event Arguments. </param>
        private void ReceiverWorkComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && IsConnected)
                Disconnect();
        }

        #endregion RECEIVER MANAGEMENT METHODS

    }
}
