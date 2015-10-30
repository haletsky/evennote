using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace evenote
{
    public class NoteListItem : ListViewItem
    {
        public NoteListItem(Note n)
        {
            Content = n;
            ContentStringFormat = "lel";
            
        }

        public string Title { get { return (Content as Note).Title; } set { (Content as Note).Title = value; } }
        public DateTime DateNotice { get { return (Content as Note).DateNotice; } set { (Content as Note).DateNotice = value; } }
        public DateTime DateCreate { get { return (Content as Note).DateCreate; } set { (Content as Note).DateCreate = value;  } }
        public FlowDocument Text { get { return (Content as Note).Text; } set { (Content as Note).Text = value; } }

    }
}
