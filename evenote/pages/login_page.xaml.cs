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
using System.Text.RegularExpressions;

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
            try
            {
                //Подключение к своей базе данных
                MyDataBase.ConnectToDB();

                //Проверяем совпадают ли имя и пароли
                MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + login.Text + "' AND users.userpass = '" + password.Password + "'");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); return;
            }

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
                (Application.Current.MainWindow as MainWindow).me = new User(Convert.ToInt32(MyDataBase.rdr[0].ToString()),
                    MyDataBase.rdr[1].ToString(),
                    MyDataBase.rdr[3].ToString(),
                    MyDataBase.rdr[4] as byte[],
                    MyDataBase.rdr[5] as DateTime?);
            }

            (Application.Current.MainWindow as MainWindow).contextUser = (Application.Current.MainWindow as MainWindow).me;

            MyDataBase.rdr.Close();

            //Пишем что мы онлайн
            MyDataBase.ChangeOnlineStatus((Application.Current.MainWindow as MainWindow).me.id);
            (Application.Current.MainWindow as MainWindow).me.online = true;

            //Отключаемся от БД
            MyDataBase.CloseConnectToDB();

            Config.SetUserDirectory(login.Text);

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

        private void signUp_button_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).ChangePage("pages/regist_page.xaml");
        }

        private void login_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^A-Za-z0-9_]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
