using System;
using System.Collections.Generic;
using System.Text;

namespace YandexTolokaNotification.Extensions
{
    public static class ListExtension
    {
       public static List<T> GetCompareElement<T>(this List<T> baseList, List<T> outerList)
        {
            List<T> ComparedElements = new List<T>();
            foreach(var l in baseList)
            {
                foreach(var outer in outerList)
                {
                    if (l.Equals(outer))
                    {
                        ComparedElements.Add(outer);
                    }
                }
            }
            return ComparedElements;
        }
    }
}
