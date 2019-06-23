using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
 
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
        }

         
    }
}
