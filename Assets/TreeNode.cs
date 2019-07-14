using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using UnityEngine;

public class TreeNode<T>
{
    private T _value;
    private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

    public TreeNode(T value)
    {
        _value = value;
    }

    public TreeNode<T> this[int i]
    {
        get { return _children[i]; }
    }

    public TreeNode<T> Parent { get; private set; }

    public T Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public ReadOnlyCollection<TreeNode<T>> Children
    {
        get { return _children.AsReadOnly(); }
    }

    public TreeNode<T> AddChild(T value)
    {
        var node = new TreeNode<T>(value) {Parent = this};
        _children.Add(node);
        return node;
    }

    public TreeNode<T> AddChild(TreeNode<T> node)
    {
        node.Parent = this;
        _children.Add(node);
        return node;
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return _children.Remove(node);
    }

    public void Traverse(Action<TreeNode<T>> action)
    {
        action(this);
        foreach (var child in _children)
            child.Traverse(action);
    }
}