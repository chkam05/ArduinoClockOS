using ArduinoConnectWeb.Models.Users;
using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Base
{
    public class BaseUniqueDataModel
    {

        //  VARIABLES

        public string Id { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseUniqueDataModel class constructor. </summary>
        /// <param name="id"> Identifier. </param>
        [JsonConstructor]
        public BaseUniqueDataModel(string? id)
        {
            Id = !string.IsNullOrEmpty(id) ? id : GetNewIdentifier();
        }

        #endregion CLASS METHODS

        #region OVERWRITTEN OPERATOR METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Determines whether the specified object is equal to the current object. </summary>
        /// <param name="obj"> Object to compare. </param>
        /// <returns> True - if the specified object is equal to the current object; False - otherwise. </returns>
        public override bool Equals(object? obj)
        {
            if (obj is BaseUniqueDataModel userDataModel)
            {
                return Id == userDataModel.Id;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serves as the default hash function. </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if the objects are equal. </summary>
        /// <param name="left"> Left side object. </param>
        /// <param name="right"> Right side object. </param>
        /// <returns> True - objects are equal; False - otherwise. </returns>
        public static bool operator ==(BaseUniqueDataModel left, BaseUniqueDataModel right)
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
        public static bool operator !=(BaseUniqueDataModel left, BaseUniqueDataModel right)
        {
            return !(left == right);
        }

        #endregion OVERWRITTEN OPERATOR METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get new identifier. </summary>
        /// <returns> New identifier. </returns>
        private string GetNewIdentifier()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        #endregion UTILITY METHODS

    }
}
