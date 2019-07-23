using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem
{
    public class Node
    {
        static int idGenerator = 0;
        int id;
        public char s;

        public Node(char v)
        {
            id = idGenerator;
            idGenerator++;
            s += v;
        }

        public void Set(char v)
        {
            s = v;
        }

        public char value
        { 
            get { return s; }
            set { s = value; }
        }

        public int Id
        { 
            get { return id; }
        }
    }

    TreeNode<Node> _root;
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
            //_root.Traverse(SolveTree);
        }
        _depth++;
    }

    public void DisplayCurrentState()
    {
        if (_root != null)
        {
            string s = "";
            Display(ref s, _root);
            Debug.Log(s);
        }
    }

    public void Display(ref string outGraph, TreeNode<Node> node, int depth = 0)
    {
        foreach (TreeNode<Node> child in node.Children)
        {
            outGraph += node.Value.Id + " " + child.Value.Id + "\n";
            Display(ref outGraph, child, depth + 1);
        }
    }

    void AddNode(ref TreeNode<Node> root, ref TreeNode<Node> current, TreeNode<Node> value)
    {
        if (root == null)
        {
            root = value;
            current = root;
        }
        else
        {
            current.AddChild(value);
        }
    }

    public TreeNode<Node> CreateTree(string state, int index = 0)
    {
        TreeNode<Node> root = null;
        TreeNode<Node> current = null;
        for (int i = index; i < state.Length; i++)
        {
            if (state[i] == '[')
            {
                i++;
                TreeNode<Node> tree = CreateTree(state, i);
                AddNode(ref root, ref current, tree);
                while (state[i] != ']') i++;
            }
            else if (state[i] == ']')
            {
                break;
            }
            else if (state[i] != '+' && state[i] != '-')
            {
                TreeNode<Node> value = new TreeNode<Node>(new Node(state[i]));
                AddNode(ref root, ref current, value);
                current = value;
            }
        }
        return root;
    }

    public TreeNode<Node> Iterate(TreeNode<Node> node)
    {
        Rule rule = _systemData.Rules.Find(r => r.Sign == node.Value.value);

        string s = rule != null ? rule.Result : node.Value.value.ToString();
        TreeNode<Node> root = CreateTree(s);// We need the last element

        foreach (TreeNode<Node> child in node.Children)
        {
            root.AddChild(Iterate(child)); // here add at the last not the root
        }

        return root;
    }

    public SystemData Data
    {
        get { return _systemData; }
        set 
        {
            _systemData = value;
            _root = CreateTree(_systemData.Axiom);
            _depth = 0;
        } 
    }
    public TreeNode<Node> CurrentState { get { return _root; } }
    public float Depth { get { return _depth; } }
}
