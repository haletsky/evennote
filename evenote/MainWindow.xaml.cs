using System;
using System.Collections.Generic;
using System.Linq;
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
using DataBaseAPI;
using System.Diagnostics;
using System.IO;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace evenote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User me;
        public User contextUser;

        public MainWindow()
        {
            Config.ConfigureProgram();
            InitializeComponent();

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode("Evennote"));
            stringElements[1].AppendChild(toastXml.CreateTextNode("Trush pidor i mraz', zdohnet ot spida!"));

            // Specify the absolute path to an image
            String imagePath = System.IO.Path.GetFullPath("../../images/icon.png");
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += ToastActivated;
            //toast.Dismissed += ToastDismissed;
            //toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier("Evennote").Show(toast);
        }

        private void ToastActivated(ToastNotification sender, object e)
        {
            Dispatcher.Invoke(() =>
            {
                Activate();
            });
        }

        public void ChangePage(string src)
        {
            mainframe.Source = new Uri(src, UriKind.Relative);
        }

        private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (me != null && me.online == true)
            {
                MyDataBase.ConnectToDB();
                MyDataBase.ChangeOnlineStatus((Application.Current.MainWindow as MainWindow).me.id);
                MyDataBase.CloseConnectToDB();
            }
        }
    }
}

