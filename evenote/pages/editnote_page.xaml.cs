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
    /// Interaction logic for editnote_page.xaml
    /// </summary>
    public partial class editnote_page : Page
    {
        public editnote_page()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (titleTextBox.Text == "" || !datepicker.SelectedDate.HasValue) return;
            Notebook.notebook.RemoveAt(Notebook.notebook.IndexOf(Notebook.rememberThis));
            Notebook.Add(new Note(titleTextBox.Text, richTextBox.Document, datepicker.SelectedDate.Value));
            Notebook.notebook.Last().SaveToFile();
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("notes_page.xaml", UriKind.Relative);

        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            titleTextBox.Text = Notebook.rememberThis.Title;
            datepicker.SelectedDate = Notebook.rememberThis.DateNotice;

            richTextBox.AppendText(new TextRange(Notebook.rememberThis.Text.ContentStart, Notebook.rememberThis.Text.ContentEnd).Text);
        }
    }
}
