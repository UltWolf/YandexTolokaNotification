using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using YandexTolokaNotification.Extensions;
using YandexTolokaNotification.Model;
using YandexTolokaNotification.Services.Abstracts;
using YandexTolokaNotification.Services.Commands;

namespace YandexTolokaNotification.ModelView
{
    public class TaskNotificationModelView : BaseViewModel
    {
        private FirefoxDriver _driver;
        private List<StringWrapper> _fullTask ;
        private List<StringWrapper> _needlyTask = new List<StringWrapper>();
        private string _customTitle = "Нет текущей задачи? Не беда, добавьте её сами.";
        private StringWrapper _choosenFullTask;
        private StringWrapper _choosenNeedTask;
        private Boolean _clock;
        public List<StringWrapper> FullTasks { get => _fullTask; set { _fullTask = value; OnPropertyChanged("FullTasks"); } }

        public List<StringWrapper> NeedlyTask { get => _needlyTask; set { _needlyTask = value; OnPropertyChanged("NeedlyTask"); } }
        public string CustomTitle { get => _customTitle; set { _customTitle = value; OnPropertyChanged("CustomTitle"); } }
        public ICommand ListenCommand { get; set; }
        public ICommand AddCustomCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand RemoveTaskCommand { get; set; }
        public StringWrapper ChoosenNeedTask { get => _choosenNeedTask; set { _choosenNeedTask = value; OnPropertyChanged("ChoosenNeedTask"); } }
        public StringWrapper ChoosenFullTask { get => _choosenFullTask; set { _choosenFullTask = value; OnPropertyChanged("ChoosenFullTask"); } }
   

        public TaskNotificationModelView(FirefoxDriver driver)
        {
            _driver = driver;
            ListenCommand = new RelayCommand(Listen);
            InitializeList();
        }
        private List<StringWrapper> GetTasks()
        {
            List<StringWrapper> tasks = new List<StringWrapper>();
            var lis =  _driver.FindElementsByClassName("tutorial-tasks-page__snippets");
            foreach(var li in lis)
            {
                
                tasks.Add(new StringWrapper(li.FindElement(By.ClassName("snippet__title")).Text));
            } 
            return tasks;
        }
        private void InitializeList()
        {
            FullTasks = GetTasks();
        }
        private void AddToNeed()
        {
            this.FullTasks.Remove(ChoosenFullTask);
            this.NeedlyTask.Add(ChoosenFullTask);

        }
        private void RemoveFromNeed()
        {
            this.FullTasks.Add(ChoosenNeedTask);
            this.NeedlyTask.Remove(ChoosenNeedTask);
        }
        public void AddCustom(object obj)
        {
            _fullTask.Add(new StringWrapper(CustomTitle));
        }
        public void Listen(object obj)
        {
             
            while (_clock){

                List<StringWrapper> Tasks = GetTasks();
                var compareTasks =  NeedlyTask.GetCompareElement(Tasks);
                if (compareTasks.Count>0)
                { 
                    MessageBox.Show("We get nextElements:" + JsonConvert.SerializeObject(compareTasks));
                }
                Monitor.Enter(_clock);
                if (_clock == false)
                {
                    break;
                }
                Monitor.Exit(_clock);
            }
        }

    }
}
