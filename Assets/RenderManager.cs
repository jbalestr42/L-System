using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager : MonoBehaviour
{
    // Represent a line from Point A to Point B interpolating from Start to End
    struct InterpolatedLine
    {
        public LineRenderer Line;
        public Vector3 PointStartA;
        public Vector3 PointEndA;
        public Vector3 PointStartB;
        public Vector3 PointEndB;
    }

    [SerializeField]
    private GameObject _linePrefab;

    private GameObject _root;
    private List<GameObject> _lines;
    private SortedDictionary<int, List<InterpolatedLine>> _interpolatedLines;

    void Start()
    {
        _root = new GameObject();
        _root.name = "Lines";
        _lines = new List<GameObject>();
        _interpolatedLines = new SortedDictionary<int, List<InterpolatedLine>>();
    }
    
    public void CreateInterpolatedLine(int order, Vector3 pointStartA, Vector3 pointEndA, Vector3 pointStartB, Vector3 pointEndB)
    {
        GameObject gameObject = GameObject.Instantiate(_linePrefab);
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, pointStartA);
        line.SetPosition(1, pointStartB);
        gameObject.transform.SetParent(_root.transform);

        InterpolatedLine interpolatedLine = new InterpolatedLine();
        interpolatedLine.PointStartA = pointStartA;
        interpolatedLine.PointEndA = pointEndA;
        interpolatedLine.PointStartB = pointStartB;
        interpolatedLine.PointEndB = pointEndB;
        interpolatedLine.Line = line;
        
        if (!_interpolatedLines.ContainsKey(order))
        {
            _interpolatedLines[order] = new List<InterpolatedLine>();
        }
        _interpolatedLines[order].Add(interpolatedLine);
    }

    public void ExpandLines()
    {
        int count = 0;
        foreach (KeyValuePair<int, List<InterpolatedLine>> interpolatedLine in _interpolatedLines)
        {
            count += interpolatedLine.Value.Count;
        }
        Debug.Log("Interpoling " + count + " lines.");
        
        StartCoroutine(ExpandLinesCor());
    }
    
    // Start interpolating lines based on which order they were created
    IEnumerator ExpandLinesCor()
    {
        float duration = 0.2f;

        foreach (KeyValuePair<int, List<InterpolatedLine>> pair in _interpolatedLines)
        {
            Debug.Log("Expand moving line. Id: " + pair.Key + ". Count: " + pair.Value.Count);
            
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                foreach (InterpolatedLine interpolatedLine in pair.Value)
                {
                    interpolatedLine.Line.SetPosition(0, Vector3.Lerp(interpolatedLine.PointStartA, interpolatedLine.PointEndA, Mathf.Min(timer, duration) / duration));
                    interpolatedLine.Line.SetPosition(1, Vector3.Lerp(interpolatedLine.PointStartB, interpolatedLine.PointEndB, Mathf.Min(timer, duration) / duration));
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
            foreach (KeyValuePair<int, List<InterpolatedLine>> interpolatedLineId in _interpolatedLines)
            {
                foreach (InterpolatedLine interpolatedLine in interpolatedLineId.Value)
                {
                    GameObject.Destroy(interpolatedLine.Line.gameObject);
                }
            }
            _interpolatedLines.Clear();
        }
    }
}
