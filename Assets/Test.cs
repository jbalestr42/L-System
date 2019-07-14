using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class ContextRule : Rule
{
    char _prev;
    char _prevCondition;
    char _next;
    char _nextCondition;
    char _condition;

    public ContextRule(char prev, char prevCondition, char sign, char nextCondition, char next, string result)
        :base(sign, result)
    {
        _prev = prev;
        _prevCondition = prevCondition;
        _next = next;
        _nextCondition = nextCondition;
    }
 
    public char Prev { get { return _prev; } }
    public char PrevCondition { get { return _prevCondition; } }
    public char Next { get { return _next; } }
    public char NextCondition { get { return _nextCondition; } }
}

// TODO Scriptable Object
public class SystemData
{
    List<Rule> _rules;
    string _axiom;
    float _angle;
    float _depthFactor;

    public SystemData(string axiom, float angle, float depthFactor)
    {
        _rules = new List<Rule>();
        _axiom = axiom;
        _angle = angle;
        _depthFactor = depthFactor;
    }

    public void AddRule(Rule rule)
    {
        _rules.Add(rule);
    }
    
    public string Axiom { get { return _axiom; } }
    public float Angle { get { return _angle; } }
    public float DepthFactor { get { return _depthFactor; } }
    public List<Rule> Rules { get { return _rules; } }
}

public class Interpretor
{
    public delegate void SystemAction(ASystem system);
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
    
    public virtual void Execute(ASystem system)
    {
        for (int i = 0; i < system.CurrentState.Length; i++)
        {
            SystemAction action = GetAction(system.CurrentState[i]);
            if (action != null)
            {
                action(system);
            }
        }
    }
}

public class LineInterpretor : Interpretor
{
    LineManager _lineManager;
    float _currentAngle;
    Vector3 _origin;
    Vector3 _currentPosition;
    float _lineLength;
    float _lineLengthScaleFactor;
    Bounds _bounds;

    public struct DrawState
    {
        public Vector3 _position;
        public float _angle;
        public float _lineLength;

        public DrawState(Vector3 position, float angle, float lineLength)
        {
            _position = position;
            _angle = angle;
            _lineLength = lineLength;
        }

        public Vector3 Position { get { return _position; } }
        public float Angle { get { return _angle; } }
        public float LineLength { get { return _lineLength; } }
    }
    Stack<DrawState> _savedPositions;

    public LineInterpretor()
        :base()
    {
        _lineManager = Object.FindObjectOfType<LineManager>();
        _savedPositions = new Stack<DrawState>();

        AddAction('+', IncreaseAngle);
        AddAction('-', DecreaseAngle);
        AddAction('F', DrawLine);
        AddAction('f', Move);
        AddAction('[', SaveDrawState);
        AddAction(']', RestoreDrawState);
        AddAction('>', MultiplyLineLength);
        AddAction('<', DivideLineLength);
        Reset();
    }
    
    void IncreaseAngle(ASystem system)
    {
        _currentAngle += system.Data.Angle;
    }

    void DecreaseAngle(ASystem system)
    {
        _currentAngle -= system.Data.Angle;
    }
    
    void SaveDrawState(ASystem system)
    {
        _savedPositions.Push(new DrawState(_currentPosition, _currentAngle, _lineLength));
    }
    
    void RestoreDrawState(ASystem system)
    {
        DrawState drawState = _savedPositions.Pop();
        _currentPosition = drawState.Position;
        _currentAngle = drawState.Angle;
        _lineLength = drawState.LineLength;
    }
    
    void MultiplyLineLength(ASystem system)
    {
        _lineLength *= _lineLengthScaleFactor;
    }

    void DivideLineLength(ASystem system)
    {
        _lineLength /= _lineLengthScaleFactor;
    }

