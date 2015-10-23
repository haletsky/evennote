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

namespace evenote.pages
{
    /// <summary>
    /// Interaction logic for search_page.xaml
    /// </summary>
    public partial class search_page : Page
    {
        public search_page()
        {
            InitializeComponent();
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            //Если пользователь ищет сам себя, просто перенаправляем его на страницу своего профиля.
            if (textBox.Text == (Application.Current.MainWindow as MainWindow).me.username)
            {
                (Application.Current.MainWindow as MainWindow).contextUser = (Application.Current.MainWindow as MainWindow).me;
                ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("profile_page.xaml", UriKind.Relative);
                return;
            }

            //Подключение к своей базе данных
            MyDataBase.ConnectToDB();
            if (MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = " + "'" + textBox.Text + "'") == 1) return;

            if (MyDataBase.rdr.HasRows == false)
            {
                MessageBox.Show("Никого не найдено.");
                return;
            }
            while (MyDataBase.rdr.Read())
            {
                //Сохраняем данные о пользователе
                (Application.Current.MainWindow as MainWindow).contextUser = new User(MyDataBase.rdr[0].ToString(),
                    MyDataBase.rdr[1].ToString(),
                    MyDataBase.rdr[3].ToString(),
                    MyDataBase.rdr[4] as byte[],
                    MyDataBase.rdr[5].ToString());
                
                if (MyDataBase.rdr[6].ToString().Equals("True"))
                {
                    (Application.Current.MainWindow as MainWindow).contextUser.online = 1;
                }
            }

            //Отключаемся от БД
            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("profile_page.xaml", UriKind.Relative);
        }
    }
}
