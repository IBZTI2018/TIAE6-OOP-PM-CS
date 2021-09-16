using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Shared.Contracts;
using GUI.Models;

namespace GUI.Controllers {
  class MainWindowController {
    DatabaseModel databaseModel;
    InferenzmotorModel inferenceModel;
    SteuerberechnerModel evaluatorModel;

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

    public void teardown()
    {
      this.databaseModel.teardown();
      this.inferenceModel.teardown();
      this.evaluatorModel.teardown();
    }
  }
}
