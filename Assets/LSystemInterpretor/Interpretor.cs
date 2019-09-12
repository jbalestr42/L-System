using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Oisif
{

public class Interpretor<T> where T : ALSystem
{
    public delegate void SystemAction(T system);
    Dictionary<char, SystemAction> _actions;

    public Interpretor()
    {
        _actions = new Dictionary<char, SystemAction>();
    }

    public void AddAction(char sign, SystemAction action)
    {
        _actions[sign] = action;
    }
    
    public SystemAction GetAction(char sign)
    {
        SystemAction action = null;
        _actions.TryGetValue(sign, out action);
        return action;
    }
    
    public virtual void Execute(T system)
    {
        Assert.IsNotNull(system, "Interpretor.Execute: The L-system is null");

        ALSystem.SystemSignAction signAction = (char sign) =>
        {
            SystemAction action = GetAction(sign);
            if (action != null)
            {
                action(system);
            }
        };

        system.ForEach(signAction);
    }
}

}