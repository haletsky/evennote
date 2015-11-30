using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace evenote
{
    //Класс-обёртка для класса Note, имеющая уникальный стиль для ListView
    public class NoteListItem : ListViewItem
    {
        public NoteListItem(Note n)
        {
            Content = n;
            this.Template = (ControlTemplate)this.FindResource("TryListViewItemWork");
        }

        public NoteListItem()
        {
            this.Template = (ControlTemplate)this.FindResource("TryListViewItemWork");
        }

        /*
        Возвращает необходимую картинку под статус заметки
        */
        public string Backuped {
            get {
                if (Evennote.OfflineMode) return @"images\notelocal.png";

                //(Content as Note).RefreshNoteState(Evennote.user.id); //Чтобы вдвое уменьшить запросы к БД, вызываем RefreshNoteState() только у ToolTipText.

                if ((Content as Note).Backuped == -2)
                {
                    return @"images\notelocal.png";
                }
                else if ((Content as Note).Backuped == -1)
                {
                    return @"images\noteneedsync.png";
                }
                else if ((Content as Note).Backuped == 0)
                {
                    return @"images\notesynched.png";
                }
                else if ((Content as Note).Backuped == 1)
                {
                    return @"images\noteneedsync.png";
                }
                return "";
            }
        }
        public string ToolTipText
        {
            get
            {
                if (Evennote.OfflineMode) return @"Local note.";

                (Content as Note).RefreshNoteState(Evennote.user.id);

                if ((Content as Note).Backuped == -2) return "Local note.";
                else if ((Content as Note).Backuped == -1) return "Need to sync!";
                else if ((Content as Note).Backuped == 0) return "Synched note.";
                else if ((Content as Note).Backuped == 1) return "Need to sync!";
                return "";
            }
        }
        public string Title { get { return (Content as Note).Title; } set { (Content as Note).Title = value; } }
        public DateTime DateCreate { get { return (Content as Note).DateCreate; } set { (Content as Note).DateCreate = value;  } }
        public DateTime DateChanged { get { return (Content as Note).DateChanged; } set { (Content as Note).DateChanged = value; } }
        public FlowDocument Text { get { return (Content as Note).Text; } set { (Content as Note).Text = value; } }
        public string GetShortDateCreate { get { return DateCreate.ToShortDateString() + " " + DateCreate.ToShortTimeString(); } }
        public string GetShortDateChanged { get { return DateChanged.ToShortDateString() + " " + DateChanged.ToShortTimeString(); } }
    }
}
