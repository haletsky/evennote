# Evennote
Evennote - program for create, edit and save your notes online.
Users can create files(note), add formating text, lists and images. Synchronize notes with server.

![](http://cs629519.vk.me/v629519798/2cf17/2NCAoffjzws.jpg)

## Structure Evennote interface:
MainWindow.xaml has just one frame, which contain pages (`login_page.xaml`, `regist_page.xaml`, `menu_page.xaml`). That pages contains program interface (controls) and another frame with pages(`editnote_page.xaml`, `notes_pages.xaml`, `profile_page.xaml`, `search_page.xaml`) where user can work.

![](http://cs629519.vk.me/v629519798/2cf29/w74H3yBhyy0.jpg)

## Classes
 * static class Evennote - main class for basics methods and properties whole program.
 * class Note - keep all information about note.
 * class NoteListItem - over-class for 'Note', have styles for ListViewItem.
 * class User - all information about user.
 * static class Notebook - contain users note, and methods for them.

![](http://cs629519.vk.me/v629519798/2cf32/p323pqnRDk0.jpg)

 P.S. DataBaseAPI.dll it is my class for connect to own DataBase.
