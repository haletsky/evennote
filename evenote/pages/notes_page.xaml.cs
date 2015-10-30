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

namespace evenote.pages
{
    /// <summary>
    /// Interaction logic for notes_page.xaml
    /// </summary>
    public partial class notes_page : Page
    {
        public notes_page()
        {
            InitializeComponent();
            Notebook.rememberThis = null;
        }

        private void addnote_btn_Click(object sender, RoutedEventArgs e)
        {
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("addnote_page.xaml", UriKind.Relative);
        }

        private void editnote_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Notebook.rememberThis == null) return;
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("editnote_page.xaml", UriKind.Relative);
            //listView.SelectedIndex = -1;
        }

        private void deletenote_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Notebook.rememberThis == null) return;
            Notebook.Delete(Notebook.rememberThis as NoteListItem);
            listView.Items.Refresh();
        }

        private void sendnote_btn_Click(object sender, RoutedEventArgs e)
        {
           
            
        }

        private void listView_Initialized(object sender, EventArgs e)
        {
            listView.ItemsSource = Notebook.notebook;

        }

        private void listView_LostFocus(object sender, RoutedEventArgs e)
        {
            Notebook.rememberThis = listView.SelectedItem as NoteListItem;
            listView.SelectedIndex = -1;
        }
    }
}

