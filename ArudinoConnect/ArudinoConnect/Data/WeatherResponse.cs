using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class WeahterResponse
    {

        //  VARIABLES

        public WeatherData Data { get; set; }
        public string Error { get; set; }
        public bool Success { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeahterResponse class constructor. </summary>
        /// <param name="success"> Base configuration of successed response information. </param>
        public WeahterResponse(bool success = false)
        {
            Success = success;
        }

        #endregion CLASS METHODS

    }
}
