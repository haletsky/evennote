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
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace evenote.pages
{
    /// <summary>
    /// Interaction logic for login_page.xaml
    /// </summary>
    public partial class login_page : Page
    {

        public login_page()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //Подключение к своей базе данных
            MyDataBase.ConnectToDB();

            //Проверяем совпадают ли имя и пароли
            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + login.Text + "' AND users.userpass = '" + password.Password + "'");
            if (MyDataBase.rdr.HasRows == false)
            {
                MessageBox.Show("Login or password is incorrect.");
                MyDataBase.rdr.Close();
                MyDataBase.CloseConnectToDB();
                return;
            }

            //Читаем данные
            while (MyDataBase.rdr.Read())
            {
                //Перенаправляем на главное меню
                (Application.Current.MainWindow as MainWindow).ChangePage("pages/menu_page.xaml");

                //Сохраняем данные о себе
                (Application.Current.MainWindow as MainWindow).me = new User(MyDataBase.rdr[0].ToString(),
                    MyDataBase.rdr[1].ToString(),
                    MyDataBase.rdr[3].ToString(),
                    MyDataBase.rdr[4] as byte[],
                    MyDataBase.rdr[5].ToString());

                (Application.Current.MainWindow as MainWindow).me.online = 1;
                (Application.Current.MainWindow as MainWindow).contextUser = (Application.Current.MainWindow as MainWindow).me;
            }
            
            MyDataBase.rdr.Close();

            //Пишем что мы онлайн
            MyDataBase.ExecuteCommand("UPDATE `evennote_db`.`users` SET `online`= 1 WHERE `idusers`= '" + (Application.Current.MainWindow as MainWindow).me.id + "'");

            //Отключаемся от БД
            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            //Считываем с диска существующие заметки
            Notebook.OpenNotes();            
        }

        //Просто методы для удобства интерфейса 
        private void login_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "login")
                (sender as TextBox).Text = "";
        }
            
        private void login_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
                (sender as TextBox).Text = "login";
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "")
                (sender as PasswordBox).Password = "password";
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "password")
                (sender as PasswordBox).Password = "";
        }

    }
}
