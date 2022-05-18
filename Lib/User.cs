using System;

namespace Lib
{
    public class User
    {
        public User(int UserID = -1, string Username = "empty")
        {
            this.UserID = UserID;
            this.Username = Username;
        }

        public int UserID { get; set; }
        public string Username { get; set; }
    }
}
