using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    List<Oisif.LSystemData> _systemData;
    Oisif.LSystem _system;
    Oisif.DrawerInterpretor _interpretor;
    int _currentIndex = 0;

    void Start()
    {
        _systemData = new List<Oisif.LSystemData>();
        
        Oisif.LSystemData systemData;
        
        // TODO Remove the last param, line length must be handled by the sign ">"
        systemData = new Oisif.LSystemData("Y", 40f, 1.3f);
        List<Oisif.StochasticRule.RuleParam> ruleParams = new List<Oisif.StochasticRule.RuleParam>();
        ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.5f, ">G[+F][-F]"));
        ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.2f, ">G[+F]"));
        ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.3f, "GC"));
        systemData.AddRule(new Oisif.StochasticRule('F', ruleParams));
        systemData.AddRule(new Oisif.SimpleRule('Y', ">G[+F][-F]"));
        _systemData.Add(systemData);
        
        // TODO this one is still broken because the rule start with a single F
        //systemData = new Oisif.LSystemData("FX", 40f, 1.5f);
        //systemData.AddRule(new Oisif.SimpleRule('X', ">F[-FX][+FX]"));
        //_systemData.Add(systemData);

        systemData = new Oisif.LSystemData("FX", 40f, 1.5f);
        systemData.AddRule(new Oisif.SimpleRule('X', ">[-FX]+FX"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.LSystemData("F", 45f, 1.5f);
        systemData.AddRule(new Oisif.SimpleRule('F', ">G[+F][-F]"));
        _systemData.Add(systemData);
        
        // Both need to be fixed
        systemData = new Oisif.LSystemData("F", 22.5f, 2f);
        systemData.AddRule(new Oisif.SimpleRule('F', "FF+[+F-F-F]-[-F+F+F]"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.LSystemData("F", 22.5f, 2f);
        systemData.AddRule(new Oisif.SimpleRule('F', "GG+[+F-F-F]-[-F+F+F]"));
        _systemData.Add(systemData);

        _system = new Oisif.LSystem();
        _system.Data = _systemData[_currentIndex];
        
        _interpretor = new Oisif.DrawerInterpretor();
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
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            _system.Reset();
            _interpretor.Reset();
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

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 40;

        GUI.Label(new Rect(10, 10, 200, 60), "Arrows to change rules", style);
        GUI.Label(new Rect(10, 60, 200, 60), "Space to iterate", style);
        GUI.Label(new Rect(10, 110, 200, 60), "R to reset", style);
    }
}