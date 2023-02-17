using ArudinoConnect.Data;
using ArudinoConnect.Events;
using ArudinoConnect.Static;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Utilities
{
    public class SerialCommander
    {

        //  VARIABLES

        private SerialPortConnection _connection;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialCommander class constructor. </summary>
        /// <param name="serialPortConnection"> Serial port connection object. </param>
        public SerialCommander(SerialPortConnection serialPortConnection)
        {
            _connection = serialPortConnection;
        }

        #endregion CLASS METHODS

        #region COMMANDER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Execute async multiple configuration uplaod command. </summary>
        /// <param name="data"> List of configuration command data carrier objects. </param>
        /// <param name="autoStart">  Auto start async process. </param>
        /// <param name="allowCancel"> Allow to cancel process. </param>
        /// <param name="progressChangedEvent"> Method that will be invoked after executing single command. </param>
        /// <param name="runWorkerCompletedEvent"> Method that will be invoked after executing command. </param>
        /// <returns> Async background executer. </returns>
        public BackgroundWorker UploadConfigurationAsync(List<ConfigCommandCarrier> data, bool autoStart = true, bool allowCancel = true, 
            ProgressChangedEventHandler progressChangedEvent = null,
            RunWorkerCompletedEventHandler runWorkerCompletedEvent = null)
        {
            var bgSetter = new BackgroundWorker();
            bgSetter.WorkerReportsProgress = true;
            bgSetter.WorkerSupportsCancellation = allowCancel;

            bgSetter.DoWork += (s, ew) =>
            {
                var result = new List<ConfigCommandResult>();

                foreach (var singleCommand in data)
                {
                    if (ew.Cancel)
                        break;

                    bgSetter.ReportProgress(data.IndexOf(singleCommand), singleCommand.Message);

                    var singleCommandResult = ExecuteCommand(singleCommand.Command, singleCommand.RequiredResponse);

                    result.Add(new ConfigCommandResult()
                    {
                        Result = singleCommandResult,
                        CompleteMessage = singleCommand.CompleteMessage,
                        FailMessage = singleCommand.FailMessage,
                    });
                }

                ew.Result = result;
            };

            bgSetter.ProgressChanged += (s, ep) =>
            {
                progressChangedEvent?.Invoke(s, ep);
            };

            bgSetter.RunWorkerCompleted += (s, ec) =>
            {
                runWorkerCompletedEvent?.Invoke(s, ec);
            };

            if (autoStart)
                bgSetter.RunWorkerAsync();

            return bgSetter;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Execute async single configuration uplaod command. </summary>
        /// <param name="data"> Configuration command data carrier object. </param>
        /// <param name="autoStart"> Auto start async process. </param>
        /// <param name="runWorkerCompletedEvent"> Method that will be invoked after executing command. </param>
        /// <returns> Async background executer. </returns>
        public BackgroundWorker UploadSingleConfigurationAsync(ConfigCommandCarrier data, bool autoStart = true,
            RunWorkerCompletedEventHandler runWorkerCompletedEvent = null)
        {
            var bgSetter = new BackgroundWorker();

            bgSetter.DoWork += (s, ew) =>
            {
                var result = ExecuteCommand(data.Command, data.RequiredResponse);
                ew.Result = result;
            };

            bgSetter.RunWorkerCompleted += (s, ec) =>
            {
                runWorkerCompletedEvent?.Invoke(s, ec);
            };

            if (autoStart)
                bgSetter.RunWorkerAsync();

            return bgSetter;
        }

        #endregion COMMANDER METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Execute command with direct output. </summary>
        /// <param name="command"> Command to execute. </param>
        /// <param name="timeout"> Time waiting for response in miliseconds. </param>
        /// <returns> Tuple (bool, string) where first value idicates success or fail, 
        /// and second value contains received data. </returns>
        private CommandResult ExecuteCommand(string command, string requiredResponse = null, int timeout = 5000)
        {
            string message = null;
            bool successed = false;

            if (_connection.IsConnected)
            {
                bool cancel = false;
                DateTime dtStart = DateTime.Now;

                var receiver = new EventHandler<SerialPortReceivedMessageEventArgs>((s, e) =>
                {
                    if (e.HasMessage)
                    {
                        switch (e.State)
                        {
                            case ReceivedMessageState.Error:
                            case ReceivedMessageState.System:
                                cancel = true;
                                break;

                            case ReceivedMessageState.Message:
                                message = e.Message.Replace(command, "").Replace(Environment.NewLine, "");
                                successed = true;
                                break;
                        }
                    }
                });

                _connection.ReceivedMessage += receiver;
                _connection.SendMessage(command);

                while (!cancel && dtStart.AddMilliseconds(timeout) > DateTime.Now)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        if (string.IsNullOrEmpty(requiredResponse))
                            break;
                        else if (message.EndsWith(requiredResponse))
                            break;
                    }
                };

                _connection.ReceivedMessage -= receiver;
            }

            return new CommandResult(successed, message);
        }

        #endregion UTILITY METHODS

    }
}
