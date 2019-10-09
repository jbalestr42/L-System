using System.Collections.Generic;
using UnityEngine;

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

public class LSystem
{
    public enum TokenType
    {
        Branch,
        Leaf
    }

    public class Token
    {
        public char Sign = '.';
        public Token Prev = null;
        public Token Next = null;
        public Token Parent = null;
        public int DrawableId = 1;
        public int DrawableIdMax = 1;
        public int Depth = 0;
        public int BranchId = 0;
        public TokenType Type = TokenType.Branch;
        public bool ShouldExpand = false;

        public Vector3 Start;
        public Vector3 End;

        public Token(char sign, Token parent, int branchId, int depth, bool shouldExpand = false)
        {
            Sign = sign;
            Parent = parent;
            BranchId = branchId;
            Depth = depth;
            ShouldExpand = shouldExpand;
        }

        public override string ToString()
        {
            return Sign + "(" + DrawableId + "/" + DrawableIdMax + ") - " + Type.ToString() + " - " + BranchId + " - " + Depth + "\n";
        }
    }

    public delegate void SystemSignAction(Token sign);
    SystemData _systemData;

    List<Token> _root;
    TokenInterpretor _interpretor;
    public int _branchId;

    public LSystem()
    {
        _interpretor = new TokenInterpretor();
        _systemData = null;
        _root = new List<Token>();
        _branchId = 0;
    }

    public virtual void ForEach(SystemSignAction action)
    {
        if (action != null)
        {
            Token currentSystem = _root[_root.Count - 1];

            while (currentSystem != null)
            {
                action(currentSystem);
                currentSystem = currentSystem.Next;
            }
        }
    }

    public virtual void NextGeneration()
    {
        Token prevGeneration = _root[_root.Count - 1];
        Token start = null;
        Token last = null;

        while (prevGeneration != null)
        {
            Rule rule = _systemData.Rules.Find(r => r.Sign == prevGeneration.Sign);

            Token tmp = null;
            Token end = null;
            if (rule != null)
            {
                int index = 0;
                tmp = GenerateTokens(prevGeneration, rule.Result, out end, ref index);
            }
            else
            {
                tmp = new Token(prevGeneration.Sign, prevGeneration, prevGeneration.BranchId, Depth());
            }

            AddToken(ref start, ref last, tmp);

            if (end != null)
            {
                last = end;
            }

            prevGeneration = prevGeneration.Next;
        }
        _root.Add(start);
    }

    public virtual void DisplayCurrentState()
    {
        Debug.LogWarning("Be carful this log is expensive !!");
        string s = "";
        string sv = "";
        LSystem.SystemSignAction stringify = (Token t) => s += t.Sign;
        LSystem.SystemSignAction stringifyVerbose = (Token t) => sv += t.ToString();
        ForEach(stringify);
        ForEach(stringifyVerbose);
        Debug.Log("Current state:\n" + s);
        Debug.Log("Current state:\n" + sv);
    }

    void AddToken(ref Token start, ref Token last, Token newToken)
    {
        if (start == null)
        {
            start = newToken;
            last = start;
        }
        else
        {
            last.Next = newToken;
            newToken.Prev = last;
            last = newToken;
        }
    }

    Token GenerateTokens(Token parent, string input, out Token end, ref int i, bool shouldExpand = false)
    {
        Token start = null;
        Token last = null;
        int drawableId = 0;
        int currentBranchId = _branchId;
        _branchId++;

        for (; i < input.Length; i++)
        {
            char sign = input[i];
            Token tmp = null;

            if (sign == '[')
            {
                AddToken(ref start, ref last, new Token(sign, parent, currentBranchId, Depth()));
                i++;
                Token endTmp = null;
                tmp = GenerateTokens(parent, input, out endTmp, ref i, true);
                AddToken(ref start, ref last, tmp);
                if (endTmp != null)
                {
                    last = endTmp;
                }

                AddToken(ref start, ref last, new Token(input[i], parent, currentBranchId, Depth()));
            }
            else if (sign == ']')
            {
                break;
            }
            else
            {
                tmp = new Token(sign, parent, currentBranchId, Depth(), shouldExpand);
                AddToken(ref start, ref last, tmp);

                TokenInterpretor.ActionData data =_interpretor.GetActionData(sign);
                if (data.IsDrawable)
                {
                    drawableId++;
                    tmp.DrawableId = drawableId;
                }
            }
        }

        end = last;
        last = start;
        while (last != null)
        {
            if (currentBranchId == last.BranchId)
            {
                last.DrawableIdMax = drawableId;
                last.Type = TokenType.Branch;
            }
            last = last.Next;
        }
        end.Type = TokenType.Leaf;
        return start;
    }

    public virtual SystemData Data {
        get { return _systemData; }
        set {
            _root.Clear();
            _branchId = 0;
            _systemData = value;
            int index = 0;
            Token end = null;
            _root.Add(GenerateTokens(null, _systemData.Axiom, out end, ref index));
        } 
    }

    public virtual int Depth()
    {
        return _root.Count;
    }
}

}