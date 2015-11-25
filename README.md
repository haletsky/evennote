# Evennote
Evennote - program for create, edit and save your notes online.
Users can create files(note), add formating text, lists and images. Synchronize notes with server.

## Structure Evennote interface:
MainWindow.xaml has just one frame, which contain pages (`login_page.xaml`, `regist_page.xaml`, `menu_page.xaml`). That pages contains program interface (controls) and another frame with pages(`editnote_page.xaml`, `notes_pages.xaml`, `profile_page.xaml`, `search_page.xaml`) where user can work.

## Classes
 * static class Evennote - main class for basics methods and properties whole program.
 * class Note - keep all information about note.
 * class NoteListItem - over-class for 'Note', have styles for ListViewItem.
 * class User - all information about user.
 * static class Notebook - contain users note, and methods for them.