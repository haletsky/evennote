using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace evenote
{
    //Класс-коллекция заметок
    public static class Notebook
    {
        public static List<Note> notebook = new List<Note>();
        public static Note rememberThis = null;

        public static void Add(Note n)
        {
            notebook.Add(n);
        }

        public static void Edit(Note n)
        {
            if (notebook.Contains(rememberThis))
            {
                //требует доработки
            }
        }

        public static void OpenNotes()
        {
            string[] notes = Directory.GetFiles(Config.path);

            for (int i = 0; i < notes.Length; i++)
            {
                Note n = new Note();
                n.OpenFromFile(notes[i]);
                Add(n);
            }
        }

        public static void Delete(Note n)
        {
            try
            {
                File.Delete(String.Format("{0}{1}.note", Config.path, n.Title));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            notebook.Remove(n);
        }
    }
}

