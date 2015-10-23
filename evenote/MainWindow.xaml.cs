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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataBaseAPI;
using MySql.Data;
using MySql.Data.MySqlClient;

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
        }

        public void ChangePage(string src)
        {
            mainframe.Source = new Uri(src, UriKind.Relative);
        }

        private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((Application.Current.MainWindow as MainWindow).me != null)
            {
                MyDataBase.ConnectToDB();
                MyDataBase.ExecuteCommand("UPDATE `evennote_db`.`users` SET `online`= 0 WHERE `idusers`= '" + (Application.Current.MainWindow as MainWindow).me.id + "'");
                MyDataBase.CloseConnectToDB();
            }
        }
    }
}

