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
        }

        //Когда открываем страницу профиля  - заполняем данные о пользователе
        private void Grid_Initialized(object sender, EventArgs e)
        {
            image.Fill = new ImageBrush((Application.Current.MainWindow as MainWindow).contextUser.avatar);
            labelname.Content = (Application.Current.MainWindow as MainWindow).contextUser.username;
            labelemail.Content = "Email: " + (Application.Current.MainWindow as MainWindow).contextUser.email;
            labeldatebirth.Content = "Date birth: " + (Application.Current.MainWindow as MainWindow).contextUser.datebirth.ToShortDateString();

            if((Application.Current.MainWindow as MainWindow).contextUser.online == 0)
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
    }
}
