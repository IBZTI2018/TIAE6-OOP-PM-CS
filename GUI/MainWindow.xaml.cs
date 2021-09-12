
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

        public async void checkSystemStatus()
        {
            var channels = new List<(dynamic circle, GrpcChannel channel)> { 
                (dbStatusCircle, DBChannel), 
                (inferenceStatusCircle, InferenceChannel), 
                (taxStatusCircle, TaxChannel) 
            };
            foreach (var channel in channels) {
                channel.circle.Fill = new SolidColorBrush(Colors.Gray);
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
            }
        }

        private async void loadPersons()
        {
            try
            {
                IPersonService personService = DBChannel.CreateGrpcService<IPersonService>();
                PersonListResponse response = await personService.getPersonAll(new EmptyRequest());
                this.personListView.Items.Clear();
                foreach (Person person in response.personList)
                {
                    Dispatcher.Invoke(() => {
                        this.personListView.Items.Add(person);
                    });
                }
            }
            catch
            {

            }
            
        }

        private async void loadTaxDeclarations()
        {
            ITaxDeclarationService taxDeclarationService = DBChannel.CreateGrpcService<ITaxDeclarationService>();
            TaxDeclarationListResponse response = await taxDeclarationService.getAllTaxDeclarations(new EmptyRequest());
            this.taxDeclarationListView.Items.Clear();
            foreach (TaxDeclaration declaration in response.declarationList)
            {
                Dispatcher.Invoke(() => {
                    this.taxDeclarationListView.Items.Add(declaration);
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
            loadTaxDeclarations();
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
            loadPersons();
            loadTaxDeclarations();
        }
    }
}
