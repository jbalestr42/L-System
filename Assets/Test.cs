using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    List<Oisif.SystemData> _systemData;
    Oisif.LSystem _system;
    Oisif.TokenInterpretor _interpretor;
    int _currentIndex = 0;

    void Start()
    {
        // TODO remove the depth factor and use the line scale factor instead
        _systemData = new List<Oisif.SystemData>();
        
        Oisif.SystemData systemData;
        
        systemData = new Oisif.SystemData("Y", 45f, 1.5f);
        List<Oisif.StochasticRule.RuleParam> ruleParams = new List<Oisif.StochasticRule.RuleParam>();
        ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.5f, "G[+F][-F]"));
        ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.5f, "G[+F]"));
        systemData.AddRule(new Oisif.StochasticRule('F', ruleParams));
        systemData.AddRule(new Oisif.SimpleRule('Y', "G[+F][-F]"));
        _systemData.Add(systemData);
        
        // TODO Remove the last param, line length must be handled by the sign ">"
        systemData = new Oisif.SystemData("FX", 40f, 1.5f);
        systemData.AddRule(new Oisif.SimpleRule('X', "F[-FX][+FX]"));
        _systemData.Add(systemData);

        systemData = new Oisif.SystemData("FX", 40f, 1.5f);
        systemData.AddRule(new Oisif.SimpleRule('X', ">[-FX]+FX"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.SystemData("F", 45f, 1.5f);
        systemData.AddRule(new Oisif.SimpleRule('F', "G[+F][-F]"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.SystemData("F", 22.5f, 2f);
        systemData.AddRule(new Oisif.SimpleRule('F', "FF+[+F-F-F]-[-F+F+F]"));
        _systemData.Add(systemData);

        _system = new Oisif.LSystem();
        _system.Data = _systemData[_currentIndex];
        
        _interpretor = new Oisif.TokenInterpretor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _currentIndex = _currentIndex > 0 ? _currentIndex - 1 : _systemData.Count - 1;
            _system.Data = _systemData[_currentIndex];
            _interpretor.Reset();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentIndex = _currentIndex < _systemData.Count - 1 ? _currentIndex + 1 : 0;
            _system.Data = _systemData[_currentIndex];
            _interpretor.Reset();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            _system.NextGeneration();
            _system.DisplayCurrentState();
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            _interpretor.Reset();
            _interpretor.Execute(_system);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_system.Depth() >= 0)
            {
                // Draw
                _interpretor.Reset();
                _interpretor.Execute(_system);
            }

            // Compute next generation
            _system.NextGeneration();
        }
    }
}