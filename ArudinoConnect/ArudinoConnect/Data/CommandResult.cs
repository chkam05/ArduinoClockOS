using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class CommandResult
    {

        //  VARIABLES

        public string Data { get; set; }
        public bool Success { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CommandResult class constructor. </summary>
        public CommandResult()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> CommandResult class constructor. </summary>
        /// <param name="success"> Execution successed. </param>
        /// <param name="data"> Result data. </param>
        public CommandResult(bool success, string data)
        {
            Success= success;
            Data = data;
        }

        #endregion CLASS METHHODS

    }
}
