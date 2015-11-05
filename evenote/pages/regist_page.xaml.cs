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
            Regex x = new Regex(@"^\w+@\w+[.]\w+$");

            if (login.Text == "login" || password.Password == "password" || password.Password != repeat_password.Password || datepicker.SelectedDate == null || email.Text == "e@mail.com" || !x.IsMatch(email.Text))
            {
                MessageBox.Show("Something wrong!", "Error!");
                return;
            }


            MyDataBase.ConnectToDB();

            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + login.Text + "' OR users.email = '" + email.Text + "'");
             
            if (MyDataBase.rdr.HasRows)
            {
                MessageBox.Show("This user already exist.");
                return;
            }

            MyDataBase.rdr.Close();
            MyDataBase.AddWithValue("@avatar", image);  
            MyDataBase.ExecuteCommand(
                "INSERT INTO `evennote_db`.`users` (`username`, `userpass`, `email`, `avatar`, `datebirth`)" + 
                "VALUES ('" + login.Text + "', '" + password.Password + "', '" + email.Text + "', @avatar, '" 
                + datepicker.SelectedDate.Value.Year + "-"
                + datepicker.SelectedDate.Value.Month + "-"
                + datepicker.SelectedDate.Value.Day + "');");

            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            (Application.Current.MainWindow as MainWindow).ChangePage("pages/login_page.xaml");
        }

        private void login_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "login" || (sender as TextBox).Text == "email")
                (sender as TextBox).Text = "";
        }

        private void login_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                if((sender as TextBox).Tag as string == "email")(sender as TextBox).Text = "email";
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

        private void selectbutton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            openFileDialog1.Filter = @"Bitmap File(*.bmp)|*.bmp|" +
                @"GIF File(*.gif)|*.gif|" +
                @"JPEG File(*.jpg)|*.jpg|" +
                @"TIF File(*.tif)|*.tif|" +
                @"PNG File(*.png)|*.png";

            openFileDialog1.FilterIndex = 3;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                imagelabel.Content = openFileDialog1.FileName;
                using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                {
                    image = new byte[fs.Length];
                    fs.Read(image, 0, (int)fs.Length);
                    fs.Close();
                }                
            }
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
    }
}
