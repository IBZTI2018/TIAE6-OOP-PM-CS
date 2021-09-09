
using System;
using System.Windows;
using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using Shared.Contracts;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Generic;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void checkSystemStatus()
        {
            var services = new List<(dynamic circle, int port)> { 
                (dbStatusCircle, Shared.Ports.DB_PORT), 
                (inferenceStatusCircle, Shared.Ports.INFERENCE_PORT), 
                (taxStatusCircle, Shared.Ports.TAX_PORT) 
            };
            foreach (var service in services) {
                service.circle.Fill = new SolidColorBrush(Colors.Gray);
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    try
                    {
                        using (var http = GrpcChannel.ForAddress("http://localhost:" + service.port.ToString()))
                        {
                            IStatusService statusService = http.CreateGrpcService<IStatusService>();
                            StatusResponse response = await statusService.getStatus(new StatusRequest { ping = 1 });
                            if (response.pong != 1)
                            {
                                throw new Exception();
                            }
                        }

                        Dispatcher.Invoke(() => {
                            // Update GUI inside async func
                            service.circle.Fill = new SolidColorBrush(Colors.Blue);
                        });
                    }
                    catch (Exception e)
                    {
                        Dispatcher.Invoke(() => {
                            // Update GUI inside async func
                            service.circle.Fill = new SolidColorBrush(Colors.Red);
                        });
                    }
                });
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            checkSystemStatus();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var http = GrpcChannel.ForAddress("http://localhost:" + Shared.Ports.DB_PORT))
            {
                IPersonService personService = http.CreateGrpcService<IPersonService>();
                PersonResponse response = await personService.getPerson(new IDRequest { id = Int32.Parse(personId.Text) });
                MessageBox.Show(response.person.firstName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            checkSystemStatus();
        }
    }
}
