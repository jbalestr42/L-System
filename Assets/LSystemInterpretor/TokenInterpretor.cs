using System.Collections.Generic;
using UnityEngine;

namespace Oisif
{

public class TokenInterpretor : Interpretor<LSystem>
{
    public struct DrawState
    {
        public Vector3 _position;
        public float _angle;
        public float _lineLength;
        public int _segmentCount;
    
        public DrawState(Vector3 position, float angle, float lineLength, int segmentCount)
        {
            _position = position;
            _angle = angle;
            _lineLength = lineLength;
            _segmentCount = segmentCount;
        }

        public Vector3 Position { get { return _position; } set { _position = value; } }
        public float Angle { get { return _angle; } set { _angle = value; } }
        public float LineLength { get { return _lineLength; } }
        public int SegmentCount { get { return _segmentCount; } }
    }

    RenderManager _RenderManager;
    float _currentAngle;
    Vector3 _origin;
    Vector3 _currentPosition;
    float _lineLength;
    float _lineLengthScaleFactor;
    int _segmentCount;
    Stack<DrawState> _savedPositions;

    public TokenInterpretor()
        :base()
    {
        _RenderManager = Object.FindObjectOfType<RenderManager>();
        _savedPositions = new Stack<DrawState>();

        AddAction('+', false, IncreaseAngle);
        AddAction('-', false, DecreaseAngle);
        AddAction('G', true, DrawLine);
        AddAction('F', true, DrawLine);
        AddAction('f', true, Move);
        AddAction('[', false, SaveDrawState);
        AddAction(']', false, RestoreDrawState);
        AddAction('>', false, MultiplyLineLength);
        AddAction('<', false, DivideLineLength);
        Reset();
    }
    
    void IncreaseAngle(LSystem.Token token, LSystem system)
    {
        _currentAngle += system.Data.Angle;
    }

    void DecreaseAngle(LSystem.Token token, LSystem system)
    {
        _currentAngle -= system.Data.Angle;
    }
    
    void SaveDrawState(LSystem.Token token, LSystem system)
    {
        _savedPositions.Push(new DrawState(_currentPosition, _currentAngle, _lineLength, _segmentCount));
    }
    
    void RestoreDrawState(LSystem.Token token, LSystem system)
    {
        DrawState drawState = _savedPositions.Pop();
        _currentPosition = drawState.Position;
        _currentAngle = drawState.Angle;
        _lineLength = drawState.LineLength;
        _segmentCount = drawState.SegmentCount;
    }
    
    void MultiplyLineLength(LSystem.Token token, LSystem system)
    {
        _lineLength *= _lineLengthScaleFactor;
    }

    void DivideLineLength(LSystem.Token token, LSystem system)
    {
        _lineLength /= _lineLengthScaleFactor;
    }

    void Move(LSystem.Token token, LSystem system)
    {
        float lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth() - 1));
        _currentPosition = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
    }

    void DrawLine(LSystem.Token token, LSystem system)
    {
        float lineLength = _lineLength;
        Vector3 offset = Vector3.zero;
        Vector3 start1 = _currentPosition;
        Vector3 start2 = _currentPosition;

        bool isParentDrawable = false;
        if (token.Parent != null)
        {
            if (ContainsAction(token.Parent.Sign))
            {
                ActionData data = GetActionData(token.Parent.Sign);
                isParentDrawable = data.IsDrawable;
            }
        }

        Vector3 start = _currentPosition;
        Vector3 end = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
        if (token.Parent != null)
        {
            if (token.ShouldExpand || !isParentDrawable)
            {
                lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth() - 1));

                // Begin of line
                start1 = _currentPosition;
                start2 = _currentPosition;

                // End of line
                start = start1;
                end = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
            }
            else
            {
                // Split the current branch
                float ratioStart = (float)(token.DrawableId - 1) / (float)token.DrawableIdMax;
                float ratioEnd = (float)token.DrawableId / (float)token.DrawableIdMax;

                // Begin of line
                start1 = token.Parent.Start + (token.Parent.End - token.Parent.Start) * ratioStart;
                start2 = token.Parent.Start + (token.Parent.End - token.Parent.Start) * ratioEnd;
                
                // End of line
                start = start1;
                end = start2;
            }
        }

        _currentPosition = end;

        // if isParentDrawable -> changer l'ordre de dessin
        _RenderManager.CreateInterpolatedLine(token.Depth + token.DrawableId, start1, start, start2, end);
        //Debug.Log("New line - token: " + token.ToString() + " | start1: " + start1.ToString("F2") + " | start : " + start.ToString("F2") + " | start2: " + start2.ToString("F2") + " |end: " + end.ToString("F2"));

        token.Start = start;
        token.End = end;

        _segmentCount++;
    }

    public void Reset()
    {
        _origin = Vector3.one;
        _currentPosition = _origin;
        _currentAngle = 0f;
        _lineLength = 5f;
        _lineLengthScaleFactor = 0.7f;
        _RenderManager.Clear();
        _savedPositions.Clear();
        _segmentCount = 0;
    }

    public override void Execute(LSystem system)
    {
        base.Execute(system);
        _RenderManager.Expand();
    }
}

}