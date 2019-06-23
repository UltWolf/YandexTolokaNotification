using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YandexTolokaNotification.Model.Enums
{
    [Serializable]
    public enum UserStateLogin
    {
        [XmlEnum("0")]
        GoogleAuth = 0,
        [XmlEnum("2")]
        AuthBySelf = 2,
        [XmlEnum("4")]
        CookieAuth = 4,
        [XmlEnum("8")]
        FaceBookAuth = 8

    }
}