    void Move(ASystem system)
    {
        float lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth - 1));
        _currentPosition = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
    }

    void DrawLine(ASystem system)
    {
        float lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth - 1));
        Vector3 start = _currentPosition;
        Vector3 end =  _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
        _currentPosition = end;

        Vector3 origin = _savedPositions.Count != 0 ? _savedPositions.Peek().Position : _origin;
        _lineManager.CreateInterpolatedLine(_savedPositions.Count, origin, start, end);

        _bounds.Encapsulate(start);
        _bounds.Encapsulate(end);
    }

    public void Reset()
    {
        _origin = Vector3.zero;
        _currentPosition = _origin;
        _currentAngle = 0f;
        _lineLength = 5f;
        _lineLengthScaleFactor = 0.7f;
        _lineManager.Clear();
        _savedPositions.Clear();
        _bounds = new Bounds();
    }

    public override void Execute(ASystem system)
    {
        base.Execute(system);
        //_lineManager.ExpandLine();
        _lineManager.ExpandLineId();
    }

    public Bounds CameraBounds { get { return _bounds; } }
}

public class ASystem
{
    SystemData _systemData;
    string _currentState;
    int _depth;

    public ASystem()
    {
        _systemData = null;
    }

    public virtual void Solve()
    {
        string prevSystem = _currentState;
        _currentState = "";
        for (int i = 0; i < prevSystem.Length; i++)
        {
            Rule rule = _systemData.Rules.Find(r => r.Sign == prevSystem[i]);

            if (rule != null)
            {
                _currentState += rule.Result;
            }
            else
            {
                _currentState += prevSystem[i];
            }
        }
        _depth++;
    }

    public void DisplayCurrentState()
    {
        Debug.Log("Current state: " + _currentState);
    }

    public SystemData Data {
        get { return _systemData; }
        set {
        _systemData = value;
        _currentState = _systemData.Axiom;
        _depth = 0;
        } 
    }
    public string CurrentState { get { return _currentState; } }
    public float Depth { get { return _depth; } }
}

public class Test : MonoBehaviour
{
    List<SystemData> _systemData;
    ASystem _system;
    LineInterpretor _interpretor;
    int _currentIndex = 0;

    void Start()
    {
        _systemData = new List<SystemData>();
        
        SystemData systemData = new SystemData("YF+XF+YF-XF-YF-XF-YF+XF+YF", 60f, 2f);
        systemData.AddRule(new Rule('X', "YF+XF+Y"));
        systemData.AddRule(new Rule('Y', "XF-YF-X"));
        _systemData.Add(systemData);

        systemData = new SystemData("F+F+F+F", 90f, 3f);
        systemData.AddRule(new Rule('F', "FF+F+F+F+FF"));
        _systemData.Add(systemData);
        
        systemData = new SystemData("F", 22.5f, 2f);
        systemData.AddRule(new Rule('F', "FF+[+F-F-F]-[-F+F+F]"));
        _systemData.Add(systemData);
        
        systemData = new SystemData("FX", 40f, 1f);
        systemData.AddRule(new Rule('X', ">[-FX]+FX"));
        _systemData.Add(systemData);
        
        systemData = new SystemData("F-F-F-F", 90f, 4f);
        systemData.AddRule(new Rule('F', "F-f+FF-F-FF-Ff-FF+f-FF+F+FF+Ff+FFF"));
        systemData.AddRule(new Rule('f', "ffffff"));
        _systemData.Add(systemData);

        systemData = new SystemData("F", 45f, 3f);
        systemData.AddRule(new Rule('F', "F[+F]FF"));
        _systemData.Add(systemData);

        _system = new ASystem();
        _system.Data = _systemData[_currentIndex];
        
        _interpretor = new LineInterpretor();
        
        systemData = new SystemData("FF+[+F-F-F]-[-F+F+F]", 22.5f, 2f);
        systemData.AddRule(new Rule('F', "FF+[+F-F-F]-[-F+F+F]"));
        TreeSystem s = new TreeSystem();
        s.Data = systemData;
        s.DisplayCurrentState();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _system.Solve();
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