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
        public static List<NoteListItem> notebook = new List<NoteListItem>();
        public static NoteListItem rememberThis = null;

        public static void Add(Note n)
        {
            notebook.Add(new NoteListItem(n));
        }

        public static void LoadNotes()
        {
            string[] notes = Directory.GetFiles(Evennote.path);

            for (int i = 0; i < notes.Length; i++)
            {
                Note n = new Note();
                n.OpenFromFile(notes[i]);
                Add(n);
            }
        }

        public static void Delete(NoteListItem n)
        {
            try
            {
                File.Delete(String.Format("{0}{1}.note", Evennote.path, n.Title));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            notebook.Remove(n);
        }

        public static Note Last()
        {
            return notebook.Last().Content as Note;
        }

        public static void RemoveAt(int index)
        {
            notebook.RemoveAt(index);
        }

        public static int IndexOf(NoteListItem n)   
        {
            return notebook.IndexOf(n);
        }
    }
}

