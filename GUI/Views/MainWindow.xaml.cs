
using System;
using System.Windows;
using System.Windows.Controls;
using ProtoBuf.Grpc.Client;
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
        private InferenceRule currentInferenceRule;
        private EvaluationRule currentEvaluationRule;

        /// <summary>
        /// Check the status of all linked systems
        /// </summary>
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
                await Task.Delay(100);
               
                Color color = service.status ? Colors.LightGreen : Colors.Red;

                Dispatcher.Invoke(() => {
                    service.circle.Fill = new SolidColorBrush(color);
                });
            }
        }

        /// <summary>
        /// Build the tree views for displaying rules
        /// </summary>
        /// <returns>An awaitabl task</returns>
        public async void buildTreeViews()
        {
            try
            {
                List<Rule> inferrenceRules = await this.windowController.getAllInferrenceRules();
                List<Rule> evaluationRules = await this.windowController.getAllEvaluationRules();

                this.buildTreeView(this.inferenceRulesView, inferrenceRules);
                this.buildTreeView(this.evaluationRulesView, evaluationRules);
            } catch {
                MessageBox.Show("Fehler beim Laden der Regeldaten aus der Datenbank.");
            }
        }
    
        /// <summary>
        /// Load all persons from the database
        /// </summary>
        private async void loadPersons()
        {
            try
            {
                List<Person> persons = await this.windowController.getAllPersons();
                this.personComboBox.Items.Clear();
                foreach (Person person in persons)
                {
                    Dispatcher.Invoke(() => {
                        this.personComboBox.Items.Add(person);
                    });
                }
            } catch
            {
                MessageBox.Show("Fehler beim Laden der Personendaten aus der Datenbank");
            }
        }

        /// <summary>
        /// Load all tax declarations from the database
        /// </summary>
        private async void loadTaxDeclarations()
        {
            try
            {
                List<TaxDeclaration> declarations = await this.windowController.getAllTaxDeclarations();
                this.taxDeclarationListView.Items.Clear();
                foreach (TaxDeclaration declaration in declarations)
                {
                    Dispatcher.Invoke(() => {
                        this.taxDeclarationListView.Items.Add(declaration);
                    });
                }
            } catch
            {
                MessageBox.Show("Fehler beim Laden der Steuererklärungen aus der Datenbank");
            }
        }
    
        /// <summary>
        /// Build a tree view for a set of rules
        /// </summary>
        /// <param name="tree">The output tree view</param>
        /// <param name="rules">The list of rules to display</param>
        private void buildTreeView(TreeView tree, List<Rule> rules)
        {
            tree.Items.Clear();

            Dictionary<int, TreeViewItem> children = new Dictionary<int, TreeViewItem>();
            foreach (var rule in rules)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = rule.rule;
                if (rule.active == false)
                {
                    item.Header += " (DEAKTIVIERT)";
                }
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

        /// <summary>
        /// Add event handler to an inference tree node
        /// </summary>
        /// <param name="item">The tree view item to add to</param>
        public void addInferrenceItemSelectHandler(TreeViewItem item)
        {
            item.Selected += new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                Rule rule =this.windowController.getInferenceRule(Convert.ToInt32((e.Source as TreeViewItem).Tag));
                this.currentInferenceRule = (InferenceRule)rule;

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

                if (rule.active)
                {
                    this.inferenceRuleToggleActive.Content = "Deaktivieren";
                }
                else
                {
                    this.inferenceRuleToggleActive.Content = "Aktivieren";
                }

            });
        }

        /// <summary>
        /// Add event handler to an evaluation tree node
        /// </summary>
        /// <param name="item">The tree view item to add to</param>
        public void addEvaluatorItemSelectHandler(TreeViewItem item)
        {
            item.Selected += new RoutedEventHandler(delegate (Object o, RoutedEventArgs e)
            {
                Rule rule = this.windowController.getEvaluationRule(Convert.ToInt32((e.Source as TreeViewItem).Tag));
                this.currentEvaluationRule = (EvaluationRule)rule;

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

                if (rule.active)
                {
                    this.evaluationRuleToggleActive.Content = "Deaktivieren";
                }
                else
                {
                    this.evaluationRuleToggleActive.Content = "Aktivieren";
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

        /// <summary>
        /// When clicking reload status button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.checkSystemStatus();
            this.loadPersons();
            this.loadTaxDeclarations();
        }

        /// <summary>
        /// When clicking reload rules for inference
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
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

        /// <summary>
        /// When clicking reload rules for evaluation
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
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

        /// <summary>
        /// When clicking the save button for an inference rule
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        private async void inferenceRuleSave_Click(object sender, RoutedEventArgs e)
        {
            bool isNewRule = false;
            if (this.inferenceRuleId.Text == null || this.inferenceRuleId.Text == "") isNewRule = true;

            InferenceRule updatedRule = new InferenceRule();
            if (!isNewRule) updatedRule.id = Convert.ToInt32(this.inferenceRuleId.Text);
            updatedRule.parentId = Convert.ToInt32(this.inferenceRuleParent.Text);
            updatedRule.rule = this.inferenceRuleName.Text;
            updatedRule.condition = this.inferenceRuleCondition.Text;
            updatedRule.transformation = this.inferenceRuleTransformation.Text;

            await this.windowController.saveNewInferenceRule(updatedRule);
            this.buildTreeViews();
        }
    
        /// <summary>
        /// When clicking the save button for an evaluation rule
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        private async void evaluationRuleSave_Click(object sender, RoutedEventArgs e)
        {
            bool isNewRule = false;
            if (this.evaluationRuleId.Text == null || this.evaluationRuleId.Text == "") isNewRule = true;

            EvaluationRule updatedRule = new EvaluationRule();
            if (!isNewRule) updatedRule.id = Convert.ToInt32(this.evaluationRuleId.Text);
            updatedRule.parentId = Convert.ToInt32(this.evaluationRuleParent.Text);
            updatedRule.rule = this.evaluationRuleName.Text;
            updatedRule.condition = this.evaluationRuleCondition.Text;
            updatedRule.transformation = this.evaluationRuleTransformation.Text;

            await this .windowController.saveNewEvaluationRule(updatedRule);
            this.buildTreeViews();
        }
    
        /// <summary>
        /// When clicking the new button for an inference rule
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        private void inferenceRuleNew_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem root = null;
            foreach (TreeViewItem item in this.inferenceRulesView.Items)
            {
                root = item;
            }

            if (root == null) return;

            this.inferenceRuleId.Text = "";
            this.inferenceRuleParent.Text = this.getParentFromTree(root);
            this.inferenceRuleName.Text = "";
            this.inferenceRuleCondition.Text = "";
            this.inferenceRuleTransformation.Text = "";
        }
    
        /// <summary>
        /// When clicking the new button for an evaluation rule
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event object</param>
        private void evaluationRuleNew_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem root = null;
            foreach (TreeViewItem item in this.evaluationRulesView.Items)
            {
                root = item;
            }

            if (root == null) return;

            this.evaluationRuleId.Text = "";
            this.evaluationRuleParent.Text = this.getParentFromTree(root);
            this.evaluationRuleName.Text = "";
            this.evaluationRuleCondition.Text = "";
            this.evaluationRuleTransformation.Text = "";
        }

        private string getParentFromTree(TreeViewItem root)
        {
            String possibleParent = "";
            List<TreeViewItem> items = this.getNodeChildren(root);
            foreach (TreeViewItem item in items)
            {
                if (item.IsSelected) possibleParent = Convert.ToString(item.Tag);
            }
            return possibleParent;
        }

        private List<TreeViewItem> getNodeChildren(TreeViewItem item)
        {
            List<TreeViewItem> list = new List<TreeViewItem>();
            list.Add(item);

            if (item.Items.Count > 0)
            {
                foreach (TreeViewItem child in item.Items)
                {
                    List<TreeViewItem> nested = getNodeChildren(child);
                    nested.ForEach(x => list.Add(x));
                }
            }

            return list;
        }

        private async void evaluationRuleToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentEvaluationRule != null)
            {
                await this.windowController.toggleActiveRule(this.currentEvaluationRule);
                await this.windowController.reloadRulesFor("evaluator");
                this.buildTreeViews();
            }
        }

        private async void inferenceRuleToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentInferenceRule != null)
            {
                await this.windowController.toggleActiveRule(this.currentInferenceRule);
                await this.windowController.reloadRulesFor("inference");
                this.buildTreeViews();
            }
        }

        private async void sendTaxDeclaration_Click(object sender, RoutedEventArgs e)
        {
            decimal income, deductions, taxDue, capital;
            int year, personId = 0;

            if (!decimal.TryParse(tdIncome.Text, out income))
            {
                Dispatcher.Invoke(() => MessageBox.Show("Einkommen ist keine gültige Zahl"));
                return;
            }

            if (!int.TryParse(tdYear.Text, out year))
            {
                Dispatcher.Invoke(() => MessageBox.Show("Jahr ist keine gültige Zahl"));
                return;
            }

            if (!decimal.TryParse(tdDeductions.Text, out deductions))
            {
                Dispatcher.Invoke(() => MessageBox.Show("Abzüge ist keine gültige Zahl"));
                return;
            }

            if (!decimal.TryParse(tdCapital.Text, out capital))
            {
                Dispatcher.Invoke(() => MessageBox.Show("Vermögen ist keine gültige Zahl"));
                return;
            }

            Person person = (Person)this.personComboBox.SelectedItem;
            if (person != null)
            {
                personId = person.id;
            }

            bool response;
            try
            {
              response = await this.windowController.createNewTaxDeclaration(income, deductions, year, personId, capital);
            } catch
            {
              response = false;
            }

            if (response)
            {
                Dispatcher.Invoke(() => MessageBox.Show("Die neue Steuererklärung wurde werfolgreich eingereicht."));
            } else
            {
                Dispatcher.Invoke(() => MessageBox.Show("Ein Fehler is aufgetreten."));
            }

            this.loadTaxDeclarations();
        }

        private async void reloadTaxDeclarationBtn_Click(object sender, RoutedEventArgs e)
        {
            this.loadTaxDeclarations();
        }
    }
}
