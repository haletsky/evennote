using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
            richTextBox.Document.Blocks.Remove(richTextBox.Document.Blocks.FirstBlock);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (titleTextBox.Text == "" || !datepicker.SelectedDate.HasValue) return;
            Notebook.notebook.RemoveAt(Notebook.notebook.IndexOf(Notebook.rememberThis));
            Notebook.Add(new Note(titleTextBox.Text, richTextBox.Document, datepicker.SelectedDate.Value));
            Notebook.notebook.Last().SaveToFile(String.Format("{1}{0}.note", Notebook.notebook.Last().Title, Config.path));
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("notes_page.xaml", UriKind.Relative);
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            titleTextBox.Text = Notebook.rememberThis.Title;
            datepicker.SelectedDate = Notebook.rememberThis.DateNotice;

            for (int i = 0; i < Notebook.rememberThis.Text.Blocks.Count; i++)
            {
                if (Notebook.rememberThis.Text.Blocks.ElementAt(i) is Paragraph)
                {
                    Paragraph p = new Paragraph();

                    for(int j = 0; j < (Notebook.rememberThis.Text.Blocks.ElementAt(i) as Paragraph).Inlines.Count; j++)
                    {
                        p.Inlines.Add(new Run(((Notebook.rememberThis.Text.Blocks.ElementAt(i) as Paragraph).Inlines.ElementAt(j) as Run).Text));
                    }                  
                    
                    richTextBox.Document.Blocks.Add(p);
                }
                else if (Notebook.rememberThis.Text.Blocks.ElementAt(i) is BlockUIContainer)
                {
                    Image im = new Image();
                    im.Stretch = Stretch.None;
                    im.Source = ((Notebook.rememberThis.Text.Blocks.ElementAt(i) as BlockUIContainer).Child as Image).Source;

                    richTextBox.Document.Blocks.Add(new BlockUIContainer(im));
                }
                else if (Notebook.rememberThis.Text.Blocks.ElementAt(i) is List)
                {

                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
                using (Stream s = openFileDialog1.OpenFile())
                {
                    Image i = new Image();
                    i.Source = BitmapFrame.Create(s, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    i.Stretch = Stretch.None;
                    richTextBox.Document.Blocks.Add(new BlockUIContainer(i));
                }
            }
        }
    }
}
