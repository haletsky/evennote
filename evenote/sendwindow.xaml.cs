using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataBaseAPI;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;


namespace evenote
{
    /// <summary>
    /// Interaction logic for sendwindow.xaml
    /// </summary>
    public partial class sendwindow : Window
    {
        Note note;
        public sendwindow(Note n)
        {
            InitializeComponent();
            note = n;
        }

        //ЭТО НЕ РАБОТАЕТ И НЕ НУЖНО НО И НЕ УДАЛЯТЬ
        private void button_Click(object sender, RoutedEventArgs e)
        {
            MyDataBase.ConnectToDB();
            MyDataBase.ExecuteCommand("SELECT * FROM users WHERE users.username = " + "'" + textBox.Text + "'");
            prbar.Value += 25;
            IPAddress ip = null;

            if (MyDataBase.rdr == null) return;
            while (MyDataBase.rdr.Read())
            {
                if (!MyDataBase.rdr[6].ToString().Equals("0"))
                {
                    ip = System.Net.IPAddress.Parse(MyDataBase.rdr[6].ToString());
                }
            }

            //Отключаемся от БД
            MyDataBase.rdr.Close();
            MyDataBase.CloseConnectToDB();
            prbar.Value += 25;
            try
            {
                TcpClient newClient = new TcpClient();

                // Соединяемся с сервером
                newClient.Connect(ip, 11000); // В этот момент сокет
                                              // порождает исключение, если
                                              // при соединении возникают проблемы

                prbar.Value += 25;
                NetworkStream stream = newClient.GetStream();

                byte[] answer = new byte[1];//Байтик удачного ответа.
                byte[] sendBytes = Encoding.UTF8.GetBytes(note.Title);//Отсылаемые данные.

                //Посылает тайтл заметки
                stream.Write(sendBytes, 0, sendBytes.Length);

                //Ждем ответ
                stream.Read(answer, 0, 1);
                //Если ответ "да", продолжаем 
                if (answer[0] != (byte)1)
                {
                    newClient.Close();
                    Close();
                    MessageBox.Show("ERROR");
                }

                sendBytes = Encoding.UTF8.GetBytes(note.DateCreate.ToString());

                //Посылает дату создания
                stream.Write(sendBytes, 0, sendBytes.Length);

                newClient.Close();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Exception: " + ex.ToString());
            }
            prbar.Value += 25;

            Close();
        }
    }
}
