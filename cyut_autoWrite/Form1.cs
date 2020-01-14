using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_autoWrite
{
    public partial class Form1 : Form
    {
        internal cyut cyutWeb;
        public Form1()
        {
            //最上層
            this.TopMost = true;
            InitializeComponent();
            initForm();
            BindingObject();
            RegisterEvent();
        }

        internal void BindingObject()
        {
            ac_tbox.TextChanged += (s, e) => cyutWeb.loginInfo.account = ac_tbox.Text;
            pwd_tbox.TextChanged += (s, e) => cyutWeb.loginInfo.password = pwd_tbox.Text;
            verifyCode_tbox.TextChanged += (s, e) => cyutWeb.loginInfo.verifyCode = verifyCode_tbox.Text;
        }

        internal void RegisterEvent()
        {
            this.FormClosing += (s, e) => cyutWeb.web.DisposeDriver();
            //login
            btn_login.Click += async (s, e) =>
            {
                await cyutWeb.Login();
                if (cyutWeb.loginInfo.isLogin)
                {
                    btn_login.Enabled = false;
                    btn_autoWrite.Enabled = true;
                }
                else
                {
                    byte[] img = cyutWeb.GetVerifyCode();
                    if (img != null)
                        setVerifyCodeSC(pictureBox1, img);
                }
            };
            //auto write
            btn_autoWrite.Click += async (s, e) =>
            {
                btn_autoWrite.Enabled = false;
                cyutWeb.AutoWrite();
                btn_autoWrite.Enabled = true;
            };
        }

        internal void initForm()
        {
            cyutWeb = new cyut();
            Task.Run(() => cyutWeb.Init()).ContinueWith(it =>
            {
                setBtn_enable(btn_login, true);

                byte[] img = cyutWeb.GetVerifyCode();
                if (img != null)
                    setVerifyCodeSC(pictureBox1, img);

                it.Dispose();
            });
        }

        #region 設置login btn enable
        private void setBtn_enable(Button btn, bool val)
        {
            if (this.InvokeRequired)
            {
                delegate_setBtn_enable method = setBtn_enable;
                Invoke(method, btn, val);
                return;
            }

            btn_login.Enabled = val;
        }
        delegate void delegate_setBtn_enable(Button btn, bool val);
        #endregion

        #region 驗證碼截圖
        private void setVerifyCodeSC(PictureBox pc, byte[] imgByteArr)
        {
            if (this.InvokeRequired)
            {
                setVerifyCode_ScreenShow method = setVerifyCodeSC;
                Invoke(method, pc, imgByteArr);
                return;
            }

            Image x = (Bitmap)((new ImageConverter()).ConvertFrom(imgByteArr));
            pc.Image = x;
        }
        delegate void setVerifyCode_ScreenShow(PictureBox pc, byte[] imgByteArr);
        #endregion
    }
}
