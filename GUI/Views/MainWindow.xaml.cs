
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
using GUI.Controllers;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window {
        private MainWindowController windowController;

        private List<Rule> inferenceRules;
        private List<Rule> evaluationRules;

        public async void checkSystemStatus()
        {
            var statuses = await this.windowController.getServiceStatus();

            var services = new List<(dynamic circle, bool status)> {
                (this.dbStatusCircle, statuses["database"]),
                (this.inferenceStatusCircle, statuses["inference"]),
                (this.taxStatusCircle, statuses["evaluator"])
            };

            foreach (var service in services) {
                service.circle.Fill = new SolidColorBrush(Colors.Gray);
                await Task.Delay(500);
               
                Color color = service.status ? Colors.LightGreen : Colors.Red;

                Dispatcher.Invoke(() => {
                    service.circle.Fill = new SolidColorBrush(color);
                });
            }
        }
    /*
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
        }*/

        public MainWindow()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            this.windowController = new MainWindowController();

            // Initialize form components
            InitializeComponent();

            this.checkSystemStatus();
        }

        protected virtual void Dispose()
        {
            this.windowController.teardown();
            this.windowController = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            checkSystemStatus();
            // loadPersons();
            // loadTaxDeclarations();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await this.windowController.reloadRulesFor("inference");
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await this.windowController.reloadRulesFor("evaluator");
        }
    }
}
