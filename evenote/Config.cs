using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace evenote
{
    //Класс-конфигурация программы
    public static class Config
    {
        public static string path;

        public static void ConfigureProgram()
        {
            //Существует ли папка хранения заметок на компьютере? Если нет, то саздаем.
            if(!Directory.Exists(String.Format("C:\\Users\\{0}\\Documents\\evennote\\", Environment.UserName))){
                Directory.CreateDirectory(String.Format("C:\\Users\\{0}\\Documents\\evennote\\", Environment.UserName));                
            }

            path = String.Format("C:\\Users\\{0}\\Documents\\evennote\\", Environment.UserName);
        }

        public static void SetUserDirectory(string user)
        {
            if (!Directory.Exists(String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, user)))
            {
                Directory.CreateDirectory(String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, user));
            }

            path = String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, user);
        }
    }
}
