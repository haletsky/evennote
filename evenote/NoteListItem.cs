using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace evenote
{
    public class NoteListItem : ListBoxItem
    {
        public NoteListItem(Note n)
        {
            Content = n;    
            //
        }

        public string Title { get { return (Content as Note).Title; } set { } }
        public DateTime DateNotice { get { return (Content as Note).DateNotice; } set { } }
        public DateTime DateCreate { get { return (Content as Note).DateCreate; } set { } }
        public FlowDocument Text { get { return (Content as Note).Text; } set { } }

    }
}
