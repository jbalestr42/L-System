using System.Collections.Generic;

namespace Oisif
{

public class Rule
{
    char _sign;
    string _result;

    public Rule(char sign, string result)
    {
        _sign = sign;
        _result = result;
    }

    public char Sign { get { return _sign; } }
    public string Result { get { return _result; } }

    public virtual bool Match(string currentState, int index)
    {
        return (currentState[index] == _sign);
    }
}

// TODO Scriptable Object?
public class SystemData
{
    public string Axiom { get; }
    public float Angle { get; }
    public float DepthFactor { get; }
    public List<Rule> Rules { get; }

    public SystemData(string axiom, float angle, float depthFactor)
    {
        Rules = new List<Rule>();
        Axiom = axiom;
        Angle = angle;
        DepthFactor = depthFactor;
    }

    public void AddRule(Rule rule)
    {
        Rules.Add(rule);
    }
}

public abstract class ALSystem
{
    public delegate void SystemSignAction(char sign);
    SystemData _systemData;

    public ALSystem()
    {
        _systemData = null;
    }

    public abstract void ForEach(SystemSignAction action);
    public abstract void NextGeneration();
    public abstract void DisplayCurrentState();
    public abstract int Depth();
    public virtual SystemData Data {
        get { return _systemData; }
        set { _systemData = value; } 
    }
}

}