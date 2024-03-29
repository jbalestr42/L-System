﻿using System.Collections.Generic;
using UnityEngine;

namespace Oisif
{

public sealed class LSystem
{
    public enum TokenType
    {
        None,
        Branch,
        Leaf
    }

    // TODO clean up this class to manage different token type (e.g. data to draw line and data to draw circle)
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
        public bool IsNewBranch = false;

        // Data to line token
        public Vector3 Start;
        public Vector3 End;

        public Token(char sign, Token parent, int branchId, int depth, bool isNewBranch = false)
        {
            Sign = sign;
            Parent = parent;
            BranchId = branchId;
            Depth = depth;
            IsNewBranch = isNewBranch;
        }

        public override string ToString()
        {
            return Sign + "(" + DrawableId + "/" + DrawableIdMax + ") - " + BranchId + " - " + Depth + " - " + IsNewBranch + "\n";
        }
    }

    public delegate void SystemSignAction(Token sign);
    LSystemData _systemData;

    List<Token> _root;
    DrawerInterpretor _interpretor;
    int _branchId; // Used to uniquely identify a token

    public LSystem()
    {
        _interpretor = new DrawerInterpretor();
        _systemData = null;
        _root = new List<Token>();
        _branchId = 0;
    }

    public void ForEach(SystemSignAction action)
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

    public void NextGeneration()
    {
        Token prevGeneration = _root[_root.Count - 1];
        Token start = null;
        Token last = null;

        while (prevGeneration != null)
        {
            ARule rule = _systemData.Rules.Find(r => r.Sign == prevGeneration.Sign);

            Token tmp = null;
            Token end = null;
            if (rule != null)
            {
                int index = 0;
                string result = rule.Evaluate(prevGeneration); // verifier si ca fonctionne correctement, j'ai un petit doute
                tmp = GenerateTokens(prevGeneration, result, out end, ref index);
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

    public void DisplayCurrentState()
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
        if (newToken != null)
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
    }

    Token GenerateTokens(Token parent, string input, out Token end, ref int i, bool isNewBranch = false)
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
                tmp = new Token(sign, parent, currentBranchId, Depth(), isNewBranch);
                AddToken(ref start, ref last, tmp);

                // TODO: [MUST FIX] This is bad, the system shouldn't know about the interpretor
                DrawerInterpretor.ActionData data =_interpretor.GetActionData(sign);
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
            }
            last = last.Next;
        }
        return start;
    }

    public LSystemData Data {
        get { return _systemData; }
        set
        {
            _systemData = value;
            Reset();
        } 
    }

    public void Reset()
    {
        _root.Clear();
        _branchId = 0;
        int index = 0;
        Token end = null;
        _root.Add(GenerateTokens(null, _systemData.Axiom, out end, ref index));
    }

    public int Depth()
    {
        return _root.Count;
    }
}

}