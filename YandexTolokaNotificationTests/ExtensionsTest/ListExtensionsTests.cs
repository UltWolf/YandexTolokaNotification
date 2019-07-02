using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using YandexTolokaNotificationTests.Data;
using YandexTolokaNotification.Extensions;
using System.Collections.ObjectModel;

namespace YandexTolokaNotificationTests.ExtensionsTest
{
    public class ListExtensionsTests
    {
        [Theory]
        [ClassData(typeof(DataStringExtensionTest))]
        public void ReturnNotNull(ObservableCollection<string> value1, ObservableCollection<string> value2)
        {
            Assert.NotNull(value1.GetCompareElement(value2));
        }
    }
}
