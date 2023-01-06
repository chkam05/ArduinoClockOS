using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class ConfigCommandResult
    {

        //  VARIABLES

        public CommandResult Result { get; set; }
        public string CompleteMessage { get; set; }
        public string FailMessage { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ConfigCommandResult class constructor. </summary>
        public ConfigCommandResult()
        {
            //
        }

        #endregion CLASS METHODS

    }
}
