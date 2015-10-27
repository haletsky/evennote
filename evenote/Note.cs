﻿using System;
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
        public DateTime DateNotice { get; set; }

        public Note(string title, FlowDocument text, DateTime dt)
        {
            Title = title;
            Text = text;
            DateCreate = DateTime.Now;
            DateNotice = dt;
        }

        public Note()
        {
            Text = new FlowDocument();
            DateCreate = new DateTime();
            DateNotice = new DateTime();
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Title, DateCreate.ToShortDateString());
        }

        //Доделать
        public void SaveToFile(string pathnote)
        {
            try
            {
                using (FileStream fs = new FileStream(pathnote, FileMode.Create, FileAccess.ReadWrite))
                {
                    /*
                    BinaryWriter kek = new BinaryWriter(fs);
                    kek.Write(DateCreate.Ticks);
                    kek.Write(DateNotice.Ticks);
                    */
                                   
                    TextRange textRange = new TextRange(
                        Text.ContentStart,
                        Text.ContentEnd);
                    textRange.Save(fs, DataFormats.XamlPackage);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        //Доделать
        public void OpenFromFile(string pathnote)
        {
            try
            {
                using (FileStream fs = new FileStream(pathnote, FileMode.Open, FileAccess.ReadWrite))
                {
                    /*
                    BinaryReader kek = new BinaryReader(fs);
                    long create = kek.ReadInt64();
                    long notice = kek.ReadInt64();
                    */
                    
                    TextRange textRange = new TextRange(
                        Text.ContentStart,
                        Text.ContentEnd);
                    textRange.Load(fs, DataFormats.XamlPackage);                    

                    Title = pathnote.Substring(pathnote.LastIndexOf('\\') + 1, (pathnote.Substring(pathnote.LastIndexOf('\\') + 1).Length - 5));
                    //DateCreate = new DateTime(create);
                    //DateNotice = new DateTime(notice);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}