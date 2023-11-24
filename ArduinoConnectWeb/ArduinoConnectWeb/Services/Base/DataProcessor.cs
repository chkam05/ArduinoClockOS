using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Services.Auth;

namespace ArduinoConnectWeb.Services.Base
{
    public class DataProcessor
    {

        //  CONST

        public const string ERROR_MESSAGE = "An unknown error occurred while processing data";


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> DataProcessor class constructor. </summary>
        public DataProcessor()
        {
            //
        }

        #endregion CLASS METHODS

        #region PROCESSING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Process method async. </summary>
        /// <typeparam name="T"> Response data model. </typeparam>
        /// <param name="func"> Method to process. </param>
        /// <returns> Task with response view model. </returns>
        protected async Task<BaseResponseModel<T>> ProcessAsync<T>(Func<BaseResponseModel<T>> func) where T : class
        {
            return await Task.Run(() =>
            {
                try
                {
                    return func();
                }
                catch (UnauthorizedException uexc)
                {
                    return new BaseResponseModel<T>(uexc.Message, uexc.StatusCode);
                }
                catch (ProcessingException pexc)
                {
                    return new BaseResponseModel<T>(pexc.Message, pexc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"{ERROR_MESSAGE}: {exc.Message}";
                    return new BaseResponseModel<T>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Process task async. </summary>
        /// <typeparam name="T"> Response data model. </typeparam>
        /// <param name="func"> Task to process. </param>
        /// <returns> Task with response view model. </returns>
        protected async Task<BaseResponseModel<T>> ProcessTaskAsync<T>(Func<Task<BaseResponseModel<T>>> func) where T : class
        {
            try
            {
                return await func();
            }
            catch (UnauthorizedException uexc)
            {
                return new BaseResponseModel<T>(uexc.Message, uexc.StatusCode);
            }
            catch (ProcessingException pexc)
            {
                return new BaseResponseModel<T>(pexc.Message, pexc.StatusCode);
            }
            catch (Exception exc)
            {
                var errorMessage = $"{ERROR_MESSAGE}: {exc.Message}";
                return new BaseResponseModel<T>(errorMessage, StatusCodes.Status400BadRequest);
            }
        }

        #endregion PROCESSING METHODS

    }
}
