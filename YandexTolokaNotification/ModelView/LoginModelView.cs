using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AngleSharp.Html.Parser;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using YandexTolokaNotification.Model;
using YandexTolokaNotification.Model.Enums;
using YandexTolokaNotification.Services.Abstracts;
using YandexTolokaNotification.Services.Commands;
using YandexTolokaNotification.Views;

namespace YandexTolokaNotification.ModelView
{
    public class LoginModelView : BaseViewModel
    {
        private string _email = "Please input your email";
        private string _password = "Please input your password";
        private Login _view;
        private List<string> proxys;
        private Random r = new Random();
        private UserStateLogin _stateLogin;
        private BinaryFormatter bf = new BinaryFormatter();
        public string Email { get => _email; set { _email = value; OnPropertyChanged("Email"); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged("Password"); } }

         public UserStateLogin StateLogin { get=>_stateLogin; set { _stateLogin = value;OnPropertyChanged("StateLogin"); } }
        public ICommand LoginCommand { get; set; } 


        public LoginModelView(Login view)
        {
            proxys = GetProxyServers();
            LoginCommand = new RelayCommand(o => Login(new object())); 
            InitiateUser();
            _view = view;

        }
        
        private void InitiateUser()
        {
            if (File.Exists("user.data"))
            {
                try
                {
                    using (FileStream fs = new FileStream("user.data", FileMode.Open))
                    {
                        User user = (User)bf.Deserialize(fs);
                        Email = user.Email;
                        Password = user.Password;
                        StateLogin = user.UST;
                    }
                } catch (Exception ex)
                {

                }
            }
        }
       
