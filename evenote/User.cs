using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Net;

namespace evenote
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public BitmapImage avatar { get; set; }
        public DateTime datebirth { get; set; }

        public bool online;

        public User(int i, string un, string eml, byte[] icon, DateTime? db)
        {
            id = i; username = un; email = eml; datebirth = db.Value;
            avatar = LoadImage(icon);
        }

        //Метод для конвертации картинки из байтов.
        private static BitmapImage LoadImage(byte[] imageData)
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
    }
}
