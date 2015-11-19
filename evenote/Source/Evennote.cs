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
        }

        public static void Authorization(string username, string password)
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
                return;
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
            MyDataBase.ChangeOnlineStatus(user.id);
            user.online = true;

            //Отключаемся от БД
            MyDataBase.CloseConnectToDB();

            //Создаем пользовательский каталог, если его нет
            SetUserDirectory(username);

            //Считываем с диска существующие заметки
            Notebook.LoadNotes();
        }

        public static User GetUserData(string username)
        {
            User temp= null;
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

        public static void Registration(string username, string password, DateTime birth, string email, byte[] avatar)
        {
            MyDataBase.ConnectToDB();

            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = '" + username + "' OR users.email = '" + email + "'");

            if (MyDataBase.rdr.HasRows)
            {
                MessageBox.Show("This user already exist.");
                return;
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
    }
}
