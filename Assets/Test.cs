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
        _systemData = new List<Oisif.SystemData>();
        
        Oisif.SystemData systemData;

        systemData = new Oisif.SystemData("F", 45f, 1.5f);
        systemData.AddRule(new Oisif.Rule('F', "G[+F]"));
        _systemData.Add(systemData);

        systemData = new Oisif.SystemData("F", 45f, 1.5f);
        systemData.AddRule(new Oisif.Rule('F', "G[+F][-F]"));
        _systemData.Add(systemData);

        systemData = new Oisif.SystemData("F", 45f, 3f);
        systemData.AddRule(new Oisif.Rule('F', "F[+F]FF"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.SystemData("YF+XF+YF-XF-YF-XF-YF+XF+YF", 60f, 2f);
        systemData.AddRule(new Oisif.Rule('X', "YF+XF+Y"));
        systemData.AddRule(new Oisif.Rule('Y', "XF-YF-X"));
        _systemData.Add(systemData);

        systemData = new Oisif.SystemData("F+F+F+F", 90f, 3f);
        systemData.AddRule(new Oisif.Rule('F', "FF+F+F+F+FF"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.SystemData("F", 22.5f, 2f);
        systemData.AddRule(new Oisif.Rule('F', "FF+[+F-F-F]-[-F+F+F]"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.SystemData("FX", 40f, 1f);
        systemData.AddRule(new Oisif.Rule('X', ">[-FX]+FX"));
        _systemData.Add(systemData);
        
        systemData = new Oisif.SystemData("F-F-F-F", 90f, 4f);
        systemData.AddRule(new Oisif.Rule('F', "F-f+FF-F-FF-Ff-FF+f-FF+F+FF+Ff+FFF"));
        systemData.AddRule(new Oisif.Rule('f', "ffffff"));
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
            _system.NextGeneration();
            _system.DisplayCurrentState();
            _interpretor.Reset();
            _interpretor.Execute(_system);

            Bounds bounds = _interpretor.CameraBounds;
            
            float xCenter = bounds.min.x + (Mathf.Abs(bounds.max.x - bounds.min.x) / 2f);
            float yCenter = bounds.min.y + (Mathf.Abs(bounds.max.y - bounds.min.y) / 2f);

            float height = 90f / Camera.main.fieldOfView * (bounds.max.y - bounds.center.y) * 1.2f;
 
            Camera.main.transform.position = new Vector3(bounds.center.x, bounds.center.y, -height);
        }
    }
}