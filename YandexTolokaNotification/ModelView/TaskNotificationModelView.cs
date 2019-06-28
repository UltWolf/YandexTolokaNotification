using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private object _locker = new object();
        private List<string> _fullTask  = new List<string>();
        private List<string> _needlyTask = new List<string>();
        private string _customTitle = "Нет текущей задачи? Не беда, добавьте её сами.";
        private string _choosenFullTask;
        private Thread ThreadSearch;
        private string _choosenNeedTask;
        private Boolean _clock = true;
        private BinaryFormatter bf = new BinaryFormatter();
        public List<string> FullTasks { get => _fullTask; set { _fullTask = value; OnPropertyChanged("FullTasks"); } }

        public List<string> NeedlyTask { get => _needlyTask; set { _needlyTask = value; OnPropertyChanged("NeedlyTask"); } }
        public string CustomTitle { get => _customTitle; set { _customTitle = value; OnPropertyChanged("CustomTitle"); } }
        public ICommand ListenCommand { get; set; }
        public ICommand AddCustomCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand RemoveTaskCommand { get; set; }
        public ICommand GetFullTasksCommand { get; set; }
        public ICommand StopListenCommand { get; set; }
        public ICommand SaveNeedlyCommand { get; set; }
        public string ChoosenNeedTask { get => _choosenNeedTask; set { _choosenNeedTask = value; OnPropertyChanged("ChoosenNeedTask"); } }
        public string ChoosenFullTask { get => _choosenFullTask; set { _choosenFullTask = value; OnPropertyChanged("ChoosenFullTask"); } }
   

        public TaskNotificationModelView(FirefoxDriver driver)
        {
            _driver = driver;
            ListenCommand = new RelayCommand(Listen);
            StopListenCommand = new RelayCommand(StopListening);
            AddCustomCommand = new RelayCommand(AddCustom);
            AddTaskCommand = new RelayCommand(AddToNeed);
            RemoveTaskCommand = new RelayCommand(RemoveFromNeed);
            GetFullTasksCommand = new RelayCommand(UpdateList);
            SaveNeedlyCommand = new RelayCommand(SaveNeedlyTasks);
            InitializeList();
            Thread thr = new Thread(getNeedlyTask);
            thr.Start();
            
        }

        public void SaveNeedlyTasks(object obj)
        {
            Task task = new Task(() =>
            {
                using (FileStream fs = new FileStream("tasks.data", FileMode.Create, FileAccess.Write))
                {
                    bf.Serialize(fs, NeedlyTask);
                }
            }
            );
            task.Start();
        }
        private void getNeedlyTask()
        {
            if (File.Exists("tasks.data"))
            {
                using (FileStream fs = new FileStream("tasks.data", FileMode.Open, FileAccess.Read))
                {
                   NeedlyTask= (List<string>) bf.Deserialize(fs);
                }
            }
        }
        public void StopListening(object obj)
        {
            Monitor.Enter(_locker);
            _clock = false;
            Monitor.Exit(_locker);
        }
        public void UpdateList(object obj)
        {
            Thread thread = new Thread(AddTasks);
            thread.Start();
        }
        private void AddTasks()
        {
            FullTasks = new List<string>();
            var lis =  _driver.FindElementsByClassName("tutorial-tasks-page__snippets");
            Parallel.ForEach(lis, (lis)=> {
                string task = GetTask(lis);
                if (task != "")
                {
                    FullTasks.Add(task);
                }
            }
            ); 
        } 
        private List<string> GetTasks()
        {
            List<string> tasks = new List<string>();
            var lis = _driver.FindElementsByClassName("tutorial-tasks-page__snippets");
            Parallel.ForEach(lis, (lis) =>
            {
                string task = GetTask(lis);
                if (task != "")
                {
                    tasks.Add(task);
                }
                }
            );
            return tasks;
        }
        private Tuple<bool, string> FindTask()
        {

            var result = Tuple.Create(false,"");
            var lis = _driver.FindElementsByClassName("tutorial-tasks-page__snippets");
            Parallel.ForEach<IWebElement>(lis,
                new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                (lis,state) =>
            {
                string nameOfTask = GetTask(lis);
                bool IsContains = NeedlyTask.Contains(nameOfTask);
                if (IsContains)
                {
                    result = Tuple.Create(true,nameOfTask);
                    state.Break();
                    
                }
            }
            );
            return result;
        }
        private string GetTask(IWebElement element)
        {
            if (element.FindElements(By.TagName("div"))[0].GetAttribute("class").Contains("snippet snippet_not-available")){
                return "";
            }
            else
            {
                return element.FindElement(By.ClassName("snippet__title")).Text;
            }
        }
        public void Initialize(object obj)
        {
            Thread thr = new Thread(InitializeList);
            thr.Start();
        }
        private void InitializeList()
        {
            FullTasks = GetTasks();
        }
        private void AddToNeed(object obj)
        {
            this.FullTasks.Remove(ChoosenFullTask);
            this.NeedlyTask.Add(ChoosenFullTask);
            UpdateLists();

        }
        private void UpdateLists()
        {
            OnPropertyChanged("NeedlyTask");
            OnPropertyChanged("FullTasks");
        }
        private void RemoveFromNeed(object obj)
        {
            this.FullTasks.Add(ChoosenNeedTask);
            this.NeedlyTask.Remove(ChoosenNeedTask);
            UpdateLists();
        }
        public void AddCustom(object obj)
        {
            _fullTask.Add(new string(CustomTitle));
            UpdateLists();
        }
        public void Listen(object obj)
        {
            try
            {
                ThreadStart threadStart = new ThreadStart(ListenCycle);
                this.ThreadSearch = new Thread(threadStart);
                this.ThreadSearch.Start();
            }
            catch (ThreadAbortException)
            {
                MessageBox.Show("Thread is cancel");
            }
        }
        private void ListenCycle()
        {
            try
            {
                _driver.Navigate().Refresh();
            }
            catch (Exception)
            {
                try
                {
                    _driver.Navigate().Refresh();
                }
                catch (Exception)
                {

                }
            }
            WaitUntilElementVisible(_driver, By.ClassName("snippets"));
            while (_clock)
            {

                var result = FindTask();
                Monitor.Enter(_locker);
                if (_clock == false)
                {
                    _clock = true;
                    MessageBox.Show("Listen have been canceled");
                    Monitor.Exit(_locker);
                    break;
                }
                if(result.Item1==true)
                {
                    MessageBox.Show("Update page, you got a task: "+result.Item2);
                }

                Monitor.Exit(_locker);
               
                int seconds = 30;
                int milliseconds = 1000;
                try
                {
                    _driver.Navigate().Refresh();
                }
                catch (Exception  )
                {
                    try
                    {
                        _driver.Navigate().Refresh();
                    }
                    catch(Exception  )
                    {
                        
                    }
                    }
                Thread.Sleep(seconds * milliseconds);
            }
        }
        public IWebElement WaitUntilElementClickable(FirefoxDriver driver, By elementLocator, int timeout = 160)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(elementLocator));
            }
            catch (NoSuchElementException)
            {

                throw;
            }
        }
        public IWebElement WaitUntilElementVisible(FirefoxDriver driver, By elementLocator, int timeout = 160)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(elementLocator));
            }
            catch (NoSuchElementException)
            {

                throw;
            }
        }
    }
}
