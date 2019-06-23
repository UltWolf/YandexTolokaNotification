using System;
using System.Collections.Generic;
using System.Text;
using YandexTolokaNotification.Model.Enums;

namespace YandexTolokaNotification.Model
{
    [Serializable]
   public  class User
    {
        public string Email;
        public string Password;
        public UserStateLogin  UST;
        public User(string Email, string password,UserStateLogin state)
        {
            this.Email = Email;
            this.Password = password;
            this.UST = state;
        }
    }
}
