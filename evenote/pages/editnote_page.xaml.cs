﻿using Microsoft.Win32;
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
                Note newtemp = new Note();
                newtemp.OpenFromFile(openFileDialog1.FileName);

                titleTextBox.Text = newtemp.Title;

                using (MemoryStream mem = new MemoryStream())
                {
                    TextRange range = new TextRange(newtemp.Text.ContentStart,
                        newtemp.Text.ContentEnd);
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
                    Clipboard.SetImage(BitmapFrame.Create(new Uri(openFileDialog1.FileName)));
                    richTextBox.Paste();
                    Clipboard.Clear();
                }
            }
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

            Notebook.Add(new Note(titleTextBox.Text, richTextBox.Document, temp));
            Notebook.Last().SaveToFile(String.Format("{1}{0}.note", Notebook.Last().Title, Config.path));
            File.SetCreationTime(String.Format("{1}{0}.note", Notebook.Last().Title, Config.path), temp);
            ((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Source = new Uri("notes_page.xaml", UriKind.Relative);
        }
    }
}
