using System.Collections.Generic;
using UnityEngine;

namespace Oisif
{

public class DrawerInterpretor : LSystemInterpretor
{
    public struct DrawState
    {
        public Vector3 Position;
        public float Angle;
        public float LineLength;
        public int SegmentCount;
    
        public DrawState(Vector3 position, float angle, float lineLength, int segmentCount)
        {
            Position = position;
            Angle = angle;
            LineLength = lineLength;
            SegmentCount = segmentCount;
        }
    }

    RenderManager _RenderManager;
    float _currentAngle;
    Vector3 _origin;
    Vector3 _currentPosition;
    float _lineLength;
    float _lineLengthScaleFactor;
    int _segmentCount;
    Stack<DrawState> _savedPositions;

    public DrawerInterpretor()
        :base()
    {
        _RenderManager = Object.FindObjectOfType<RenderManager>();
        _savedPositions = new Stack<DrawState>();

        AddAction('+', false, IncreaseAngle);
        AddAction('-', false, DecreaseAngle);
        AddAction('Y', true, DrawLine); // TODO add a way to add multiple sign doing the same action
        AddAction('G', true, DrawLine);
        AddAction('F', true, DrawLine);
        AddAction('E', true, DrawLine);
        AddAction('C', false, DrawCircle);
        AddAction('f', true, Move);
        AddAction('[', false, SaveDrawState);
        AddAction(']', false, RestoreDrawState);
        AddAction('>', false, MultiplyLineLength);
        AddAction('<', false, DivideLineLength);
        Reset();
    }
    
    void IncreaseAngle(LSystem.Token token, LSystem system)
    {
        _currentAngle += system.Data.Angle.Value();
    }

    void DecreaseAngle(LSystem.Token token, LSystem system)
    {
        _currentAngle -= system.Data.Angle.Value();
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
        Vector3 pointStartA = _currentPosition;
        Vector3 pointStartB = _currentPosition;

        bool isParentDrawable = false;
        if (token.Parent != null)
        {
            if (ContainsAction(token.Parent.Sign))
            {
                ActionData data = GetActionData(token.Parent.Sign);
                isParentDrawable = data.IsDrawable;
            }
        }

        Vector3 pointEndA = _currentPosition;
        Vector3 pointEndB = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
        if (token.Parent != null)
        {
            if (token.IsNewBranch || !isParentDrawable)
            {
                lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth() - 1));

                // Begin of line
                pointStartA = _currentPosition;
                pointStartB = _currentPosition;

                // End of line
                pointEndA = pointStartA;
                pointEndB = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
            }
            else
            {
                // Split the current branch
                float ratioStart = (float)(token.DrawableId - 1) / (float)token.DrawableIdMax;
                float ratioEnd = (float)token.DrawableId / (float)token.DrawableIdMax;

                // Begin of line
                pointStartA = token.Parent.Start + (token.Parent.End - token.Parent.Start) * ratioStart;
                pointStartB = token.Parent.Start + (token.Parent.End - token.Parent.Start) * ratioEnd;
                
                // End of line
                pointEndA = pointStartA;
                pointEndB = pointStartB;
            }
        }

        _currentPosition = pointEndB;

        // if isParentDrawable -> changer l'ordre de dessin
        _RenderManager.CreateInterpolableLine(token.Depth + token.DrawableId, pointStartA, pointEndA, pointStartB, pointEndB);
        //Debug.Log("New line - token: " + token.ToString() + " | pointStartA: " + start1.ToString("F2") + " | pointEndA : " + pointEndA.ToString("F2") + " | pointStartB: " + pointStartB.ToString("F2") + " |pointEndB: " + pointEndB.ToString("F2"));

        token.Start = pointEndA;
        token.End = pointEndB;
        _segmentCount++;
    }
    
    void DrawCircle(LSystem.Token token, LSystem system)
    {
        float sizeStart = _lineLength;
        float sizeEnd = sizeStart;

        if (token.Parent == null || token.Parent.Sign != 'C')
        {
            // New circle
            sizeStart = 0f;
        }
        _RenderManager.CreateInterpolableCircle(token.Depth + token.DrawableId, _currentPosition, _currentPosition, sizeStart, sizeEnd);

        token.Start = _currentPosition;
        token.End = _currentPosition;
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

    public override bool IsDoneExecuting()
    {
        return _RenderManager.IsDoneExpanding();
    }
}

}