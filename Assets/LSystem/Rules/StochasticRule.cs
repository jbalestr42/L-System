using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oisif
{

public class StochasticRule : ARule
{
    public struct RuleParam
    {
        public float Probability;
        public string Result;

        public RuleParam(float probability, string result)
        {
            Probability = probability;
            Result = result;
        }
    }

    private List<RuleParam> _ruleParams;

    public StochasticRule(char sign, List<RuleParam> ruleParams)
        :base(sign)
    {
        _ruleParams = ruleParams;

        float total = 0;
        foreach (RuleParam param in _ruleParams)
        {
            total += param.Probability;
        }
        Assert.IsTrue(total.Equals(1f), "StochasticRule: Probabilities sum must be 1.");
    }

    public override string Evaluate(LSystem.Token token)
    {
        Assert.IsNotNull(_ruleParams, "StochasticRule.Evaluate: There are no entries.");
        Assert.IsTrue(_ruleParams.Count > 0, "StochasticRule.Evaluate: There are no entries.");
        
        float rand = Random.value;
        float probability = 0f;
        foreach (RuleParam param in _ruleParams)
        {
            probability += param.Probability;
            if (rand <= probability)
            {
                return param.Result;
            }
        }
 
        return _ruleParams[_ruleParams.Count - 1].Result;
    }
}

}