using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _SeleniumWeb;
using OpenQA.Selenium;

namespace cyut_autoWrite
{
    class cyut
    {
        public gSeleniumWeb web;
        public LoginModel loginInfo;
        public cyut()
        {
            web = new gSeleniumWeb();
            loginInfo = new LoginModel();
        }

        public void Init()
        {
            web.Init("https://auth2.cyut.edu.tw/User/Login");
            //web.waitForByXPath("//div[@class='form-horizontal']");
        }

        public async Task Login()
        {
            if (string.IsNullOrEmpty(loginInfo.account) || string.IsNullOrEmpty(loginInfo.password) || string.IsNullOrEmpty(loginInfo.verifyCode))
            {
                MessageBox.Show("請確實輸入登入資訊。");
                return;
            }

            //ac
            web.WebTypeInByXPath("//input[@id='Account']", loginInfo.account);
            //pwd
            web.WebTypeInByXPath("//input[@id='Password']", loginInfo.password);
            //verify code
            web.WebTypeInByXPath("//input[@id='VerificationCode']", loginInfo.verifyCode);
            //click login
            web.webClickEventByXPath("//input[@value='登入']");

            Task.Delay(300).Wait();
            if (string.IsNullOrEmpty(web.webTextGetByXPath("//span[@data-valmsg-for='Account']")) && string.IsNullOrEmpty(web.webTextGetByXPath("//span[@data-valmsg-for='Password']")) && string.IsNullOrEmpty(web.webTextGetByXPath("//span[@data-valmsg-for='VerificationCode']")))
            {
                loginInfo.isLogin = true;
                MessageBox.Show("登入成功。");
            }
            else
                MessageBox.Show("登入失敗，請確認帳號密碼與驗證碼。");
        }

        public async Task AutoWrite()
        {
            web.webClickEventByXPath("//div[@data-filter-name='教學評量填答']/a");
            //web.GoToUrl("http://admin2.cyut.edu.tw/tquest/student/question_main_stu.php");
            var attrList = web.elementAttrList("//*[@id='frm_main']/table/thead/tr[2]/td/table//a", "onclick");
            if (attrList.Count == 0)
                MessageBox.Show("已填寫完所有問卷囉。");
            else
            {
                foreach (var item in attrList)
                {
                    string clearStr = item.Replace("fncActionOpen(", "").Replace(");", "").Replace("'", "");
                    List<string> paraList = new List<string>();
                    paraList.AddRange(clearStr.Split(','));
                    string writeLessonURL = $"http://admin2.cyut.edu.tw/tquest/student/question_write.php?Action={ paraList[0] }&OID={ paraList[1] }&crskey={ paraList[2] }&basno={ paraList[3] }&term={ paraList[4] }";
                    web.GoToUrl(writeLessonURL);

                    web.webAllClickEventByXPath("//tr[@class='txt_12']/td[not(@class)][1]/input");
                    //其他部份 Other parts
                    web.webAllClickEventByXPath("//tr[not(@class)][2]/td[@class='txt_12'][1]/input[1]");

                    if (web.waitForByXPath("//iframe[1]"))
                    {
                        var iframe = web.driver.FindElement(By.XPath("//iframe[1]"));
                        web.driver.SwitchTo().Frame(iframe);
                        Task.Delay(300).Wait();
                        web.webAllClickEventByXPath("//div[contains(@class,'col-check5')]");
                        Task.Delay(300).Wait();
                        web.webAllClickEventByXPath("//div[contains(@class,'col-check5')]");
                        web.SwitchFrameDefault();
                        Task.Delay(300).Wait();
                        //送出問卷
                        web.webClickEventByXPath("//input[@name='send']");
                        Task.Delay(500).Wait();
                        web.dismissAlert();
                        Task.Delay(500).Wait();
                        web.dismissAlert();
                    }
                }

                web.GoToUrl("https://auth2.cyut.edu.tw/stuApp/B77A3260-1747-4998-B359-4EF97A6EF324?sLang=zh-TW");
                MessageBox.Show("恭喜，所有問卷填寫完畢。");
            }
        }


        public byte[] GetVerifyCode()
        {
            return web.getScreenShot("//img[@alt='驗證碼']");
        }
    }
}
