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
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            richTextBox.Document.Blocks.Remove(richTextBox.Document.Blocks.LastBlock);
            if (Notebook.rememberThis == null) return;
            titleTextBox.Text = Notebook.rememberThis.Title;

            using (MemoryStream mem = new MemoryStream())
            {
                TextRange range = new TextRange(Notebook.rememberThis.Text.ContentStart,
                    Notebook.rememberThis.Text.ContentEnd);
                range.Save(mem, DataFormats.XamlPackage);
                mem.Position = 0;

                TextRange kange = new TextRange(richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd);
                kange.Load(mem, DataFormats.XamlPackage);
            }
        }

        private void open_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            openFileDialog1.Filter = @"Evennote File(*.note)|*.note";

            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                Note temp = Evennote.OpenNote(openFileDialog1.FileName);

                titleTextBox.Text = temp.Title;

                using (MemoryStream mem = new MemoryStream())
                {
                    TextRange range = new TextRange(temp.Text.ContentStart,
                        temp.Text.ContentEnd);
                    range.Save(mem, DataFormats.XamlPackage);
                    mem.Position = 0;

                    TextRange kange = new TextRange(richTextBox.Document.ContentStart,
                        richTextBox.Document.ContentEnd);
                    kange.Load(mem, DataFormats.XamlPackage);
                }
            }
        }

        private void open_image_button(object sender, RoutedEventArgs e)
        {
            BitmapFrame image = Evennote.OpenImageInWindow();
            if (image == null) return;

            var temp = Clipboard.GetDataObject();
            Clipboard.SetImage(image);
            richTextBox.Paste();
            Clipboard.Clear();
            Clipboard.SetDataObject(temp);
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            if (titleTextBox.Text == "") return;

            DateTime temp = DateTime.Now;

            if (Notebook.rememberThis != null)
            {
                temp = Notebook.rememberThis.DateCreate;
                if (titleTextBox.Text != Notebook.rememberThis.Title)
                {
                    Notebook.Delete(Notebook.rememberThis);
                }
                else
                {
                    Notebook.RemoveAt(Notebook.IndexOf(Notebook.rememberThis));
                }
            }
            else
            {
                foreach (Note x in Notebook.notebook)
                {
                    if (titleTextBox.Text == x.Title)
                    {
                        MessageBox.Show("Note with same title already exist.");
                        return;
                    }
                }
            }
            Notebook.Add(new Note(titleTextBox.Text, richTextBox.Document, temp, DateTime.Now));
            Notebook.Last().SaveToFile(String.Format("{0}{1}.note", Evennote.path, Notebook.Last().Title));
            File.SetCreationTime(String.Format("{1}{0}.note", Notebook.Last().Title, Evennote.path), temp);
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("notes_page.xaml", UriKind.Relative);
        }

        private void colorpickerbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            //File.AppendAllText(Evennote.ConfigUserFile, Evennote.ColorNote.ToString());
        }

        private void titleTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^A-Za-z0-9_]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
