using System.Collections.Generic;
using System.Threading.Tasks;
using GUI.Models;
using Shared.Models;

namespace GUI.Controllers {
  class MainWindowController {
    DatabaseModel databaseModel;
    InferenzmotorModel inferenceModel;
    SteuerberechnerModel evaluatorModel;

    private List<Person> personList;
    private List<TaxDeclaration> declarationList;
    private List<Rule> inferenceRules;
    private List<Rule> evaluationRules;

    public MainWindowController()
    {
      this.databaseModel = new DatabaseModel();
      this.inferenceModel = new InferenzmotorModel();
      this.evaluatorModel = new SteuerberechnerModel();
    }

    public async Task<Dictionary<string, bool>> getServiceStatus()
    {
      return new Dictionary<string, bool>()
      {
          ["database"] = await this.databaseModel.serviceIsRunning(),
          ["inference"] = await this.inferenceModel.serviceIsRunning(),
          ["evaluator"] = await this.evaluatorModel.serviceIsRunning()
      };
    }

    public async Task reloadRulesFor(string service)
    {
      if (service == "inference") await this.inferenceModel.reloadRules();
      if (service == "evaluator") await this.evaluatorModel.reloadRules();
    }

    public async Task<List<Rule>> getAllInferrenceRules()
    {
      var list = await this.databaseModel.getAllInferenceRules();
      this.inferenceRules = list.ConvertAll(x => (Rule)x);
      return this.inferenceRules;
    }

    public Rule getInferenceRule(int id)
    {
      return this.inferenceRules.Find(x => x.id == id);
    }

    public async Task<List<Rule>> getAllEvaluationRules()
    {
      var list = await this.databaseModel.getAllEvaluationRules();
      this.evaluationRules = list.ConvertAll(x => (Rule)x);
      return this.evaluationRules;
    }

    public Rule getEvaluationRule(int id)
    {
      return this.evaluationRules.Find(x => x.id == id);
    }

    public async Task<List<Person>> getAllPersons()
    {
        this.personList = await this.databaseModel.getAllPersons();
        return this.personList;
    }

    public async Task<List<TaxDeclaration>> getAllTaxDeclarations()
    {
        this.declarationList = await this.databaseModel.getAllTaxDeclarations();
        return this.declarationList;
    }
    
    public async Task<List<Rule>> saveAddedInferenceRule(InferenceRule rule)
    {
        // TODO: Attempt to persist to database and catch eventual errors
        
        Rule parentRule = this.inferenceRules.Find(x => x.id == rule.parentId);
        rule.parent = parentRule;
        this.inferenceRules.Add(rule);

        return this.inferenceRules;
    }

    public async Task<List<Rule>> saveUpdatedInferenceRule(InferenceRule rule)
    {
        // TODO: Attempt to persist to database and catch eventual errors
        
        Rule oldRule = this.inferenceRules.Find(x => x.id == rule.id);
        this.inferenceRules.Remove(oldRule);

        Rule parentRule = this.inferenceRules.Find(x => x.id == rule.parentId);
        rule.parent = parentRule;
        this.inferenceRules.Add(rule);

        return this.inferenceRules;
    }


    public async Task<List<Rule>> saveAddedEvaluationRule(EvaluationRule rule)
    {
        // TODO: Attempt to persist to database and catch eventual errors
        
        Rule parentRule = this.evaluationRules.Find(x => x.id == rule.parentId);
        rule.parent = parentRule;
        this.evaluationRules.Add(rule);

        return this.evaluationRules;
    }

    public async Task<List<Rule>> saveUpdatedEvaluationRule(EvaluationRule rule)
    {
        // TODO: Attempt to persist to database and catch eventual errors

        Rule oldRule = this.evaluationRules.Find(x => x.id == rule.id);
        this.evaluationRules.Remove(oldRule);

        Rule parentRule = this.evaluationRules.Find(x => x.id == rule.parentId);
        rule.parent = parentRule;
        this.evaluationRules.Add(rule);

        return this.evaluationRules;
    }

    public void teardown()
    {
        this.databaseModel.teardown();
        this.inferenceModel.teardown();
        this.evaluatorModel.teardown();
    }
  }
}
