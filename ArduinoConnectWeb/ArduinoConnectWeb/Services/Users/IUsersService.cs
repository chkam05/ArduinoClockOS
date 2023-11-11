using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Models.Users;

namespace ArduinoConnectWeb.Services.Users
{
    public interface IUsersService
    {

        //  METHODS

        public Task<ResponseBaseModel<ResponseUserModel>> GetUserById(string id);
        public Task<ResponseBaseModel<ResponseUserModel>> GetUserByUserName(string userName);
        public Task<ResponseBaseModel<ResponseUsersListModel>> GetUsersList();

    }
}
