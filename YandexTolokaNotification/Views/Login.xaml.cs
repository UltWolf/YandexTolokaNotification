using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YandexTolokaNotification.ModelView;

namespace YandexTolokaNotification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public  partial class Login : Window
    { 
        public Login()
        {
          
            InitializeComponent();
            this.DataContext = new LoginModelView(this);
            Uri tolokaUri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Toloka.png", UriKind.Absolute);
            Toloka.Source = new BitmapImage(tolokaUri);
            Uri googleUri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Google.png", UriKind.Absolute);
            Google.Source = new BitmapImage(googleUri);
        }

         
    }
}
