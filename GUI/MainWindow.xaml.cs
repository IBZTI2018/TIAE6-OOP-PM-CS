using System.Windows;
using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using DB.Contracts;
using System;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var http = GrpcChannel.ForAddress("http://localhost:" + DB.Program.DB_PORT))
            {
                IPersonService personService = http.CreateGrpcService<IPersonService>();
                PersonResponse response = await personService.getPerson(new IDRequest { id = Int32.Parse(personId.Text) });
                MessageBox.Show(response.person.firstName);
            }
        }
    }
}
