using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using YandexTolokaNotificationTests.Data;
using YandexTolokaNotification.Extensions;

namespace YandexTolokaNotificationTests.ExtensionsTest
{
    public class ListExtensionsTests
    {
        [Theory]
        [ClassData(typeof(DataStringExtensionTest))]
        public void ReturnNotNull(List<string> value1, List<string> value2)
        {
            Assert.NotNull(value1.GetCompareElement(value2));
        }
    }
}