        public void Login(object obj)
        { 
            int index;
            if (obj is int) {
                index = (int)obj;
            }
            else
            {
                index = 0;
            }
           
            try
            {
                FirefoxOptions options = new FirefoxOptions();
                if (index < proxys.Count)
                {
                    Proxy proxy = new Proxy();
                    proxy.Kind = ProxyKind.Manual;
                    proxy.IsAutoDetect = false;
                    proxy.SslProxy = proxys[index];
                    options.Proxy = proxy;

                    FirefoxDriver driver = new FirefoxDriver(FirefoxDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(5));
                    LoginInAccount(driver);
                    TaskViews tw =  new TaskViews(driver) ;
                    tw.Show();
                    _view.Close();
                }
                
            }
            catch (Exception ex)
            {
                Login(++index);
            }
            


        }
        public void LoginInAccount(FirefoxDriver driver)
        {
            try
            {
                User user = new User(_email, _password, StateLogin);

                using (FileStream fs = new FileStream("user.data", FileMode.Create))
                {
                    bf.Serialize(fs, user);
                }
                By header_login_but = By.ClassName("header-login");
                driver.Navigate().GoToUrl("https://toloka.yandex.ru/");
                WaitUntilElementClickable(driver, header_login_but);
                driver.FindElement(header_login_but).Click();
                if (((int)StateLogin) == 0)
                {
                    GoogleAuth(driver);
                }
                else if (((int)StateLogin) == 2)
                {
                    LoginBySelf(driver);
                }
            }catch(Exception ex)
            {
                driver.Quit();

                throw ;
            }

        }
        private void LoginBySelf(FirefoxDriver driver)
        {
            WaitUntilElementVisible(driver, By.ClassName("passp-content"));
            var wrap = driver.FindElement(By.ClassName("passp-content"));
            var wrapForContent = wrap.FindElement(By.ClassName("passp-auth-content"));
            var wrapForPassword = wrapForContent.FindElement(By.ClassName("passp-login-form"));
            var wrapInsideForm = wrapForPassword.FindElement(By.ClassName("passp-form-field"));
            var wrapInput = wrapInsideForm.FindElement(By.ClassName("passp-form-field__input"));
            wrapInput.FindElement(By.TagName("input")).SendKeys(_email);
            driver.FindElementByClassName("passp-sign-in-button")
                .FindElements(By.TagName("button"))[0].Click();
            var passwordInput = By.Id("passp-field-passwd");
            WaitUntilElementClickable(driver, passwordInput);
            driver.FindElement(passwordInput).SendKeys(_password);
            driver.FindElement(By.ClassName("passp-sign-in-button")).FindElement(By.TagName("button")).Click();
            WaitUntilElementVisible(driver, By.ClassName("snippets"));
        }
        private void GoogleAuth(FirefoxDriver driver)
        {
            try { 
                
                By formGroup = By.ClassName("form-group");
                By inputTag = By.TagName("input"); 
                By GoogleAuthDiv = By.ClassName("passp-social-block__list-item");
                 
                WaitUntilElementClickable(driver, GoogleAuthDiv);
                driver.FindElements(GoogleAuthDiv)[2].Click();
                var windows = driver.WindowHandles.ToList();
                driver.SwitchTo().Window(windows[1]);

                var emailId = By.Id("identifierId");
                var BlockWithInputs = By.Id("initialView");
                WaitUntilElementVisible(driver, BlockWithInputs, 120);
                var MatchWithId = driver.FindElements(emailId);
                var inputWithEmail = MatchWithId.FirstOrDefault(m => m.TagName == "input");
                inputWithEmail.SendKeys(_email);

                driver.FindElement(By.Id("identifierNext")).Click();
                Thread.Sleep(1200);
                if (driver.FindElement(By.Id("captchaimg")).GetProperty("src") != null)
                {
                    MessageBox.Show("Вылезла капча, пожалуйста откройте браузер и введите её и нажмите продолжить, и только потом закройте это окно.");
                }
                var inputs = driver.FindElements(inputTag);

                foreach (var i in inputs)
                {
                    if (i.GetAttribute("name") == "password")
                    {

                        driver.FindElements(inputTag)[1].SendKeys(_password);

                        driver.FindElement(By.Id("passwordNext")).Click();
                        Thread.Sleep(r.Next(4, 10) * 1000);
                        if (driver.WindowHandles.ToList().Count > 2)
                        {
                            MessageBox.Show("Input code from Gmail in browser");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                driver.Quit();
                throw new Exception("Exception", ex);
            }
           
        }
        public IWebElement WaitUntilElementClickable(FirefoxDriver driver, By elementLocator, int timeout = 60)
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
        public IWebElement WaitUntilElementVisible(FirefoxDriver driver, By elementLocator, int timeout = 60)
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

        public List<string> GetProxyServers()
        {
            List<String> proxyList = new List<string>();
            var httpRequest = (HttpWebRequest)HttpWebRequest.Create("https://www.proxynova.com/proxy-server-list/country-ru/");
            httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            httpRequest.KeepAlive = true;
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0";

            var httpResponse = httpRequest.GetResponse();
            string html = "";
            using (Stream stream = httpResponse.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    html = sr.ReadToEnd();
                }
            }
            var parser = new HtmlParser();
            var body = parser.ParseDocument(html);
            var table = body.GetElementById("tbl_proxy_list");

            var trs = table.GetElementsByTagName("tr");
            for (int i = 1; i < trs.Length; i++)
            {
                var tds = trs[i].GetElementsByTagName("td");
                if (tds.Length > 0)
                {
                    var proxyScript = tds[0].GetElementsByTagName("script");
                    if (proxyScript.Length != 0)
                    {
                        var arrayWithData = proxyScript[0].TextContent.Split("\'");
                        if (arrayWithData.Length > 4)
                        {
                            var numb = arrayWithData[1].Substring(8);
                            var lastDataNumbs = arrayWithData[3];
                            string http = numb + lastDataNumbs;
                            string port = tds[1].TextContent.Replace("\t", "").Replace("\n", "").Replace(" ", "");
                            proxyList.Add(http + ":" + port);
                        }
                    }
                }
                

            }
            return proxyList;
        }
    }
}

