using System.Collections.Generic;
using UnityEngine;

namespace Oisif
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

public class LineInterpretor : Interpretor
{
    LineManager _lineManager;
    float _currentAngle;
    Vector3 _origin;
    Vector3 _currentPosition;
    float _lineLength;
    float _lineLengthScaleFactor;
    Bounds _bounds;
    int _segmentCount;
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
    
    void IncreaseAngle(ALSystem system)
    {
        _currentAngle += system.Data.Angle;
    }

    void DecreaseAngle(ALSystem system)
    {
        _currentAngle -= system.Data.Angle;
    }
    
    void SaveDrawState(ALSystem system)
    {
        _savedPositions.Push(new DrawState(_currentPosition, _currentAngle, _lineLength, _segmentCount));
    }
    
    void RestoreDrawState(ALSystem system)
    {
        DrawState drawState = _savedPositions.Pop();
        _currentPosition = drawState.Position;
        _currentAngle = drawState.Angle;
        _lineLength = drawState.LineLength;
        _segmentCount = drawState.SegmentCount;
    }
    
    void MultiplyLineLength(ALSystem system)
    {
        _lineLength *= _lineLengthScaleFactor;
    }

    void DivideLineLength(ALSystem system)
    {
        _lineLength /= _lineLengthScaleFactor;
    }

    void Move(ALSystem system)
    {
        float lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth() - 1));
        _currentPosition = _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
    }

    void DrawLine(ALSystem system)
    {
        float lineLength = _lineLength / Mathf.Pow(system.Data.DepthFactor, (system.Depth() - 1));
        Vector3 start = _currentPosition;
        Vector3 end =  _currentPosition + Quaternion.Euler(0f, 0f, _currentAngle) * new Vector3(lineLength, 0f, 0f);
        _currentPosition = end;

        Vector3 origin = _savedPositions.Count != 0 ? _savedPositions.Peek().Position : _origin;

        _lineManager.CreateInterpolatedLine(_segmentCount, start, start, end);

        _bounds.Encapsulate(start);
        _bounds.Encapsulate(end);
        _segmentCount++;
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
        _segmentCount = 0;
    }

    public override void Execute(ALSystem system)
    {
        base.Execute(system);
        //_lineManager.ExpandLine();
        _lineManager.ExpandLineId();
    }

    public Bounds CameraBounds { get { return _bounds; } }
}

}