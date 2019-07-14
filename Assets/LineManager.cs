using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public GameObject _linePrefab;

    List<GameObject> _lines;

    public struct InterpolatedLine
    {
        public LineRenderer Line;
        public Vector3 Origin;
        public Vector3 Start;
        public Vector3 End;
    }
    List<InterpolatedLine> _interpolatedLines;
    GameObject _root;

    Dictionary<int, List<InterpolatedLine>> _interpolatedLinesId;

    void Start()
    {
        _lines = new List<GameObject>();
        _interpolatedLines = new List<InterpolatedLine>();
        _interpolatedLinesId = new Dictionary<int, List<InterpolatedLine>>();
        _root = new GameObject();
        _root.name = "Lines";
    }

    public void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject gameObject = GameObject.Instantiate(_linePrefab);
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        gameObject.transform.SetParent(_root.transform);
        _lines.Add(gameObject);
    }

    public void CreateInterpolatedLine(Vector3 start, Vector3 end)
    {
        GameObject gameObject = GameObject.Instantiate(_linePrefab);
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, start);
        gameObject.transform.SetParent(_root.transform);
        InterpolatedLine interpolatedLine = new InterpolatedLine();
        interpolatedLine.Start = start;
        interpolatedLine.End = end;
        interpolatedLine.Line = line;
        _interpolatedLines.Add(interpolatedLine);
    }
    
    public void CreateInterpolatedLine(int id, Vector3 origin, Vector3 start, Vector3 end)
    {
        GameObject gameObject = GameObject.Instantiate(_linePrefab);
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, start);
        gameObject.transform.SetParent(_root.transform);
        InterpolatedLine interpolatedLine = new InterpolatedLine();
        interpolatedLine.Origin = origin;
        interpolatedLine.Start = start;
        interpolatedLine.End = end;
        interpolatedLine.Line = line;
        
        if (!_interpolatedLinesId.ContainsKey(id))
        {
            _interpolatedLinesId[id] = new List<InterpolatedLine>();
        }
        _interpolatedLinesId[id].Add(interpolatedLine);
    }

    public void ExpandLine()
    {
        StartCoroutine(ExpandLineCor());
    }

    public void ExpandLineId()
    {
        StartCoroutine(ExpandLineIdCor(0));
    }

    IEnumerator ExpandLineCor()
    {
        float timer = 0f;
        while (timer < 2f)
        {
            foreach (InterpolatedLine interpolatedLine in _interpolatedLines)
            {
                //interpolatedLine.Line.SetPosition(0, Vector3.Lerp(interpolatedLine.Origin, interpolatedLine.Start, Mathf.Min(timer, 2f) / 2f));
                interpolatedLine.Line.SetPosition(1, Vector3.Lerp(interpolatedLine.Start, interpolatedLine.End, Mathf.Min(timer, 2f) / 2f));
            }
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator ExpandLineIdCor(int id)
    {
        float timer = 0f;
        float timerMax = 0.1f;
        if (_interpolatedLinesId.ContainsKey(id))
        {
            while (timer < timerMax)
            {
                timer += Time.deltaTime;
                foreach (InterpolatedLine interpolatedLine in _interpolatedLinesId[id])
                {
                    interpolatedLine.Line.SetPosition(0, Vector3.Lerp(interpolatedLine.Origin, interpolatedLine.Start, Mathf.Min(timer, timerMax) / timerMax));
                    interpolatedLine.Line.SetPosition(1, Vector3.Lerp(interpolatedLine.Origin, interpolatedLine.End, Mathf.Min(timer, timerMax) / timerMax));
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            StartCoroutine(ExpandLineIdCor(++id));
        }
    }

    public void Clear()
    {
        if (_lines != null)
        {
            foreach (GameObject line in _lines)
            {
                GameObject.Destroy(line);
            }
            _lines.Clear();
        }
        if (_interpolatedLines != null)
        {
            foreach (InterpolatedLine interpolatedLine in _interpolatedLines)
            {
                GameObject.Destroy(interpolatedLine.Line.gameObject);
            }
            _interpolatedLines.Clear();
        }
        if (_interpolatedLinesId != null)
        {
            foreach (KeyValuePair<int, List<InterpolatedLine>> interpolatedLineId in _interpolatedLinesId)
            {
                foreach (InterpolatedLine interpolatedLine in interpolatedLineId.Value)
                {
                    GameObject.Destroy(interpolatedLine.Line.gameObject);
                }
            }
            _interpolatedLinesId.Clear();
        }
    }
}
