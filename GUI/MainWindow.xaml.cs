using System;
using System.Windows;
using Grpc.Core;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Channel dbChannel = null;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                this.dbChannel = new Channel("127.0.0.1:" + DB.Program.DB_PORT, ChannelCredentials.Insecure);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception encountered: {ex}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var client = new PersonService.PersonServiceClient(this.dbChannel);
            PersonResponse response = client.GetPerson(new PersonRequest{ Id = personId.Text });
            MessageBox.Show(response.FirstName + " " + response.LastName);
        }
    }
}
