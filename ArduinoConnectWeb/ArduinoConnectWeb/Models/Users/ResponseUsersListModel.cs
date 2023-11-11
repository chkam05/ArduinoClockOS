namespace ArduinoConnectWeb.Models.Users
{
    public class ResponseUsersListModel
    {

        //  VARIABLES

        public List<ResponseUsersListItemModel>? Users { get; set; }

    }

    public class ResponseUsersListItemModel
    {
        
        //  VARIABLES

        public string? Id { get; set; }
        public string? UserName { get; set; }

    }
}
