using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using YandexTolokaNotification.Services.Abstracts;
using YandexTolokaNotification.Services.Commands;

namespace YandexTolokaNotification.ModelView
{ 
        public class NotifyIconViewModel:BaseViewModel
        { 
            /// <summary>
            /// Shows a window, if none is already open.
            /// </summary>
            public ICommand ShowWindowCommand
            {
                get
                {
                    return new RelayCommand((object obj) =>
                    {
                        Application.Current.MainWindow = new Login();
                        Application.Current.MainWindow.Visibility = Visibility.Visible;
                    }, (object obj) => Application.Current.MainWindow.IsVisible==false);
                }
                
            }

            /// <summary>
            /// Hides the main window. This command is only enabled if a window is open.
            /// </summary>
            public ICommand HideWindowCommand
            {
            get
            {
                return new RelayCommand((object obj) => { Application.Current.MainWindow.Visibility = Visibility.Hidden; }, (object obj) =>  Application.Current.MainWindow != null);
  
                }
            }


            /// <summary>
            /// Shuts down the application.
            /// </summary>
            public ICommand ExitApplicationCommand
            {
                get
                {
                    return new RelayCommand( (object obj) => Application.Current.Shutdown() );
                }
            }
        }
    }

