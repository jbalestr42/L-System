using System.Collections.Generic;
using UnityEngine;

namespace Oisif
{

public class Token
{
    public char Sign = '.';
    public Token Prev = null;
    public Token Next = null;
    public Token Parent = null;

    public Token(char sign, Token parent)
    {
        Sign = sign;
        Parent = parent;
    }
}

public class TokenSystem : ALSystem
{
    SystemData _systemData;
    List<Token> _root;

    public TokenSystem()
    {
        _systemData = null;
        _root = new List<Token>();
    }

    public override void ForEach(SystemSignAction action)
    {
        if (action != null)
        {
            Token currentSystem = _root[_root.Count - 1];

            while (currentSystem != null)
            {
                action(currentSystem.Sign);
                currentSystem = currentSystem.Next;
            }
        }
    }

    public override void NextGeneration()
    {
        Token prevGeneration = _root[_root.Count - 1];
        Token start = null;
        Token last = null;

        while (prevGeneration != null)
        {
            Rule rule = _systemData.Rules.Find(r => r.Sign == prevGeneration.Sign);
            string input = rule != null ? rule.Result : prevGeneration.Sign.ToString();

            Token tmp = GenerateTokens(prevGeneration, input);
            if (start == null)
            {
                start = tmp;
            }
            else
            {
                last.Next = tmp;
            }

            while (tmp.Next != null)
            {
                tmp = tmp.Next;
            }
            last = tmp;

            prevGeneration = prevGeneration.Next;
        }
        _root.Add(start);
    }

    public override void DisplayCurrentState()
    {
        string s = "";
        ALSystem.SystemSignAction stringify = (char sign) => s += sign;
        ForEach(stringify);
        Debug.Log("Current state " + s);
    }

    Token GenerateTokens(Token parent, string input)
    {
        Token start = null;
        Token last = null;
        for (int i = 0; i < input.Length; i++)
        {
            Token tmp = new Token(input[i], parent);
            if (start == null)
            {
                start = tmp;
                last = start;
            }
            else
            {
                last.Next = tmp;
                tmp.Prev = last;
                last = tmp;
            }
        }
        return start;
    }

    public override SystemData Data {
        get { return _systemData; }
        set {
            _systemData = value;
            _root.Add(GenerateTokens(null, _systemData.Axiom));
        } 
    }

    public override int Depth()
    {
        return _root.Count;
    }
}

}