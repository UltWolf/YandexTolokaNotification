using System;
using System.Collections.Generic;
using System.Text;

namespace YandexTolokaNotification.Model
{
     public  class StringWrapper
    {
        public StringWrapper(string s)
        {
            _value = s;
        }
        public string Value { get { return _value; } set { _value = value; } }
        string _value;
    }
}
