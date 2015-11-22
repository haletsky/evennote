using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using DataBaseAPI;

namespace evenote
{
    //Класс-описание заметки
    public class Note
    {
        public int Backuped { get; set; }
        /*
        -2 на бд нет заметки
        -1 на бд старая заметка
        0 заметки равны
        1 на бд новая заметка
        */
        public int Id { get; set; }
        public string Title { get; set; }
        public FlowDocument Text { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateChanged { get; set; }

        public Note(string title, FlowDocument text, DateTime dtCR, DateTime dtCH)
        {
            Title = title;
            Text = text;
            DateCreate = dtCR;
            DateChanged = dtCH;
            //DateChanged = DateChanged.ToUniversalTime();
            //DateCreate = DateCreate.ToUniversalTime();
        }

        public Note()
        {
            Text = new FlowDocument();
            DateCreate = new DateTime();
            DateChanged = new DateTime();
            //DateChanged = DateChanged.ToUniversalTime();
            //DateCreate = DateCreate.ToUniversalTime();
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

                    DateChanged = File.GetLastWriteTime(pathnote);
                    DateCreate = File.GetCreationTime(pathnote);
                    Title = pathnote.Substring(pathnote.LastIndexOf('\\') + 1, (pathnote.Substring(pathnote.LastIndexOf('\\') + 1).Length - 5));
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void RefreshNoteState(int userid)
        {
            MyDataBase.ConnectToDB();

            MyDataBase.ExecuteCommand("SELECT dateChanged FROM notes WHERE iduser = " + userid  + " AND title = '" + Title + "';");

            if (!MyDataBase.rdr.HasRows)
            {
                Backuped = -2;
                return;
            }

            while (MyDataBase.rdr.Read())
            {
                DateTime fromDB = new DateTime((long)MyDataBase.rdr[0]);

                if (DateTime.Compare(DateChanged, fromDB) > 0)//Когда на бд старая заметка, а у нас новая
                {
                    Backuped = -1;
                }
                else if (DateTime.Compare(DateChanged, fromDB) < 0)//Когда на бд новая заметка, а у нас старая
                {
                    Backuped = 1;
                }
                else
                {
                    Backuped = 0; ;
                }
            }

            MyDataBase.CloseConnectToDB();
        }
    }
}