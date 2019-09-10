using System.Collections.Generic;
using UnityEngine;

namespace Oisif
{

public class StringSystem : ALSystem
{
    List<string> _state;

    public StringSystem()
    {
        _state = new List<string>();
    }

    public override void ForEach(SystemSignAction action)
    {
        if (action != null)
        {
            string currentState = _state[_state.Count - 1];
            for (int i = 0; i < currentState.Length; i++)
            {
                action(currentState[i]);
            }
        }
    }

    public override void NextGeneration()
    {
        string prevGeneration = _state[_state.Count - 1];
        string newGeneration = "";

        for (int i = 0; i < prevGeneration.Length; i++)
        {
            Rule rule = Data.Rules.Find(r => r.Sign == prevGeneration[i]);

            if (rule != null)
            {
                newGeneration += rule.Result;
            }
            else
            {
                newGeneration += prevGeneration[i];
            }
        }
        _state.Add(newGeneration);
    }

    public override void DisplayCurrentState()
    {
        string currentState = _state[_state.Count - 1];
        Debug.Log("Current state: " + currentState);
    }

    public override SystemData Data {
        set {
            base.Data = value;
            _state.Add(Data.Axiom);
        } 
    }

    public override int Depth()
    {
        return _state.Count;
    }
}

}