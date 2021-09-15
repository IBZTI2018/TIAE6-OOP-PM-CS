
using System;
using System.Windows;
using System.Windows.Controls;
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

    public partial class MainWindow : Window {
        GrpcChannel DBChannel;
        GrpcChannel InferenceChannel;
        GrpcChannel TaxChannel;

        private List<Rule> inferenceRules;
        private List<Rule> evaluationRules;

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

        private async void loadInferenceRules()
        {
            IRuleService ruleService = DBChannel.CreateGrpcService<IRuleService>();
            InferenceRulesResponse inferenceRules = await ruleService.getInferenceRules(new EmptyRequest());
            this.inferenceRules = inferenceRules.rules.ConvertAll(x => (Rule)x);
            this.buildTree(this.inferenceRulesView, this.inferenceRules);
        }

        private async void loadEvaluationRules()
        {
            IRuleService ruleService = DBChannel.CreateGrpcService<IRuleService>();
            EvaluationRulesResponse evaluationRules = await ruleService.getEvaluationRules(new EmptyRequest());
            this.evaluationRules = evaluationRules.rules.ConvertAll(x => (Rule)x);
            this.buildTree(this.evaluationRulesView, this.evaluationRules);
        }

        private void buildTree(TreeView tree, List<Rule> rules)
        {
            tree.Items.Clear();

            Dictionary<int, TreeViewItem> children = new Dictionary<int, TreeViewItem>();
            foreach (var rule in rules)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = rule.rule;
                item.Tag = rule.id;
                item.IsExpanded = true;

                // This is not an ideal way of adding listeners at all...
                if (tree == this.inferenceRulesView)
                {
                    item.Selected += new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
                    {
                        Rule rule = this.inferenceRules.Find(x => x.id == Convert.ToInt32((e.Source as TreeViewItem).Tag));
                        this.inferenceRuleId.Text = Convert.ToString(rule.id);
                        this.inferenceRuleName.Text = rule.rule;
                        this.inferenceRuleCondition.Text = rule.condition;
                        this.inferenceRuleTransformation.Text = rule.transformation;
                        if (rule.parent != null)
                        {
                            this.inferenceRuleParent.Text = Convert.ToString(rule.parent.id);
                        }
                        else
                        {
                            this.inferenceRuleParent.Text = "-";
                        }
                    });
                }

                if (tree == this.evaluationRulesView)
                {
                    item.Selected += new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
                    {
                        Rule rule = this.evaluationRules.Find(x => x.id == Convert.ToInt32((e.Source as TreeViewItem).Tag));
                        this.evaluationRuleId.Text = Convert.ToString(rule.id);
                        this.evaluationRuleName.Text = rule.rule;
                        this.evaluationRuleCondition.Text = rule.condition;
                        this.evaluationRuleTransformation.Text = rule.transformation;
                        if (rule.parent != null)
                        {
                            this.evaluationRuleParent.Text = Convert.ToString(rule.parent.id);
                        }
                        else
                        {
                            this.evaluationRuleParent.Text = "-";
                        }
                    });
                }

                children[rule.id] = item;

                // If the rule has a parent, we look to append it
                if (rule.parent != null && rule.parent.id != 0)
                {
                    if (children.ContainsKey(rule.parent.id))
                    {
                        children[rule.parent.id].Items.Add(item);
                        continue;
                    }
                }

                // Otherwise just add it to the bottom.
                tree.Items.Add(item);
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
            loadInferenceRules();
            loadEvaluationRules();
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
