using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseAPI;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using Microsoft.Win32;
using evenote.pages;


namespace evenote
{
    public static class Evennote
    {
        public static User user;
        public static User contextUser;
        public static string path;

        public static void SetUserDirectory(string username)
        {
            if (!Directory.Exists(String.Format("C:\\Users\\{0}\\Documents\\evennote\\", Environment.UserName)))
            {
                Directory.CreateDirectory(String.Format("C:\\Users\\{0}\\Documents\\evennote\\", Environment.UserName));
            }

            if (!Directory.Exists(String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, user)))
            {
                Directory.CreateDirectory(String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, username));
            }

            path = String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, username);

            Directory.CreateDirectory(path + ".del");
        }

        public static bool Authorization(string username, string password)
        {
            //Подключение к своей базе данных
            MyDataBase.ConnectToDB();

            //Проверяем совпадают ли имя и пароли
            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + username + "' AND users.userpass = '" + password + "'");

            if (MyDataBase.rdr.HasRows == false)
            {
                //Надо сделать throwException
                MyDataBase.rdr.Close();
                MyDataBase.CloseConnectToDB();
                return false;
            }

            //Читаем данные
            while (MyDataBase.rdr.Read())
            {
                //Сохраняем данные о себе
                user = new User(Convert.ToInt32(MyDataBase.rdr[0].ToString()),
                    MyDataBase.rdr[1].ToString(),
                    MyDataBase.rdr[3].ToString(),
                    MyDataBase.rdr[4] as byte[],
                    MyDataBase.rdr[5] as DateTime?);
            }

            contextUser = user;

            MyDataBase.rdr.Close();

            //Пишем что мы онлайн
            MyDataBase.SetOnlineUser(user.id);
            user.online = true;

            //Отключаемся от БД
            MyDataBase.CloseConnectToDB();

            SetUserDirectory(username);

            //Считываем с диска существующие заметки
            Notebook.LoadNotes();

            return true;
        }

        public static User GetUserData(string username)
        {
            User temp = null;
            //Подключение к своей базе данных
            MyDataBase.ConnectToDB();
            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = " + "'" + username + "'");

            if (MyDataBase.rdr.HasRows == false)
            {
                return null;
            }

            while (MyDataBase.rdr.Read())
            {
                //Сохраняем данные о пользователе
                temp = new User(Convert.ToInt32(MyDataBase.rdr[0].ToString()),
                    MyDataBase.rdr[1].ToString(),
                    MyDataBase.rdr[3].ToString(),
                    MyDataBase.rdr[4] as byte[],
                    MyDataBase.rdr[5] as DateTime?);

                if (MyDataBase.rdr[6].ToString().Equals("True"))
                {
                    temp.online = true;
                }
            }

            //Отключаемся от БД
            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            return temp;
        }

        public static bool Registration(string username, string password, DateTime birth, string email, byte[] avatar)
        {
            MyDataBase.ConnectToDB();

            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + username + "' OR users.email = '" + email + "'");

            if (MyDataBase.rdr.HasRows)
            {
                MessageBox.Show("This user already exist.");
                return false;
            }

            MyDataBase.rdr.Close();
            MyDataBase.AddWithValue("@avatar", avatar);
            MyDataBase.ExecuteCommand(
                "INSERT INTO `evennote_db`.`users` (`username`, `userpass`, `email`, `avatar`, `datebirth`)" +
                "VALUES ('" + username + "', '" + password + "', '" + email + "', @avatar, '"
                + birth.Year + "-"
                + birth.Month + "-"
                + birth.Day + "');");

            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            return true;
        }

        public static Note OpenNote(string pathnote)
        {
            Note temp = new Note();
            using (FileStream fs = new FileStream(pathnote, FileMode.Open, FileAccess.ReadWrite))
            {
                TextRange textRange = new TextRange(
                    temp.Text.ContentStart,
                    temp.Text.ContentEnd);
                textRange.Load(fs, DataFormats.XamlPackage);

                temp.DateCreate = File.GetCreationTime(pathnote);
                temp.Title = pathnote.Substring(pathnote.LastIndexOf('\\') + 1, (pathnote.Substring(pathnote.LastIndexOf('\\') + 1).Length - 5));
            }
            return temp;
        }

        public static BitmapImage ConvertImageToByte(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public static BitmapFrame OpenImageInWindow()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            openFileDialog1.Filter = @"Bitmap File(*.bmp)|*.bmp|" +
                @"GIF File(*.gif)|*.gif|" +
                @"JPEG File(*.jpg)|*.jpg|" +
                @"TIF File(*.tif)|*.tif|" +
                @"PNG File(*.png)|*.png";

            openFileDialog1.FilterIndex = 3;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                return BitmapFrame.Create(new Uri(openFileDialog1.FileName));
            }
            return null;
        }

        public static void SyncNotes()
        {
            MyDataBase.ConnectToDB();

            string[] notes = Directory.GetFiles(path + ".del\\");

            for (int i = 0; i < notes.Length; i++)
            {
                MyDataBase.ExecuteCommand("DELETE FROM `evennote_db`.`notes` WHERE `iduser`='" + Evennote.user.id + "' AND `title`='" + notes[i].Split('\\').Last().Split('.').First() + "';");
                //Разбиваем путь по слэшам, берем имя файла с расширением. Разюиваем имя файла и вытягиваем имя заметки.
                File.Delete(path + ".del\\" + notes[i].Split('\\').Last());
                MyDataBase.rdr.Close();
            }

            MyDataBase.ExecuteCommand("SELECT idnote, title, note, dateCreate, dateChanged FROM notes WHERE notes.iduser = " + user.id + ";");

            int idnote = -1;
            List<Note> fromDB = new List<Note>();

            while (MyDataBase.rdr.Read())
            {
                Note buf = new Note();
                using (MemoryStream mem = new MemoryStream((byte[])MyDataBase.rdr[2]))
                {
                    TextRange textRange = new TextRange(
                        buf.Text.ContentStart,
                        buf.Text.ContentEnd);
                    textRange.Load(mem, DataFormats.XamlPackage);

                    buf.Id = (int)MyDataBase.rdr[0];
                    buf.Title = (string)MyDataBase.rdr[1];
                    buf.DateCreate = new DateTime((long)MyDataBase.rdr[3]);
                    buf.DateChanged = new DateTime((long)MyDataBase.rdr[4]);

                    fromDB.Add(buf);
                }
            }
            MyDataBase.rdr.Close();

            Notebook.notebook.ForEach(delegate (Note x) //коллекция локальных заметок
            {
                bool flag = true;
                foreach (Note y in fromDB) //коллекция заметок из бд
                {                  
                    if (y.Title == x.Title)
                    {
                        y.DateChanged = y.DateChanged;
                        y.DateCreate = y.DateCreate;
                        flag = false;
                        //Время взятое из БД почему то не равняется времени локальному, хотя все базовые значения совпадают. 

                        if (DateTime.Compare(x.DateChanged, y.DateChanged) < 0)
                        {
                            //select
                            y.SaveToFile(String.Format("{0}{1}.note", path, y.Title));
                            File.SetCreationTime(String.Format("{0}{1}.note", path, y.Title), y.DateCreate);
                            File.SetLastWriteTime(String.Format("{0}{1}.note", path, y.Title), y.DateChanged);
                            //Notebook.Delete(x);
                            Notebook.Add(y);
                            break;
                        }
                        else if (DateTime.Compare(x.DateChanged, y.DateChanged) > 0)
                        {
                            //update
                            idnote = y.Id;
                            
                            using (MemoryStream mem = new MemoryStream())
                            {
                                TextRange textRange = new TextRange(
                                    x.Text.ContentStart,
                                    x.Text.ContentEnd);
                                textRange.Save(mem, DataFormats.XamlPackage);

                                MyDataBase.AddWithValue("@notefile", mem.ToArray());
                                MyDataBase.ExecuteCommand("UPDATE notes SET note = @notefile, dateChanged = '" + x.DateChanged.Ticks + "' WHERE notes.idnote = " + idnote);
                            }
                            break;
                        }
                    }
                }

                //Добавляем новую заметку в БД
                if (flag)
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        TextRange textRange = new TextRange(
                            x.Text.ContentStart,
                            x.Text.ContentEnd);
                        textRange.Save(mem, DataFormats.XamlPackage);

                        DateTime cR = x.DateCreate;
                        DateTime cH = x.DateChanged;

                        MyDataBase.AddWithValue("@notefile", mem.ToArray());
                        MyDataBase.ExecuteCommand("INSERT INTO `evennote_db`.`notes` (`iduser`, `title`, `note`, `dateCreate`, `dateChanged`) VALUES (" + user.id + ", '" + x.Title + "', @noteFile, " + cR.Ticks + ", " + cH.Ticks + ");");
                    }
                }
            });

            foreach (Note y in fromDB) //коллекция заметок из бд
            {
                bool flag = true;
                Notebook.notebook.ForEach(delegate (Note x) //коллекция локальных заметок
                {
                    if (y.Title == x.Title)
                    {
                        flag = false;
                    }
                });

                if (flag)
                {
                    y.DateChanged = y.DateChanged;
                    y.DateCreate = y.DateCreate;
                    y.SaveToFile(String.Format("{0}{1}.note", Evennote.path, y.Title));
                    Notebook.notebook.Add(y);
                    
                    File.SetCreationTime(String.Format("{0}{1}.note", Evennote.path, y.Title), y.DateCreate);
                    File.SetLastWriteTime(String.Format("{0}{1}.note", Evennote.path, y.Title), y.DateChanged);
                    (((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Content as notes_page).SyncListView();
                }
            }
            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();
        }
    }
}
