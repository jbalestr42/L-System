using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Oisif
{

public class LSystemInterpretor
{
    public struct ActionData
    {
        public bool IsDrawable;
        public SystemAction Action;

        public ActionData(bool isDrawable, SystemAction action)
        {
            IsDrawable = isDrawable;
            Action = action;
        }
    }

    public delegate void SystemAction(LSystem.Token token, LSystem system);
    Dictionary<char, ActionData> _actions;

    public LSystemInterpretor()
    {
        _actions = new Dictionary<char, ActionData>();
    }

    public void AddAction(char sign, bool isDrawabale, SystemAction action)
    {
        _actions[sign] = new ActionData(isDrawabale, action);
    }
    
    public ActionData GetActionData(char sign)
    {
        //TODO assert exist in _actions
        ActionData data;
        _actions.TryGetValue(sign, out data);
        return data;
    }
    
    public SystemAction GetAction(char sign)
    {
        ActionData data;
        _actions.TryGetValue(sign, out data);
        return data.Action;
    }
    
    public bool ContainsAction(char sign)
    {
        return _actions.ContainsKey(sign);
    }
    
    public virtual void Execute(LSystem system)
    {
        Assert.IsNotNull(system, "Interpretor.Execute: The L-system is null");

        LSystem.SystemSignAction signAction = (LSystem.Token token) =>
        {
            GetAction(token.Sign)?.Invoke(token, system);
        };

        system.ForEach(signAction);
    }

    public virtual bool IsDoneExecuting()
    {
        return true;
    }

}

}