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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.WebSockets;
using System.Threading;
using System.ComponentModel;

namespace WpfApplication2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>    


    public partial class MainWindow : Window
    {
        ClientWebSocket client;
        private MyData m_MyData;
        //private void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.messagelog.DataContext = this.data;
        //}

        public MainWindow()
        {
            InitializeComponent();
            
            client = new ClientWebSocket();

            MyData data = new MyData { MessageLog = "Test" };
            messagelog.DataContext = this.messagelog;
            //CancellationToken cancellation = new CancellationToken();


            //FRSWebSocket.CloseAsync((WebSocketCloseStatus)FRSWebSocket.CloseStatus,null,CancellationToken.None);

        }

        static async void StartReceiving(ClientWebSocket client)
        {
            while (true)
            {
                var array = new byte[4096];
                
                var result = await client.ReceiveAsync(new ArraySegment<byte>(array), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string msg = Encoding.UTF8.GetString(array, 0, result.Count);
                    System.Diagnostics.Debug.WriteLine(msg);
                    MessageBoxResult a = MessageBox.Show(msg, "TEST");
                    

                    //Console.ForegroundColor = ConsoleColor.DarkBlue;
                    //Console.WriteLine("--> {0}", msg);
                    //Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                   
                    MessageBoxResult a = MessageBox.Show("CLOSE","TEST");
                    if (a == MessageBoxResult.OK)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        break;
                    }                    
                }

                array = null;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //do my stuff before closing
            client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            //MessageBoxResult a = MessageBox.Show("HAHA", "TEST", MessageBoxButton.OKCancel);
            //if (a == MessageBoxResult.OK)
            //    MessageBox.Show("OK");
            base.OnClosing(e);
        }

        private void BtnConnect(object sender, RoutedEventArgs e)
        {
            string id = TextSessionID.Text.ToString();
          

            string line = string.Format("{{ \"session_id\" : \"{0}\" }}", id.ToString());  ;
            //while ((line = Console.ReadLine()) != "exit")
            try
            {
                var array = new ArraySegment<byte>(Encoding.UTF8.GetBytes(line));

                //cln.SendAsync(new ArraySegment<byte>("my message".GetBytesUtf8()), System.Net.WebSockets.WebSocketMessageType.Text, true, new CancellationToken()).Wait();
                client.SendAsync(array, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                string ss = ex.ToString();
            }
         }

        private void BtnClear(object sender, RoutedEventArgs e) {
            messagelog.Text = "";
        }

        private void BtnStart(object sender, RoutedEventArgs e)
        {
            string url = "ws://172.16.10.146:7077/frs/ws/fcsreconizedresult";
            try
            {
                client.ConnectAsync(new Uri(url), CancellationToken.None).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                //throw;
            }
            

            // var test = Console.ReadLine();

            StartReceiving(client);
        }

        private void BtnStop(object sender, RoutedEventArgs e)
        {
            client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public class MyData : INotifyPropertyChanged
        {
            string messageLog;
            public string MessageLog
            {
                get { return messageLog; }
                set
                {
                    messageLog = value;
                    NotifyChange("MessageLog");
                }
            }
            private void NotifyChange(params object[] properties)
            {
                if (PropertyChanged != null)
                {
                    foreach (string p in properties)
                        PropertyChanged(this, new PropertyChangedEventArgs(p));
                }
            }
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }
    }
}
