using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace evenote
{
    //Класс-описание заметки
    public class Note
    {
        public string Title { get; set; }
        public FlowDocument Text { get; set; }
        public DateTime DateCreate { get; set; }

        public Note(string title, FlowDocument text, DateTime dt)
        {
            Title = title;
            Text = text;
            DateCreate = dt;
        }

        public Note()
        {
            Text = new FlowDocument();
            DateCreate = new DateTime();
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Title, DateCreate.ToShortDateString());
        }

        public void SaveToFile(string pathnote)
        {
            try
            {
                using (FileStream fs = new FileStream(pathnote, FileMode.Create, FileAccess.ReadWrite))
                {
                    TextRange textRange = new TextRange(
                        Text.ContentStart,
                        Text.ContentEnd);
                    textRange.Save(fs, DataFormats.XamlPackage);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void OpenFromFile(string pathnote)
        {
            try
            {
                using (FileStream fs = new FileStream(pathnote, FileMode.Open, FileAccess.ReadWrite))
                {
                    TextRange textRange = new TextRange(
                        Text.ContentStart,
                        Text.ContentEnd);
                    textRange.Load(fs, DataFormats.XamlPackage);

                    DateCreate = File.GetCreationTime(pathnote);
                    Title = pathnote.Substring(pathnote.LastIndexOf('\\') + 1, (pathnote.Substring(pathnote.LastIndexOf('\\') + 1).Length - 5));
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}