using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Oisif
{

public class Interpretor<T> where T : LSystem
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

    public delegate void SystemAction(LSystem.Token token, T system);
    Dictionary<char, ActionData> _actions;

    public Interpretor()
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
    
    public virtual void Execute(T system)
    {
        Assert.IsNotNull(system, "Interpretor.Execute: The L-system is null");

        LSystem.SystemSignAction signAction = (LSystem.Token token) =>
        {
            SystemAction action = GetAction(token.Sign);
            if (action != null)
            {
                action(token, system);
            }
        };

        system.ForEach(signAction);
    }
}

}