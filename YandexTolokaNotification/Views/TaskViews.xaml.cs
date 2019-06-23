using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YandexTolokaNotification.ModelView;

namespace YandexTolokaNotification.Views
{
    /// <summary>
    /// Interaction logic for TaskViews.xaml
    /// </summary>
    public partial class TaskViews : Window
    {
        public TaskViews(FirefoxDriver driver)
        {
            InitializeComponent();
            this.DataContext = new TaskNotificationModelView(driver);
        }
         
    }
}
