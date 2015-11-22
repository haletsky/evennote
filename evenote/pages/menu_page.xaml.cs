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
    /// Interaction logic for menu_page.xaml
    /// </summary>
    public partial class menu_page : Page
    {
        public menu_page()
        {
            InitializeComponent();           
        }

        //Методы смены интерфейса
        private void notes_btn_Click(object sender, RoutedEventArgs e)
        {
            frame.Source = new Uri("notes_page.xaml", UriKind.Relative);
        }

        private void profile_btn_Click(object sender, RoutedEventArgs e)
        {
            Evennote.contextUser = Evennote.user;
            if (!(frame.Content is profile_page))
            {
                frame.Source = new Uri("profile_page.xaml", UriKind.Relative);
            }
            else
            {
                frame.Refresh();
            }
        }

        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            frame.Source = new Uri("search_page.xaml", UriKind.Relative);
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            frame.Source = new Uri("profile_page.xaml", UriKind.Relative);
        }
    }
}
