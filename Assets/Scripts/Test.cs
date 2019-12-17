using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    struct TestData
    {
        public Oisif.LSystemData Data { get; private set; }
        public string Text { get; private set; }

        public TestData(Oisif.LSystemData data, string text)
        {
            Data = data;
            Text = text;
        }
    }

    List<TestData> _systemDatas;
    Oisif.LSystem _system;
    Oisif.DrawerInterpretor _interpretor;
    int _currentIndex = 0;
    bool _showUI = true;

    void Start()
    {
        _systemDatas = new List<TestData>();
        
        {
            Oisif.LSystemData systemData = new Oisif.LSystemData("FX", new Oisif.SimpleValue(40.0f), 1.1f);
            systemData.AddRule(new Oisif.SimpleRule('X', ">[-FX]+FX"));
            _systemDatas.Add(new TestData(systemData, "A simple tree"));
        }

        {
            Oisif.LSystemData systemData = new Oisif.LSystemData("F", new Oisif.SimpleValue(22.5f), 2.3f);
            systemData.AddRule(new Oisif.SimpleRule('F', "GG+[+F-F-F]-[-F+F+F]"));
            _systemDatas.Add(new TestData(systemData, "A more complexe tree"));
        }

        {
            Oisif.LSystemData systemData = new Oisif.LSystemData("Y", new Oisif.SimpleValue(40.0f), 1.1f);
            List<Oisif.StochasticRule.RuleParam> ruleParams = new List<Oisif.StochasticRule.RuleParam>();
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.5f, ">G[+F][-F]"));
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.15f, ">G[+F]"));
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.15f, ">G[-F]"));
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.2f, "GC"));
            systemData.AddRule(new Oisif.StochasticRule('F', ruleParams));
            systemData.AddRule(new Oisif.SimpleRule('Y', ">G[+F][-F]"));
            _systemDatas.Add(new TestData(systemData, "A simple random tree"));
        }
        
        {
            Oisif.LSystemData systemData = new Oisif.LSystemData("Y", new Oisif.RangeValue(35.0f, 45.0f), 1.1f);
            List<Oisif.StochasticRule.RuleParam> ruleParams = new List<Oisif.StochasticRule.RuleParam>();
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.6f, ">G[+F][F][-F]"));
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.05f, ">G[+F]"));
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.05f, ">G[-F]"));
            ruleParams.Add(new Oisif.StochasticRule.RuleParam(0.3f, "GC"));
            systemData.AddRule(new Oisif.StochasticRule('F', ruleParams));
            systemData.AddRule(new Oisif.SimpleRule('Y', ">G[+F][Y][-F]"));
            _systemDatas.Add(new TestData(systemData, "A more complexe random tree"));
        }

        _system = new Oisif.LSystem();
        _system.Data = _systemDatas[_currentIndex].Data;
        _interpretor = new Oisif.DrawerInterpretor();
    }

    void Update()
    {
        if (_interpretor != null && !_interpretor.IsDoneExecuting())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _currentIndex = _currentIndex > 0 ? _currentIndex - 1 : _systemDatas.Count - 1;
            _system.Data = _systemDatas[_currentIndex].Data;
            _interpretor.Reset();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentIndex = _currentIndex < _systemDatas.Count - 1 ? _currentIndex + 1 : 0;
            _system.Data = _systemDatas[_currentIndex].Data;
            _interpretor.Reset();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _system.Reset();
            _interpretor.Reset();
        }

        if (Input.GetKey(KeyCode.Space))
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

        if (Input.GetKeyDown(KeyCode.H))
        {
            _showUI = !_showUI;
        }
    }

    void OnGUI()
    {
        if (_showUI)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 40;

            GUI.Label(new Rect(10, Screen.height - 260, 200, 60), "Space to grow", style);
            GUI.Label(new Rect(10, Screen.height - 210, 200, 60), "R to reset", style);
            GUI.Label(new Rect(10, Screen.height - 160, 200, 60), "H to hide the text", style);
            GUI.Label(new Rect(10, Screen.height - 110, 200, 60), "Arrows left/right to change tree", style);
            GUI.Label(new Rect(10, Screen.height - 60, 200, 60), "Example " + (_currentIndex + 1) + "/" + _systemDatas.Count + " -> " + _systemDatas[_currentIndex].Text, style);
        }
    }
}