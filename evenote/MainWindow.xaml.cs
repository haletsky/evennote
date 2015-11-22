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
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ChangePage(string src)
        {
            mainframe.Source = new Uri(src, UriKind.Relative);
        }

        private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Evennote.user != null && Evennote.user.online == true)
            {
                MyDataBase.ConnectToDB();
                MyDataBase.SetOfflineUser(Evennote.user.id);
                MyDataBase.CloseConnectToDB();
            }
        }
    }
}

