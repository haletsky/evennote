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
    /// Interaction logic for search_page.xaml
    /// </summary>
    public partial class search_page : Page
    {
        public search_page()
        {
            InitializeComponent();
            searchBtn.IsEnabled = !Evennote.OfflineMode;
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            //Если пользователь ищет сам себя, просто перенаправляем его на страницу своего профиля.
            if (textBox.Text == Evennote.user.username)
            {
                Evennote.contextUser = Evennote.user;
                ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("profile_page.xaml", UriKind.Relative);
                return;
            }

            Evennote.contextUser = Evennote.GetUserData(textBox.Text);
            if(Evennote.contextUser == null)
            {
                MessageBox.Show("User " + textBox.Text + " not exist!");
                return;
            }

            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("profile_page.xaml", UriKind.Relative);
        }
    }
}
