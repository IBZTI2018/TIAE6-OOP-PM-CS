
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

        public async void buildTreeViews()
        {
            List<Rule> inferrenceRules = await this.windowController.getAllInferrenceRules();
            List<Rule> evaluationRules = await this.windowController.getAllEvaluationRules();

            this.buildTreeView(this.inferenceRulesView, inferrenceRules);
            this.buildTreeView(this.evaluationRulesView, evaluationRules);
        }
    
        private async void loadPersons()
        {
            List<Person> persons = await this.windowController.getAllPersons();
            this.personListView.Items.Clear();
            foreach (Person person in persons)
            {
                Dispatcher.Invoke(() => {
                    this.personListView.Items.Add(person);
                });
            }
        }

        private async void loadTaxDeclarations()
        {
            List<TaxDeclaration> declarations = await this.windowController.getAllTaxDeclarations();
            this.taxDeclarationListView.Items.Clear();
            foreach (TaxDeclaration declaration in declarations)
            {
                Dispatcher.Invoke(() => {
                    this.taxDeclarationListView.Items.Add(declaration);
                });
            }
        }
    
        private void buildTreeView(TreeView tree, List<Rule> rules)
        {
            tree.Items.Clear();

            Dictionary<int, TreeViewItem> children = new Dictionary<int, TreeViewItem>();
            foreach (var rule in rules)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = rule.rule;
                item.Tag = rule.id;
                item.IsExpanded = true;

                if (tree == this.inferenceRulesView) this.addInferrenceItemSelectHandler(item);
                if (tree == this.evaluationRulesView) this.addEvaluatorItemSelectHandler(item);

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

        public void addInferrenceItemSelectHandler(TreeViewItem item)
        {
            item.Selected += new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                Rule rule =this.windowController.getInferenceRule(Convert.ToInt32((e.Source as TreeViewItem).Tag));
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

        public void addEvaluatorItemSelectHandler(TreeViewItem item)
        {
            item.Selected += new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                Rule rule = this.windowController.getEvaluationRule(Convert.ToInt32((e.Source as TreeViewItem).Tag));
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

        public MainWindow()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            this.windowController = new MainWindowController();

            // Initialize form components
            InitializeComponent();

            this.checkSystemStatus();
            this.loadPersons();
            this.loadTaxDeclarations();
            this.buildTreeViews();
        }

        protected virtual void Dispose()
        {
            this.windowController.teardown();
            this.windowController = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.checkSystemStatus();
            this.loadPersons();
            this.loadTaxDeclarations();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.windowController.reloadRulesFor("inference");
                MessageBox.Show("Inferenzmotor hat seine Regeln neu geladen.");
            } catch
            {
                MessageBox.Show("Inferenzmotor konnte seine Regeln nicht neu laden.");
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.windowController.reloadRulesFor("evaluator");
                MessageBox.Show("Inferenzmotor hat seine Regeln neu geladen.");
            }
            catch
            {
                MessageBox.Show("Steuerberechner konnte seine Regeln nicht neu laden.");
            }
        }
    }
}
