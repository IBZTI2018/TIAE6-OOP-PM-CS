using System.Collections.Generic;
using System.Threading.Tasks;
using GUI.Models;
using Shared.Models;

namespace GUI.Controllers {
  /// <summary>
  /// Controller of the main application window
  /// </summary>
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
    
    /// <summary>
    /// Get the status of all linked services
    /// </summary>
    /// <returns>A dictionary with service status information</returns>
    public async Task<Dictionary<string, bool>> getServiceStatus()
    {
      return new Dictionary<string, bool>()
      {
          ["database"] = await this.databaseModel.serviceIsRunning(),
          ["inference"] = await this.inferenceModel.serviceIsRunning(),
          ["evaluator"] = await this.evaluatorModel.serviceIsRunning()
      };
    }

    /// <summary>
    /// Command a worker node to reload its rules
    /// </summary>
    /// <param name="service">The name of the service to reolad</param>
    /// <returns>An awaitable task</returns>
    public async Task reloadRulesFor(string service)
    {
      if (service == "inference") await this.inferenceModel.reloadRules();
      if (service == "evaluator") await this.evaluatorModel.reloadRules();
    }

    /// <summary>
    /// Get a list of all inferrence rules
    /// </summary>
    /// <returns>An awaitable task with a list of inference rules</returns>
    public async Task<List<Rule>> getAllInferrenceRules()
    {
      var list = await this.databaseModel.getAllInferenceRules();
      this.inferenceRules = list.ConvertAll(x => (Rule)x);
      return this.inferenceRules;
    }

    /// <summary>
    /// Get a single inference rule (from the local cache)
    /// </summary>
    /// <param name="id">The ID of the rule to get</param>
    /// <returns>The inference rule, if it was found in the local cache</returns>
    public Rule getInferenceRule(int id)
    {
      return this.inferenceRules.Find(x => x.id == id);
    }

    /// <summary>
    /// Get a list of all evaluation rules
    /// </summary>
    /// <returns>An awaitable task with a list of evaluation rules</returns>
    public async Task<List<Rule>> getAllEvaluationRules()
    {
      var list = await this.databaseModel.getAllEvaluationRules();
      this.evaluationRules = list.ConvertAll(x => (Rule)x);
      return this.evaluationRules;
    }

    /// <summary>
    /// Get a single evaluation rule (from the local cache)
    /// </summary>
    /// <param name="id">The ID of the rule to get</param>
    /// <returns>The evaluation rule, if it was found in the local cache</returns>
    public Rule getEvaluationRule(int id)
    {
      return this.evaluationRules.Find(x => x.id == id);
    }

    /// <summary>
    /// Get a list of all persons
    /// </summary>
    /// <returns>An awaitable task with a list of persons</returns>
    public async Task<List<Person>> getAllPersons()
    {
        this.personList = await this.databaseModel.getAllPersons();
        return this.personList;
    }

    /// <summary>
    /// Get all tax declarations
    /// </summary>
    /// <returns>An awaitable task with a list of tax declarations</returns>
    public async Task<List<TaxDeclaration>> getAllTaxDeclarations()
    {
        this.declarationList = await this.databaseModel.getAllTaxDeclarations();
        return this.declarationList;
    }

    /// <summary>
    /// Persist a new inference rule in the database
    /// </summary>
    /// <param name="rule">The rule to persist</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> saveNewInferenceRule(InferenceRule rule)
    {
        return await this.databaseModel.saveNewInferenceRule(rule);
    }

    /// <summary>
    /// Persist a new evaluation rule in the database
    /// </summary>
    /// <param name="rule">The rule to persist</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> saveNewEvaluationRule(EvaluationRule rule)
    {
        return await this.databaseModel.saveNewEvaluationRule(rule);
    }

    /// <summary>
    /// Toggle the active flag of a rule
    /// </summary>
    /// <param name="rule">The rule to toggle</param>
    /// <returns>A boolean success indicator of persisting the data</returns>
    public async Task<bool> toggleActiveRule(Rule rule)
    {
        return await this.databaseModel.toggleActiveRule(rule);
    }

    /// <summary>
    /// Create a new tax delcaration
    /// 
    /// This is a dummy function. Eventually, this would be done via API.
    /// </summary>
    /// <param name="income">The income of the person</param>
    /// <param name="deductions">The given deductions of a person</param>
    /// <param name="year">The year for which de declaration is filed</param>
    /// <param name="personId">The id of the person to file for</param>
    /// <param name="capital">The capital of the person</param>
    /// <returnsA boolean success indicator of persisting the data></returns>
    public async Task<bool> createNewTaxDeclaration(decimal income, decimal deductions, int year, int personId, decimal capital)
    {
        return await this.databaseModel.createNewTaxDeclaration(income, deductions, year, personId, capital);
    }

    /// <summary>
    /// Tear down the controller
    /// </summary>
    public void teardown()
    {
        this.databaseModel.teardown();
        this.inferenceModel.teardown();
        this.evaluatorModel.teardown();
    }
  }
}
