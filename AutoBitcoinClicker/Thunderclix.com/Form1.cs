﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Thunderclix.com
{
    public partial class Form1 : Form
    {
        private IWebDriver driver = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnClick_Click(object sender, EventArgs e)
        {
            try
            {
                string[] arguments = new string[] { "no-sandbox", "test-type", "ignore-certificate-errors", "disable-popup-blocking" };
                var chromeOptions = new ChromeOptions() { BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" };
                chromeOptions.AddArguments(arguments);

                driver = new ChromeDriver(@"chromedriver_win32\", chromeOptions, new System.TimeSpan(10, 0, 0));
                
                if (driver != null)
                {
                    driver.Url = "https://thunderclix.com/login.aspx";

                    var userName = driver.FindElement(By.Id("ctl00_MainContentPlaceHolder_ctl00_Username"));
                    userName.SendKeys("username");

                    var password = driver.FindElement(By.Id("ctl00_MainContentPlaceHolder_ctl00_Password"));
                    password.SendKeys("password");

                    var btnLogin = driver.FindElement(By.Id("ctl00_MainContentPlaceHolder_ctl00_LoginButton"));
                    btnLogin.Click();

                    driver.Url = "https://thunderclix.com/user/earn/coinhiveclaim.aspx";

                    int count = 0;

                    while (count < 10000)
                    {
                        IList<IWebElement> iframe = driver.FindElements(By.TagName("iframe"));

                        foreach (var item in iframe)
                        {
                            if (item.Size.Height == 78 && item.Size.Width == 304)
                            {
                                driver.SwitchTo().Frame(item);
                                break;
                            }
                        }

                        driver.FindElement(By.Id("verify-me")).Click();

                        while (driver.FindElement(By.ClassName("error-text")).Text == "reload")
                        {
                            driver.FindElement(By.LinkText("reload")).Click();
                            driver.FindElement(By.Id("verify-me")).Click();
                            Thread.Sleep(2000);
                        }

                        var progress = driver.FindElement(By.ClassName("verified-text")).Text;
                        while (progress != "verified")
                        {
                            progress = driver.FindElement(By.ClassName("verified-text")).Text;
                        }

                        driver.SwitchTo().DefaultContent();

                        var request = driver.FindElement(By.Id("ctl00_PageMainContent_CoinhiveClaim_ClaimCoinhiveButton"));
                        request.Click();

                        count++;
                    }
                }

                driver.Close();
                driver.Quit();

                //run the program again and close this one
                Process.Start(Application.StartupPath + "\\Thunderclix.com.exe");
                //or you can use Application.ExecutablePath
                //close this one
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception)
            {
                driver.Close();
                driver.Quit();

                //run the program again and close this one
                Process.Start(Application.StartupPath + "\\Thunderclix.com.exe");
                //or you can use Application.ExecutablePath
                //close this one
                Process.GetCurrentProcess().Kill();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.BeginInvoke(new MethodInvoker(delegate () { btnClick.PerformClick(); }));
        }
    }
}
