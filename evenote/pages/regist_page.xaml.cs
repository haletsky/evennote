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
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace evenote.pages
{
    /// <summary>
    /// Interaction logic for regist_page.xaml
    /// </summary>
    public partial class regist_page : Page
    {

        byte[] image;

        public regist_page()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Regex x = new Regex(@"^[\w.]+@\w+[.]\w+$");

            if (login.Text == "login" || password.Password == "password" || password.Password != repeat_password.Password || datepicker.SelectedDate == null || email.Text == "e@mail.com" || !x.IsMatch(email.Text))
            {
                MessageBox.Show("Something wrong!", "Error!");
                return;
            }

            Evennote.Registration(login.Text, password.Password, datepicker.SelectedDate.Value, email.Text, image);

            (Application.Current.MainWindow as MainWindow).ChangePage("pages/login_page.xaml");
        }

        private void selectbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(Evennote.OpenImageInWindow().ToString().Remove(0, 8), FileMode.Open, FileAccess.Read))
                {
                    imagelabel.Content = fs.Name.Split('\\').Last();
                    image = new byte[fs.Length];
                    fs.Read(image, 0, (int)fs.Length);
                    fs.Close();
                }
            }
            catch { }
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

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).ChangePage("pages/login_page.xaml");
        }


        //Просто методы для удобного интерфейса
        private void login_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "login" || (sender as TextBox).Text == "e@mail.com")
                (sender as TextBox).Text = "";
        }

        private void login_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                if((sender as TextBox).Tag as string == "email")(sender as TextBox).Text = "e@mail.com";
                else (sender as TextBox).Text = "login";
            }
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "password")
                (sender as PasswordBox).Password = "";
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "")
                (sender as PasswordBox).Password = "password";
        }

    }
}
