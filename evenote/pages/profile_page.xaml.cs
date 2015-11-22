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

namespace evenote.pages
{
    /// <summary>
    /// Interaction logic for profile_page.xaml
    /// </summary>
    public partial class profile_page : Page
    {
        public profile_page()
        {
            InitializeComponent();
            if (Evennote.contextUser == Evennote.user)
            {
                logout_btn.Visibility = Visibility.Visible;
            }
            else
            {
                logout_btn.Visibility = Visibility.Hidden;
            }
        }

        //Когда открываем страницу профиля  - заполняем данные о пользователе
        private void Grid_Initialized(object sender, EventArgs e)
        {
            if (Evennote.contextUser.avatar != null)
                image.Fill = new ImageBrush(Evennote.contextUser.avatar);

            labelname.Content = Evennote.contextUser.username;
            labelemail.Content = "Email: " + Evennote.contextUser.email;
            labeldatebirth.Content = "Date birth: " + Evennote.contextUser.datebirth.ToShortDateString();

            if (Evennote.contextUser.online == false)
            {
                labeloline.Content = "Offline";
            }
            else
            {
                labeloline.Content = "Online";
                Color c = new Color();
                c.A = 200;
                c.B = 8;
                c.G = 200;
                c.R = 40;

                SolidColorBrush color = new SolidColorBrush(c);
                labeloline.Foreground = color;
            }
        }

        private void logout_btn_Click(object sender, RoutedEventArgs e)
        {
            Notebook.notebook.RemoveRange(0, Notebook.notebook.Count);
            Evennote.user.online = false;
            (Application.Current.MainWindow as MainWindow).ChangePage("pages/login_page.xaml");
            Evennote.AutoLogin = false;
            System.IO.File.Delete(Evennote.ConfigFile);
            MyDataBase.ConnectToDB();
            MyDataBase.ChangeOnlineStatus(Evennote.user.id);
            MyDataBase.CloseConnectToDB();
            Evennote.user = null;
        }
    }
}