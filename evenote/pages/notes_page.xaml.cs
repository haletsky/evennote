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
using System.Threading;
using DataBaseAPI;

namespace evenote.pages
    {
        /// <summary>
        /// Interaction logic for notes_page.xaml
        /// </summary>
        public partial class notes_page : Page
        {
            //
            List<NoteListItem> noteListItems = new List<NoteListItem>();

        public notes_page()
        {
            InitializeComponent();
            Notebook.rememberThis = null;

            syncnote_btn.IsEnabled = !Evennote.OfflineMode;

            RefreshStatusBackup();
        }

        private void addnote_btn_Click(object sender, RoutedEventArgs e)
        {
            Notebook.rememberThis = null;
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("editnote_page.xaml", UriKind.Relative);
        }

        private void editnote_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Notebook.rememberThis == null) return;
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("editnote_page.xaml", UriKind.Relative);
        }

        private void deletenote_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Notebook.rememberThis == null) return;
            Notebook.Delete(Notebook.rememberThis);
            SyncListView();
        }

        private void sendnote_btn_Click(object sender, RoutedEventArgs e)
        {
            try {
                Evennote.SyncNotes();
                //Dispatcher.BeginInvoke(new ThreadStart(Evennote.SyncNotes)); //Executes the specified delegate asynchronously with the specified arguments, at the specified priority, on the thread that the Dispatcher was created on.
                RefreshStatusBackup();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView_Initialized(object sender, EventArgs e)
        {
            SyncListView();
            listView.ItemsSource = noteListItems;
        }

        private void listView_LostFocus(object sender, RoutedEventArgs e)
        {
            Notebook.rememberThis = (listView.SelectedItem as NoteListItem).Content as Note;
            listView.SelectedIndex = -1;
        }

        public void SyncListView()
        {
            noteListItems.Clear();
            foreach (Note x in Notebook.notebook)
            {
                noteListItems.Add(new NoteListItem(x));
            }
            listView.Items.Refresh();        
        }

        public void RefreshStatusBackup()
        {
            int temp = Evennote.GetCountNotesFromDB();
            string result = "";
            if (temp == -1) result = "You are offline.";
            else result = String.Format("Backup: {0}", temp);

            statusBackup.Content = result;
        }
    }
}