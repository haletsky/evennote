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
using System.Windows.Media;

namespace evenote
{
    /*
    Класс Evennote содержит основные методы и свойства программы такие как: информацию 
    о авторизированном пользователе, пути папок, режимы офлайнмода и автологина, синхронизация 
    заметок с сервером и т.д.
    */
    public static class Evennote
    {
        public static User user;
        public static User contextUser;
        public static string path;

        private static bool offlineMode = false;

        //Свойство офлайна. Может включатся вручную или по пропаже(?) интернета
        public static bool OfflineMode
        {
            get
            {
                if (!offlineMode)
                {
                    if (!GetInternetConnect())
                    {
                        offlineMode = true;
                    }
                }
                return offlineMode;
            }
            set
            {
                offlineMode = value;
            }
        }

        //Свойство "Remember me", котороей включается на странице логина
        public static bool AutoLogin { get; set; }

        //Свойство пути к файлу конфига, который содержит логин и пароль
        public static string ConfigFile { get { return String.Format("C:\\Users\\{0}\\Documents\\evennote\\config.ini", Environment.UserName); } }

        //Свойство пути папки "корзины" для заметок пользователя
        public static string DeleteDirectory { get; set; }

        public static void SetUserDirectory(string username)
        {

            Directory.CreateDirectory(String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, username));

            path = String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, username);

            DeleteDirectory = path + ".del";

            Directory.CreateDirectory(DeleteDirectory);
        }

        public static bool Authorization(string username, string password)
        {
            //Подключение к своей базе данных
            MyDataBase.ConnectToDB();

            //Проверяем совпадают ли имя и пароли
            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + username + "' AND users.userpass = '" + password + "'");

            //Проверка, нашли ли мы вообще такого пользователя в БД
            if (MyDataBase.rdr.HasRows == false)
            {
                //Надо сделать throwException //ай, потом..
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

            //Пишем что мы онлайн в БД
            MyDataBase.SetOnlineUser(user.id);

            //Отключаемся от БД
            MyDataBase.CloseConnectToDB();

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
                MessageBox.Show("This user already exist.");//Некрасиво тут содержать MessageBox, но всё же.
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

        public static Note OpenNoteFromFile(string pathnote)
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
                FileInfo s = new FileInfo(openFileDialog1.FileName);
                if(s.Length > 1000000)
                {
                    MessageBox.Show("Image size can't be over 1mb.");
                    return null;
                }
                
                return BitmapFrame.Create(new Uri(openFileDialog1.FileName));
            }
            return null;
        }

        public static void SyncNotes()
        {
            if (!GetInternetConnect()) return;

            //Удаление заметок из бд и из системы.
            MyDataBase.ConnectToDB();

            string[] notes = Directory.GetFiles(DeleteDirectory);

            for (int i = 0; i < notes.Length; i++)
            {
                MyDataBase.ExecuteCommand("DELETE FROM `evennote_db`.`notes` WHERE `iduser`='" + Evennote.user.id + "' AND `title`='" + notes[i].Split('\\').Last().Split('.').First() + "';");
                //Разбиваем путь по слэшам, берем имя файла с расширением. Разюиваем имя файла и вытягиваем имя заметки.
                File.Delete(notes[i]);

                MyDataBase.rdr.Close();
            }

            MyDataBase.ExecuteCommand("SELECT idnote, title, note, dateCreate, dateChanged FROM notes WHERE notes.iduser = " + user.id + ";");

            //написать проверки о наличии данных

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
                        flag = false;

                        if (DateTime.Compare(x.DateChanged, y.DateChanged) < 0)
                        {
                            //Когда на бд новая заметка, а у нас старая

                            x.DateChanged = y.DateChanged;
                            x.DateCreate = y.DateCreate;

                            x.Text = y.Text;

                            x.SaveToFile(String.Format("{0}{1}.note", Evennote.path, x.Title));

                            File.SetCreationTime(String.Format("{0}{1}.note", Evennote.path, x.Title), x.DateCreate);
                            File.SetLastWriteTime(String.Format("{0}{1}.note", Evennote.path, x.Title), x.DateChanged);

                            break;
                        }
                        else if (DateTime.Compare(x.DateChanged, y.DateChanged) > 0)
                        {
                            //Когда на бд старая заметка, а у нас новая

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

            //Когда у нас нет локальных заметок, и есть заметки в БД.
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
                    y.SaveToFile(String.Format("{0}{1}.note", Evennote.path, y.Title));
                    Notebook.notebook.Add(y);

                    File.SetCreationTime(String.Format("{0}{1}.note", Evennote.path, y.Title), y.DateCreate);
                    File.SetLastWriteTime(String.Format("{0}{1}.note", Evennote.path, y.Title), y.DateChanged);

                }
            }

            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            (((Application.Current.MainWindow as MainWindow).mainframe.Content as menu_page).frame.Content as notes_page).SyncListView();
        }

        public static void SetConfigurateFile()
        {
            StreamWriter sw = File.CreateText(ConfigFile);
            sw.Close();
        }

        public static string ReadConfigFile() { 
            return File.ReadAllText(ConfigFile);
        }

        public static void WriteConfigFile(string data)
        {
            File.WriteAllText(ConfigFile, data);
        }

        public static int GetCountNotesFromDB()
        {
            if (OfflineMode) return -1;
            MyDataBase.ConnectToDB();

            MyDataBase.ExecuteCommand("SELECT COUNT(idnote) FROM evennote_db.notes WHERE notes.iduser = "+ user.id +";");

            if (!MyDataBase.rdr.HasRows) return 0;

            int x = 0;

            while (MyDataBase.rdr.Read())
            {
                x = Convert.ToInt32((long)MyDataBase.rdr[0]);
            }

            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();

            return x;
        }

        public static bool GetInternetConnect()
        {
            bool isConnected = false;
            using (var tcpClient = new System.Net.Sockets.TcpClient())
            {
                tcpClient.SendTimeout = 10;
                tcpClient.ReceiveTimeout = 10;
                tcpClient.NoDelay = true;

                try {
                    tcpClient.Connect("64.233.161.113", 443); // google
                    isConnected = tcpClient.Connected;
                }
                catch { }
                finally
                {
                    isConnected = tcpClient.Connected;
                }
            }

            return isConnected;
        }
    }
}
