using System.Collections.Generic;

namespace Oisif
{

public class Interpretor
{
    public delegate void SystemAction(ALSystem system);
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
    
    public virtual void Execute(ALSystem system)
    {
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