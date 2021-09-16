using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Shared.Contracts;
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

    public void teardown()
    {
      this.databaseModel.teardown();
      this.inferenceModel.teardown();
      this.evaluatorModel.teardown();
    }
  }
}
