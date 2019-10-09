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
    
    public struct InterpolatedMovingLine
    {
        public LineRenderer Line;
        public Vector3 Start1;
        public Vector3 End1;
        public Vector3 Start2;
        public Vector3 End2;
    }

    List<InterpolatedLine> _interpolatedLines;
    GameObject _root;

    Dictionary<int, List<InterpolatedLine>> _interpolatedLinesId;
    SortedDictionary<int, List<InterpolatedMovingLine>> _interpolatedMovingLinesId;

    void Start()
    {
        _lines = new List<GameObject>();
        _interpolatedLines = new List<InterpolatedLine>();
        _interpolatedLinesId = new Dictionary<int, List<InterpolatedLine>>();
        _interpolatedMovingLinesId = new SortedDictionary<int, List<InterpolatedMovingLine>>();
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
    
    public void CreateInterpolatedLine(int id, Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
    {
        GameObject gameObject = GameObject.Instantiate(_linePrefab);
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, start1);
        line.SetPosition(1, start2);
        gameObject.transform.SetParent(_root.transform);
        InterpolatedMovingLine interpolatedLine = new InterpolatedMovingLine();
        interpolatedLine.Start1 = start1;
        interpolatedLine.End1 = end1;
        interpolatedLine.Start2 = start2;
        interpolatedLine.End2 = end2;
        interpolatedLine.Line = line;
        
        if (!_interpolatedMovingLinesId.ContainsKey(id))
        {
            _interpolatedMovingLinesId[id] = new List<InterpolatedMovingLine>();
        }
        _interpolatedMovingLinesId[id].Add(interpolatedLine);
    }

    public void ExpandLine()
    {
        StartCoroutine(ExpandLineCor());
    }

    public void ExpandLineId()
    {
        int count = 0;
        foreach (KeyValuePair<int, List<InterpolatedLine>> interpolatedLine in _interpolatedLinesId)
        {
            count += interpolatedLine.Value.Count;
        }
        Debug.Log("Interpoling " + count + " lines.");
        StartCoroutine(ExpandLineIdCor(0));
    }

    public void ExpandMovingLineId()
    {
        int count = 0;
        foreach (KeyValuePair<int, List<InterpolatedMovingLine>> interpolatedLine in _interpolatedMovingLinesId)
        {
            count += interpolatedLine.Value.Count;
        }
        Debug.Log("Interpoling " + count + " lines.");
        StartCoroutine(ExpandMovingLineIdCor());
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
    
    IEnumerator ExpandMovingLineIdCor()
    {
        float timerMax = 0.2f;

        foreach (KeyValuePair<int, List<InterpolatedMovingLine>> pair in _interpolatedMovingLinesId)
        {
            float timer = 0f;
            Debug.Log("Expand moving line. Id: " + pair.Key + ". Count: " + pair.Value.Count);
            while (timer < timerMax)
            {
                timer += Time.deltaTime;
                foreach (InterpolatedMovingLine interpolatedLine in pair.Value)
                {
                    interpolatedLine.Line.SetPosition(0, Vector3.Lerp(interpolatedLine.Start1, interpolatedLine.End1, Mathf.Min(timer, timerMax) / timerMax));
                    interpolatedLine.Line.SetPosition(1, Vector3.Lerp(interpolatedLine.Start2, interpolatedLine.End2, Mathf.Min(timer, timerMax) / timerMax));
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
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
        if (_interpolatedMovingLinesId != null)
        {
            foreach (KeyValuePair<int, List<InterpolatedMovingLine>> interpolatedLineId in _interpolatedMovingLinesId)
            {
                foreach (InterpolatedMovingLine interpolatedLine in interpolatedLineId.Value)
                {
                    GameObject.Destroy(interpolatedLine.Line.gameObject);
                }
            }
            _interpolatedMovingLinesId.Clear();
        }
    }
}
