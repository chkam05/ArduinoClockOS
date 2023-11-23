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

        #region AUTHORIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Perform authroization. </summary>
        /// <param name="authService"> Authentication service interface. </param>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        protected async Task<BaseResponseModel<SessionDataModel>> AuthorizeAsync(IAuthService authService, string? accessToken)
        {
            try
            {
                if (authService is null)
                    throw new UnauthorizedException("Authentication service unavailable");

                return await authService.AuthorizeAsync(accessToken);
            }
            catch (UnauthorizedException exc)
            {
                return new BaseResponseModel<SessionDataModel>(exc.Message, exc.StatusCode);
            }
            catch (ProcessingException exc)
            {
                return new BaseResponseModel<SessionDataModel>(exc.Message, exc.StatusCode);
            }
            catch (Exception exc)
            {
                var errorMessage = $"{ERROR_MESSAGE}: {exc.Message}";

                return new BaseResponseModel<SessionDataModel>(errorMessage, StatusCodes.Status400BadRequest);
            }
        }

        #endregion AUTHORIZATION METHODS

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

        //  --------------------------------------------------------------------------------
        /// <summary> Process method async with authorization. </summary>
        /// <typeparam name="T"> Response data model. </typeparam>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="authService"> Authentication service interface. </param>
        /// <param name="func"> Method to process. </param>
        /// <returns> Task with response view model. </returns>
        protected async Task<BaseResponseModel<T>> ProcessAsyncWithAuthorization<T>(string? accessToken,
            IAuthService authService, Func<SessionDataModel, BaseResponseModel<T>> func) where T : class
        {
            var authorizationResponse = await AuthorizeAsync(authService, accessToken);

            if (authorizationResponse.IsSuccess && authorizationResponse.Content is SessionDataModel sessionDataModel)
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        return func(sessionDataModel);
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
            else
            {
                var errorMessages = authorizationResponse.ErrorMessages ?? new List<string> { ERROR_MESSAGE };
                var statusCode = authorizationResponse.StatusCode;
                return new BaseResponseModel<T>(errorMessages, statusCode);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Process task async with authorization. </summary>
        /// <typeparam name="T"> Response data model. </typeparam>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="authService"> Authentication service interface. </param>
        /// <param name="func"> Task to process. </param>
        /// <returns> Task with response view model. </returns>
        protected async Task<BaseResponseModel<T>> ProcessTaskAsyncWithAuthorization<T>(string? accessToken,
            IAuthService authService, Func<SessionDataModel, Task<BaseResponseModel<T>>> func) where T : class
        {
            var authorizationResponse = await AuthorizeAsync(authService, accessToken);

            if (authorizationResponse.IsSuccess && authorizationResponse.Content is SessionDataModel sessionDataModel)
            {
                try
                {
                    return await func(sessionDataModel);
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
            else
            {
                var errorMessages = authorizationResponse.ErrorMessages ?? new List<string> { ERROR_MESSAGE };
                var statusCode = authorizationResponse.StatusCode;
                return new BaseResponseModel<T>(errorMessages, statusCode);
            }
        }

        #endregion PROCESSING METHODS

    }
}
