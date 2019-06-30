using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System; 
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using YandexTolokaNotification.Services.Abstracts;
using YandexTolokaNotification.Services.Commands;

namespace YandexTolokaNotification.ModelView
{
    public class TaskNotificationModelView : BaseViewModel
    {
        private FirefoxDriver _driver;
        private object _locker = new object();
        private ObservableCollection<string> _fullTask  = new ObservableCollection<string>();
        private ObservableCollection<string> _needlyTask = new ObservableCollection<string>();
        private string _customTitle = "Нет текущей задачи? Не беда, добавьте её сами.";
        private string _choosenFullTask;
        private Thread ThreadSearch;
        private string _choosenNeedTask;
        private Boolean _clock = true;
        private BinaryFormatter bf = new BinaryFormatter();
        public ObservableCollection<string> FullTasks { get => _fullTask; set { _fullTask = value; OnPropertyChanged("FullTasks"); } }

        public ObservableCollection<string> NeedlyTask { get => _needlyTask; set { _needlyTask = value; OnPropertyChanged("NeedlyTask"); } }
        public string CustomTitle { get => _customTitle; set { _customTitle = value; OnPropertyChanged("CustomTitle"); } }
        public ICommand ObservableCollectionenCommand { get; set; }
        public ICommand AddCustomCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand RemoveTaskCommand { get; set; }
        public ICommand GetFullTasksCommand { get; set; }
        public ICommand StopObservableCollectionenCommand { get; set; }
        public ICommand SaveNeedlyCommand { get; set; }
        public string ChoosenNeedTask { get => _choosenNeedTask; set { _choosenNeedTask = value; OnPropertyChanged("ChoosenNeedTask"); } }
        public string ChoosenFullTask { get => _choosenFullTask; set { _choosenFullTask = value; OnPropertyChanged("ChoosenFullTask"); } }
        private static object _syncLock = new object();


        public TaskNotificationModelView(FirefoxDriver driver)
        {
            _driver = driver;
            ObservableCollectionenCommand = new RelayCommand(ObservableCollectionen);
            StopObservableCollectionenCommand = new RelayCommand(StopObservableCollectionening);
            AddCustomCommand = new RelayCommand(AddCustom);
            AddTaskCommand = new RelayCommand(AddToNeed);
            RemoveTaskCommand = new RelayCommand(RemoveFromNeed);
            GetFullTasksCommand = new RelayCommand(UpdateObservableCollection);
            SaveNeedlyCommand = new RelayCommand(SaveNeedlyTasks);
            BindingOperations.EnableCollectionSynchronization(FullTasks, _syncLock);
            BindingOperations.EnableCollectionSynchronization(NeedlyTask, _syncLock); 
            lock (_syncLock)
            {
                Thread thr = new Thread(getNeedlyTask);
                thr.Start();
            }
        }

        public void SaveNeedlyTasks(object obj)
        { lock (_syncLock)
            {
                Task task = new Task(() =>
                {
                    using (FileStream fs = new FileStream("tasks.data", FileMode.Create, FileAccess.Write))
                    {
                        bf.Serialize(fs, NeedlyTask);
                        MessageBox.Show("Tasks has been saved successfull");
                    }
                }
                );
                task.Start();
            }
        }
        private void getNeedlyTask()
        {
            lock (_syncLock)
            {
                if (File.Exists("tasks.data"))
                {
                    using (FileStream fs = new FileStream("tasks.data", FileMode.Open, FileAccess.Read))
                    {
                        NeedlyTask = (ObservableCollection<string>)bf.Deserialize(fs);
                    }
                }
            }
        }
        public void StopObservableCollectionening(object obj)
        {
            Monitor.Enter(_locker);
            _clock = false;
            Monitor.Exit(_locker);
        }
        public void UpdateObservableCollection(object obj)
        {
             ObservableCollection<string> newTasks = GetTasks();
            Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
              new Action(() => { FullTasks = newTasks; })); 
            
        }
         
        private ObservableCollection<string> GetTasks()
        {
            ObservableCollection<string> tasks = new ObservableCollection<string>();
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
            lock (_syncLock)
            {
                Parallel.ForEach<IWebElement>(lis,
                new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                (lis, state) =>
            {
                string nameOfTask = GetTask(lis);
                bool IsContains = NeedlyTask.Contains(nameOfTask);
                if (IsContains)
                {
                    result = Tuple.Create(true, nameOfTask);
                    state.Break();

                }
            }
            );
            }
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
            Thread thr = new Thread(InitializeObservableCollection);
            thr.Start();
        }
        private void InitializeObservableCollection()
        {
            lock (_syncLock)
            {
                FullTasks = GetTasks();
            }
        }
        private void AddToNeed(object obj)
        {
            lock (_syncLock)
            {
                Application.Current.Dispatcher.BeginInvoke(
                     DispatcherPriority.Background,
                 new Action(() => {

                     this.NeedlyTask.Add(ChoosenFullTask);
                     this.FullTasks.Remove(ChoosenFullTask);
                 }));
            }
            UpdateObservableCollection();

        }
     
        private void RemoveFromNeed(object obj)
        {
            lock (_syncLock)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                new Action(() => {
                    this.FullTasks.Add(ChoosenNeedTask);
                this.NeedlyTask.Remove(ChoosenNeedTask);
                }));
            }
            UpdateObservableCollection();
           
        }
        private void UpdateObservableCollection()
        {
            OnPropertyChanged("NeedlyTask");

            OnPropertyChanged("FullTasks");
        }
        public void AddCustom(object obj)
        {
            lock (_syncLock)
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                    new Action(() =>
                    {
                        _fullTask.Add(new string(CustomTitle));
                    }));
                OnPropertyChanged("FullTasks");
            }
        }
        public void ObservableCollectionen(object obj)
        {
            try
            {
                ThreadStart threadStart = new ThreadStart(ObservableCollectionenCycle);
                this.ThreadSearch = new Thread(threadStart);
                this.ThreadSearch.Start();
            }
            catch (ThreadAbortException)
            {
                MessageBox.Show("Thread is cancel");
            }
        }
        private void ObservableCollectionenCycle()
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
                    MessageBox.Show("ObservableCollectionen have been canceled");
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
