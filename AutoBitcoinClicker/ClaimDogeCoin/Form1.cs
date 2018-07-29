using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ClaimDogeCoin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IWebDriver chromedriver;

        private void btnClaim_Click(object sender, EventArgs e)
        {
            try
            {
                string[] arguments = new string[] { "no-sandbox", "test-type", "ignore-certificate-errors", "disable-popup-blocking" };
                var chromeOptions = new ChromeOptions() { BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" };
                chromeOptions.AddArguments(arguments);
                chromedriver = new ChromeDriver(@"chromedriver_win32\", chromeOptions, new System.TimeSpan(70, 0, 0));

                if (chromedriver != null)
                {
                    chromedriver.Url = "http://coin.dy.fi/doge/";
                    int count = 0;
                    while (count <= 1000)
                    {
                        var address = chromedriver.FindElement(By.Id("address"));
                        address.SendKeys("Dogecoin Addresses");

                        IList<IWebElement> iframe = chromedriver.FindElements(By.TagName("iframe"));
                        chromedriver.SwitchTo().Frame(iframe[2]);
                        chromedriver.FindElement(By.Id("verify-me")).Click();

                        var error = chromedriver.FindElement(By.ClassName("error-text")).Text;
                        while (error == "reload")
                        {
                            chromedriver.FindElement(By.LinkText("reload")).Click();
                            error = chromedriver.FindElement(By.ClassName("error-text")).Text;
                        }

                        var progress = chromedriver.FindElement(By.ClassName("verified-text")).Text;
                        while (progress != "verified")
                        {
                            progress = chromedriver.FindElement(By.ClassName("verified-text")).Text;
                        }

                        chromedriver.SwitchTo().DefaultContent();

                        var request = chromedriver.FindElement(By.Id("request"));
                        request.Click();

                        Thread.Sleep(3000);

                        var reload = chromedriver.FindElement(By.Id("reload"));
                        reload.Click();

                        count++;
                    }
                }

                chromedriver.Close();
                chromedriver.Quit();
                //run the program again and close this one
                Process.Start(Application.StartupPath + "\\ClaimDogeCoin.exe");
                //or you can use Application.ExecutablePath
                //close this one
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception)
            {
                chromedriver.Close();
                chromedriver.Quit();

                //run the program again and close this one
                Process.Start(Application.StartupPath + "\\ClaimDogeCoin.exe");
                //or you can use Application.ExecutablePath
                //close this one
                Process.GetCurrentProcess().Kill();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.BeginInvoke(new MethodInvoker(delegate () { btnClaim.PerformClick(); }));
        }
    }
}
