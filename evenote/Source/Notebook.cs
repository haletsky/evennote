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

        //Необходимый буфер для редактирования, удаления выбранной в списке заметки
        public static Note rememberThis = null;

        public static void Add(Note n)
        {
            notebook.Add(n);
        }

        public static void LoadNotes()
        {
            string[] notes = Directory.GetFiles(Evennote.path, "*.note");

            for (int i = 0; i < notes.Length; i++)
            {
                Note n = new Note();
                n.OpenFromFile(notes[i]);
                notebook.Add(n);
            }
        }

        public static void Delete(Note n)
        {
            try
            {
                File.Move(String.Format("{0}{1}.note", Evennote.path, n.Title), String.Format("{0}\\{1}.note", Evennote.DeleteDirectory, n.Title));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            notebook.Remove(n);
        }

        public static Note Last()
        {
            return notebook.Last();
        }

        public static void RemoveAt(int index)
        {
            notebook.RemoveAt(index);
        }

        public static int IndexOf(Note n)   
        {
            return notebook.IndexOf(n);
        }
    }
}

