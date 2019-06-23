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
using YandexTolokaNotification.Services.Abstracts;
using YandexTolokaNotification.Services.Commands;

namespace YandexTolokaNotification.ModelView
{
    public class TaskNotificationModelView : BaseViewModel
    {
        private FirefoxDriver _driver;
        private List<string> _fullTask = new List<string>();
        private List<string> _needlyTask;
        private string _customTitle;
        private string _choosenFullTask;
        private string _choosenNeedTask;
        private Boolean _clock;
        public List<string> FullTasks { get => _fullTask; set { _fullTask = value; OnPropertyChanged("FullTasks"); } }

        public List<string> NeedlyTask { get => _needlyTask; set { _needlyTask = value; OnPropertyChanged("NeedlyTask"); } }
        public string CustomTitle { get => _customTitle; set { _customTitle = value; OnPropertyChanged("CustomTitle"); } }
        public ICommand ListenCommand { get; set; }
        public ICommand AddCustomCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand RemoveTaskCommand { get; set; }
        public string ChoosenNeedTask { get => _choosenNeedTask; set { _choosenNeedTask = value; OnPropertyChanged("ChoosenNeedTask"); } }
        public string ChoosenFullTask { get => _choosenFullTask; set { _choosenFullTask = value; OnPropertyChanged("ChoosenFullTask"); } }
   

        public TaskNotificationModelView(FirefoxDriver driver)
        {
            _driver = driver;
            ListenCommand = new RelayCommand(Listen);
            InitializeList();
        }
        private List<string> GetTasks()
        {
            List<string> tasks = new List<string>();
            var lis =  _driver.FindElementsByClassName("tutorial-tasks-page__snippets");
            foreach(var li in lis)
            {
                tasks.Add(li.FindElement(By.ClassName("snippet__title")).Text);
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
            _fullTask.Add(CustomTitle);
        }
        public void Listen(object obj)
        {
             
            while (_clock){

                List<string> Tasks = GetTasks();
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
