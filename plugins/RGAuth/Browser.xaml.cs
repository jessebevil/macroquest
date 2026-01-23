using MSHTML;
using RGAuth.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Authorization = RGAuth.Code.Authorization;

namespace RGAuth
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {
        public String authMessage = "";
        public String authUri = "";
        public String redirectUri = "";
        public WebBrowser browser = null!;
        public HttpListener listener = null!;
        public String user = null!;
        public SecureString password = null!;
        public PageMode pageMode = PageMode.Authorization;
        private Boolean loginAttempted = false;

        public enum PageMode
        {
            Authorization,
            Watch
        }

        public Browser()
        {
            InitializeComponent();

            browser = auth_webBrowser;
        }

        private void auth_webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            dynamic activeX = ((WebBrowser)sender).GetType().InvokeMember("ActiveXInstance",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, ((WebBrowser)sender), new object[] { })!;

            activeX.Silent = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listener != null)
            {
                listener.Close();
            }
        }

        #region Authorization Functionality
        private void auth_webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (this.IsLoaded)
            {
                HTMLDocument rg = (HTMLDocument)auth_webBrowser.Document;

                switch (pageMode)
                {
                    case PageMode.Authorization:
                        IHTMLElement authorizeBtn = AssignMSHTMLElement(rg, "button--primary button button--icon button--icon--lock");

                        if (!loginAttempted)
                        {
                            loginAttempted = true;

                            if (authorizeBtn != null)
                            {
                                ConfirmUserPermission(authorizeBtn);
                            }
                            else
                            {
                                IHTMLElement loginFld = null!;
                                IHTMLElement pwFld = null!;
                                IHTMLElement loginBtn = AssignMSHTMLElement(rg, "button--primary button button--icon button--icon--login");

                                foreach (IHTMLElement element in rg.getElementsByClassName("input"))
                                {
                                    if (!String.IsNullOrEmpty(element.id))
                                    {
                                        switch (element.id.ToUpper())
                                        {
                                            case String _id when _id.Contains("_XFUID-1"):
                                                loginFld = element;
                                                break;

                                            case String _id when _id.Contains("_XFUID-2"):
                                                pwFld = element;
                                                break;
                                        }
                                    }
                                }

                                if (loginFld != null)
                                {
                                    loginFld.innerText = user;
                                    pwFld.innerText = SSEncryption.ToInsecureString(password);
                                    loginBtn.click();
                                }
                            }
                        }
                        else if (!auth_webBrowser.Source.ToString().Contains(redirectUri))
                        {
                            if (authorizeBtn != null)
                            {
                                ConfirmUserPermission(authorizeBtn);
                            }
                            else
                            {
                                CheckForBadData(rg);
                            }
                        }
                        break;

                    case PageMode.Watch:
                        IHTMLElement watchBtn = (AssignMSHTMLElement(rg, "button--primary button button--icon button--icon--notificationsOn") != null) ?
                                                AssignMSHTMLElement(rg, "button--primary button button--icon button--icon--notificationsOn") :
                                                AssignMSHTMLElement(rg, "button--primary button button--icon button--icon--notificationsOff");

                        watchBtn.click();

                        this.Close();
                        break;
                }
            }
        }

        private IHTMLElement AssignMSHTMLElement(MSHTML.HTMLDocument rg, String className)
        {
            IHTMLElement element = null!;

            foreach (IHTMLElement elem in rg.getElementsByClassName(className))
            {
                element = elem;
            }

            return element;
        }

        private void CheckForBadData(MSHTML.HTMLDocument rg)
        {
            IHTMLElement badLoginDiv = AssignMSHTMLElement(rg, "blockMessage blockMessage--error blockMessage--iconic");
            IHTMLElement blockedDiv = AssignMSHTMLElement(rg, "blockMessage");

            if (badLoginDiv != null)
            {
                Authorization.LoginFailed("Incorrect Username/Password");
            }
            else if (blockedDiv != null)
            {
                Authorization.LoginFailed("Too many login attempts, please try again later...");
            }

            this.Close();
        }

        private void ConfirmUserPermission(IHTMLElement authorizeBtn)
        {
            switch (Authorization.AskForAuth(authMessage))
            {
                case MessageBoxResult.Yes:
                    authorizeBtn.click();
                    break;

                default:
                    Authorization.LoginFailed("Cannot login, authorization declined.");

                    this.Close();
                    break;
            }
        }
        #endregion Authorization Functionality

        #region Watch Functionality

        #endregion Watch Functionality
    }
}
