using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexTolokaNotificationTests.Data
{
    public class DataStringExtensionTest : IEnumerable<object[]>
    {
        public DataStringExtensionTest()
        {
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new List<string>() { "bla", "blas", "blasterOne", "Learging some one" },
                new List<string>(){"fifth", "bla", "blas", "i got it"}
            };
            yield return new object[] {
                new List<string>() { "blaf", "blast", "blasterOne", "Learging some one" },
                new List<string>(){"fifth", "bla", "blas", "i got it"}
            };

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
