using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace YandexTolokaNotification.Extensions
{
    public static class ObservableCollectionExtension
    {
       public static ObservableCollection<T> GetCompareElement<T>(this ObservableCollection<T> baseObservableCollection, ObservableCollection<T> outerObservableCollection)
        {
            ObservableCollection<T> ComparedElements = new ObservableCollection<T>();
            foreach(var l in baseObservableCollection)
            {
                foreach(var outer in outerObservableCollection)
                {
                    if (l.Equals(outer))
                    {
                        ComparedElements.Add(outer);
                    }
                }
            }
            return ComparedElements;
        }
        public static ObservableCollection<T> RemoveCompareElement<T>(this ObservableCollection<T> baseObservableCollection, ObservableCollection<T> outerObservableCollection)
        {
            
            baseObservableCollection.Remove(baseObservableCollection.GetCompareElement(outerObservableCollection));
            return baseObservableCollection;
        }
        public static void Remove<T>(this ObservableCollection<T> baseObservableCollection, ObservableCollection<T> outerObservableCollection)
        {
            foreach(var outt in outerObservableCollection){
                baseObservableCollection.Remove(outerObservableCollection);
            }

        }
        }
}
