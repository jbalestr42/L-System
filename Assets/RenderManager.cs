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

    // Represent a line from Point A to Point B interpolating from Start to End
    struct InterpolatedCircle
    {
        public SpriteRenderer Sprite;
        public Vector3 CenterStart;
        public Vector3 CenterEnd;
        public float SizeStart;
        public float SizeEnd;
    }

    [SerializeField]
    private GameObject _linePrefab;

    [SerializeField]
    private GameObject _circlePrefab;

    private GameObject _root;
    private List<GameObject> _lines;
    private SortedDictionary<int, List<InterpolatedLine>> _interpolatedLines;
    private SortedDictionary<int, List<InterpolatedCircle>> _interpolatedCircles;

    void Start()
    {
        _root = new GameObject();
        _root.name = "Lines";
        _lines = new List<GameObject>();
        _interpolatedLines = new SortedDictionary<int, List<InterpolatedLine>>();
        _interpolatedCircles = new SortedDictionary<int, List<InterpolatedCircle>>();
    }
    
    public void CreateCircle(int order, Vector3 centerStart, Vector3 centerEnd, float sizeStart, float sizeEnd)
    {
        GameObject gameObject = GameObject.Instantiate(_circlePrefab);
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.transform.position = centerStart;
        sprite.transform.localScale = Vector3.zero;
        gameObject.transform.SetParent(_root.transform);

        InterpolatedCircle interpolatedCircle = new InterpolatedCircle();
        interpolatedCircle.Sprite = sprite;
        interpolatedCircle.CenterStart = centerStart;
        interpolatedCircle.CenterEnd = centerEnd;
        interpolatedCircle.SizeStart = sizeStart;
        interpolatedCircle.SizeEnd = sizeEnd;

        if (!_interpolatedCircles.ContainsKey(order))
        {
            _interpolatedCircles[order] = new List<InterpolatedCircle>();
        }
        _interpolatedCircles[order].Add(interpolatedCircle);
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

    public void Expand()
    {
        int count = 0;
        foreach (KeyValuePair<int, List<InterpolatedLine>> interpolatedLine in _interpolatedLines)
        {
            count += interpolatedLine.Value.Count;
        }
        Debug.Log("Interpoling " + count + " lines.");
        
        count = 0;
        foreach (KeyValuePair<int, List<InterpolatedCircle>> interpolatedCircle in _interpolatedCircles)
        {
            count += interpolatedCircle.Value.Count;
        }
        Debug.Log("Interpoling " + count + " circles.");

        StartCoroutine(ExpandLinesCor());
        StartCoroutine(ExpandCirclesCor());
    }
    
    // Start interpolating lines based on which order they were created
    IEnumerator ExpandLinesCor()
    {
        float duration = 0.2f;

        foreach (KeyValuePair<int, List<InterpolatedLine>> pair in _interpolatedLines)
        {
            Debug.Log("Expanding  line. Id: " + pair.Key + ". Count: " + pair.Value.Count);
            
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
    
    // Start interpolating circles based on which order they were created
    IEnumerator ExpandCirclesCor()
    {
        float duration = 0.2f;

        foreach (KeyValuePair<int, List<InterpolatedCircle>> pair in _interpolatedCircles)
        {
            Debug.Log("Expanding circle. Id: " + pair.Key + ". Count: " + pair.Value.Count);
            
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                foreach (InterpolatedCircle interpolatedCircle in pair.Value)
                {
                    float scale = Mathf.Lerp(interpolatedCircle.SizeStart, interpolatedCircle.SizeEnd, Mathf.Min(timer, duration) / duration);
                    interpolatedCircle.Sprite.transform.position = Vector3.Lerp(interpolatedCircle.CenterStart, interpolatedCircle.CenterEnd, Mathf.Min(timer, duration) / duration);
                    interpolatedCircle.Sprite.transform.localScale = new Vector3(scale, scale, scale);
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
            foreach (KeyValuePair<int, List<InterpolatedLine>> interpolatedLines in _interpolatedLines)
            {
                foreach (InterpolatedLine interpolatedLine in interpolatedLines.Value)
                {
                    GameObject.Destroy(interpolatedLine.Line.gameObject);
                }
            }
            _interpolatedLines.Clear();
        }

        if (_interpolatedCircles != null)
        {
            foreach (KeyValuePair<int, List<InterpolatedCircle>> interpolatedCircles in _interpolatedCircles)
            {
                foreach (InterpolatedCircle interpolatedCircle in interpolatedCircles.Value)
                {
                    GameObject.Destroy(interpolatedCircle.Sprite.gameObject);
                }
            }
            _interpolatedCircles.Clear();
        }
    }
}
