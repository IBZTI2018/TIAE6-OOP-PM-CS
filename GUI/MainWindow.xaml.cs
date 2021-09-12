
using System;
using System.Windows;
using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;
using Shared.Contracts;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Generic;
using Shared.Models;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        GrpcChannel DBChannel;
        GrpcChannel InferenceChannel;
        GrpcChannel TaxChannel;

        public void checkSystemStatus()
        {
            var channels = new List<(dynamic circle, GrpcChannel channel)> { 
                (dbStatusCircle, DBChannel), 
                (inferenceStatusCircle, InferenceChannel), 
                (taxStatusCircle, TaxChannel) 
            };
            foreach (var channel in channels) {
                channel.circle.Fill = new SolidColorBrush(Colors.Gray);
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    try
                    {
                        IStatusService statusService = channel.channel.CreateGrpcService<IStatusService>();
                        StatusResponse response = await statusService.getStatus(new StatusRequest { ping = 1 });
                        if (response.pong != 1)
                        {
                            throw new Exception();
                        }

                        Dispatcher.Invoke(() => {
                            // Update GUI inside async func
                            channel.circle.Fill = new SolidColorBrush(Colors.LightGreen);
                        });
                    }
                    catch (Exception e)
                    {
                        Dispatcher.Invoke(() => {
                            // Update GUI inside async func
                            channel.circle.Fill = new SolidColorBrush(Colors.Red);
                        });
                    }
                });
            }
        }

        private async void loadPersons()
        {
            IPersonService personService = DBChannel.CreateGrpcService<IPersonService>();
            PersonListResponse response = await personService.getPersonAll(new EmptyRequest());
            foreach (Person person in response.personList)
            {
                Dispatcher.Invoke(() => {
                    this.personListView.Items.Add(person);
                });
            }
        }

        public MainWindow()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            DBChannel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.DB_PORT);
            InferenceChannel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.INFERENCE_PORT);
            TaxChannel = GrpcChannel.ForAddress(Shared.Network.BASE_HOST + Shared.Network.TAX_PORT);

            InitializeComponent();
            checkSystemStatus();
            loadPersons();
        }

        protected virtual void Dispose()
        {
            DBChannel.Dispose();
            InferenceChannel.Dispose();
            TaxChannel.Dispose();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            checkSystemStatus();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            IInferenceService service = this.InferenceChannel.CreateGrpcService<IInferenceService>();
            BoolResponse response = await service.reloadRules(new EmptyRequest());
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ITaxCalculatorService service = this.TaxChannel.CreateGrpcService<ITaxCalculatorService>();
            BoolResponse response = await service.reloadRules(new EmptyRequest());
        }
    }
}
