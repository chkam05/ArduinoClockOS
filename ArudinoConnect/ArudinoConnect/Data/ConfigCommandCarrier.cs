using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class ConfigCommandCarrier
    {

        //  VARIABLES

        public string Command { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string CompleteMessage { get; set; }
        public string FailMessage { get; set; }
        public PackIconKind Icon { get; set; }
        public string RequiredResponse { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ConfigurationUploadCarrier class constructor. </summary>
        public ConfigCommandCarrier()
        {
            //
        }

        #endregion CLASS METHODS

    }
}
