using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem
{
    public enum ENodeType
    {
        Root = 0,
        Branch,
        Leaf,
        End
    }
    static string[] NodeTypeName = { "root", "branch", "leaf", "end" };

    public class Node
    {
        static int idGenerator = 0;
        int id;
        public char s;
        public ENodeType t;

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

        public ENodeType Type
        {
            get { return t; }
            set { t = value; }
        }
    }

    public class TreeData<T>
    {
        public TreeNode<T> start;
        public TreeNode<T> end;

        public TreeData()
        {
            start = null;
            end = null;
        }
    }

    TreeData<Node> _tree;
    SystemData _systemData;
    int _depth;

    public TreeSystem()
    {
        _systemData = null;
        _depth = 0;
    }

    public void DisplayCurrentState()
    {
        string s = "";
        Display(ref s, _tree.start);
        Debug.Log("Root: " + _tree.start.Value.value);
        Debug.Log("End: " + _tree.end.Value.value);
        Debug.Log(s);
    }

    public void Display(ref string outGraph, TreeNode<Node> node, int depth = 0)
    {
        foreach (TreeNode<Node> child in node.Children)
        {
            outGraph += node.Value.Id + "_" + NodeTypeName[(int)node.Value.Type] + " " + child.Value.Id + "_" + NodeTypeName[(int)child.Value.Type] + "\n";
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
            current.Value.Type = ENodeType.Branch;
        }
    }

    public void CreateTree(string state)
    {
        _tree = new TreeData<Node>();
        int i = 0;
        CreateTree(_tree, state, ref i);
    }

    // Create a tree from a given string (e.g. FF+[+F-F[+FF]-F]-F[-F+F+F])
    public TreeNode<Node> CreateTree(TreeData<Node> treeData, string state, ref int i)
    {
        TreeNode<Node> root = null;
        TreeNode<Node> current = null;
        for (; i < state.Length; i++)
        {
            if (state[i] == '[')
            {
                i++;
                TreeNode<Node> tree = CreateTree(null, state, ref i);
                AddNode(ref root, ref current, tree);
                i++;
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
                if (treeData != null)
                {
                    treeData.end = current;
                }
            }
        }
        current.Value.Type = ENodeType.Leaf;
        if (treeData != null)
        {
            treeData.start = root;
            treeData.start.Value.Type = ENodeType.Root;
            treeData.end.Value.Type = ENodeType.End;
        }
        return root;
    }

    public TreeNode<Node> Iterate(TreeData<Node> tree)
    {
        Rule rule = _systemData.Rules.Find(r => r.Sign == tree.start.Value.value);

        string s = rule != null ? rule.Result : tree.start.Value.value.ToString();
        TreeNode<Node> root = null;// = CreateTree(tree, s);// We need the last element

        foreach (TreeNode<Node> child in tree.start.Children)
        {
  //          root.AddChild(Iterate(child)); // here add at the last not the root
        }

        return root;
    }

    public SystemData Data
    {
        get { return _systemData; }
        set 
        {
            _systemData = value;
            _depth = 0;
            CreateTree(_systemData.Axiom);
        } 
    }
    public float Depth { get { return _depth; } }
}
