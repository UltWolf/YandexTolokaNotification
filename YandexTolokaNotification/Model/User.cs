using System;
using System.Collections.Generic;
using System.Text;

namespace YandexTolokaNotification.Model
{
    [Serializable]
   public  class User
    {
        public string Email;
        public string Password;
        public User(string Email, string password)
        {
            this.Email = Email;
            this.Password = password;
        }
    }
}
