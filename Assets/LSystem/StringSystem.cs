using UnityEngine;

namespace Oisif
{

public class StringSystem : ALSystem
{
    string _currentState;
    int _depth;

    public StringSystem()
    {
    }

    public override void ForEach(SystemSignAction action)
    {
        if (action != null)
        {
            for (int i = 0; i < _currentState.Length; i++)
            {
                action(_currentState[i]);
            }
        }
    }

    public override void NextGeneration()
    {
        string prevSystem = _currentState;
        _currentState = "";
        for (int i = 0; i < prevSystem.Length; i++)
        {
            Rule rule = Data.Rules.Find(r => r.Sign == prevSystem[i]);

            if (rule != null)
            {
                _currentState += rule.Result;
            }
            else
            {
                _currentState += prevSystem[i];
            }
        }
        _depth++;
    }

    public override void DisplayCurrentState()
    {
        Debug.Log("Current state: " + _currentState);
    }

    public override SystemData Data {
        set {
            base.Data = value;
            _currentState = Data.Axiom;
            _depth = 0;
        } 
    }

    public override int Depth()
    {
        return _depth;
    }
}

}