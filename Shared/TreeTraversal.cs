using System;
using System.Collections.Generic;
using Shared.Models;
using CodingSeb.ExpressionEvaluator;

/// <summary>
/// Generic libary for building and traversing decision trees.
/// </summary>
namespace Shared.TreeTraversal {

    /// <summary>
    /// Parser for rules in rule tree.
    /// 
    /// Allows evaluating a simple rule script as either condition or transformation
    /// using the given input data.
    /// </summary>
    public class RuleParser {
      /// <summary>
      /// Evaluate a given script string as condition.
      /// </summary>
      /// <param name="script">The script string to execute</param>
      /// <param name="input">The RuleData containing input variables</param>
      /// <returns>Boolean indicator of wheter or not condition is true.</returns>
      public static bool evaluateAsCondition(string script, RuleData input)
      {
        ExpressionEvaluator evaluator = new ExpressionEvaluator();
        evaluator.Variables = input.data;
        return Convert.ToBoolean(evaluator.Evaluate(script));
      }

      /// <summary>
      /// Evaluate a given script as data transformation
      /// </summary>
      /// <param name="script">The script string to execute</param>
      /// <param name="input">The RuleData containing input variables</param>
      /// <returns>RuleData containing modified input variables</returns>
      public static RuleData evaluateAsTransformation(string script, RuleData input)
      {
        ExpressionEvaluator evaluator = new ExpressionEvaluator();
        evaluator.Variables = input.data;
        evaluator.Evaluate(script);
        return new RuleData(new Dictionary<string, object>(evaluator.Variables));
      }
    }

    /// <summary>
    /// Wrapped data for passing information through the decision tree
    /// 
    /// Allows storing the following values:
    ///   Vorjahr:  vj_einkommen, vj_vermoegen, vj_steuersatz
    ///   Laufjahr: lj_einkommen, lj_vermoegen, lj_steuersatz
    /// </summary>
    public class RuleData {
      public Dictionary<String, object> data;

      public RuleData(Dictionary<String, object> data)
      {
        this.data = data;
      }
    }

    /// <summary>
    /// Generic rule traversal implementation
    /// 
    /// This takes a list of rules, looks for the one with no parent given
    /// and uses that as the root to build a tree of rules to work through.
    /// 
    /// Every rule will have its condition evaluated and its transformation
    /// applied, if any is specified.
    /// </summary>
    public class RuleTraversal {
      /// <summary>
      /// Automatically build and then traverse a decision tree based on a list of rules.
      /// </summary>
      /// <param name="rules">The list of rules in the tree</param>
      /// <param name="data">The input dataset to pass through</param>
      /// <returns>The output dataset of the decision tree</returns>
      public static RuleData traverseRuleTree(List<Rule> rules, RuleData data)
      {
        // First, we find the root node by finding the rule without a parent
        Rule rootNode = null;
        Rule currentNode = null;
        Rule nextNode = null;
        List<Rule> children = new List<Rule>();

        foreach (var rule in rules)
        {
          if (rule.parent == null || rule.parent.id == 0) rootNode = rule;
        }

        // If no root node is found, we just return the data as-is
        if (rootNode == null)
        {
          Console.WriteLine("  -> No root rule found, returning input as-is");
          return data;
        }

        // If a root node is found, we iterate through the node tree
        // until we get to a node which is a leaf (e.g. has no children)
        currentNode = rootNode;
        do
        {
          Console.WriteLine("  -> Traversing through node " + currentNode.rule);

          // Get all children for the current rule node
          children = rules.FindAll((rule) => rule.parent != null && rule.parent.id == currentNode.id);
          nextNode = null;

          foreach (var child in children)
          {
            // Check if condition is true. If not, continue.
            if (child.condition == null) child.condition = "true";
            Console.WriteLine("  --> Checking condition: " + child.condition);
            if (RuleParser.evaluateAsCondition(child.condition, data)) {
              if (child.transformation != null && child.transformation != "" && child.active)
              {
                Console.WriteLine("  --> Performing Transformation: " + child.transformation);
                data = RuleParser.evaluateAsTransformation(child.transformation, data);
              } 

              nextNode = child;
              break;
            }
          }

          // If no condition matched, we also stop. 
          // TODO: Talk with customer what to do in this case!
          if (children.Count > 0 && nextNode == null)
          {
            Console.WriteLine("  -> Had " + children.Count + " children but none matched. Returnung data.");
            return data;
          }

          currentNode = nextNode;
        } while (children.Count > 0);

        Console.WriteLine("  -> Reached leaf node on tree. Returning final data.");
        return data;
      }
    }
}
