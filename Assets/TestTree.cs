using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem
{
    TreeNode<string> _root;
    SystemData _systemData;
    int _depth;

    public TreeSystem()
    {
        _root = null;
        _systemData = null;
        _depth = 0;
    }

    public virtual void Solve()
    {
        if (_root != null)
        {
            _root.Traverse(SolveTree);
        }
        _depth++;
    }

    public void DisplayCurrentState()
    {
        if (_root != null)
        {
            _root.Traverse(Display);
        }
    }

    public void Display(TreeNode<string> node)
    {
        Debug.Log(node.Value);
    }

    public TreeNode<string> CreateTreeNode(string state, int index = 0)
    {
        TreeNode<string> node = new TreeNode<string>("");
        for (int i = index; i < state.Length; i++)
        {
            if (state[i] == '[')
            {
                i++;
                node.AddChild(CreateTreeNode(state, i));
                while (state[i] != ']') i++;
            }
            else if (state[i] == ']')
            {
                return node;
            }
            else
            {
                node.Value += state[i];
            }
        }
        return node;
    }

    public void SolveTree(TreeNode<string> node)
    {
        for (int i = 0; i < node.Value.Length; i++)
        {
            if (node.Value[i] == '[')
            {
                //node.AddChild
            }
            else if (node.Value[i] == ']')
            {

            }
            else
            {
                Rule rule = _systemData.Rules.Find(r => r.Sign == node.Value[i]);

                if (rule != null)
                {
                    node.Value = rule.Result;
                }
                else
                {
                    node.Value = rule.Result;
                    //_currentState += prevSystem[i];
                }
            }
        }
    }

    public SystemData Data
    {
        get { return _systemData; }
        set 
        {
            _systemData = value;
            _root = CreateTreeNode(_systemData.Axiom);
            _depth = 0;
        } 
    }
    public TreeNode<string> CurrentState { get { return _root; } }
    public float Depth { get { return _depth; } }
}
