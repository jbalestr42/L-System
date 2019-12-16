using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager : MonoBehaviour
{
    interface IInterpolableShape
    {
        void Update(float percent);
        void Destroy();
    }

    // Represent a line from Point A to Point B interpolating from Start to End
    struct InterpolableLine : IInterpolableShape
    {
        public LineRenderer Line;
        public Vector3 PointStartA;
        public Vector3 PointEndA;
        public Vector3 PointStartB;
        public Vector3 PointEndB;

        public void Update(float percent)
        {
            Line.SetPosition(0, Vector3.Lerp(PointStartA, PointEndA, percent));
            Line.SetPosition(1, Vector3.Lerp(PointStartB, PointEndB, percent));
        }

        public void Destroy()
        {
            GameObject.Destroy(Line.gameObject);
        }
    }

    // Represent a circle interpolating center to SizeStart to SizeSEnd
    struct InterpolableCircle : IInterpolableShape
    {
        public SpriteRenderer Sprite;
        public Vector3 CenterStart;
        public Vector3 CenterEnd;
        public float SizeStart;
        public float SizeEnd;

        public void Update(float percent)
        {
            float scale = Mathf.Lerp(SizeStart, SizeEnd, percent);
            Sprite.transform.position = Vector3.Lerp(CenterStart, CenterEnd, percent);
            Sprite.transform.localScale = new Vector3(scale, scale, scale);
        }

        public void Destroy()
        {
            GameObject.Destroy(Sprite.gameObject);
        }
    }

    [SerializeField]
    private GameObject _linePrefab = null;

    [SerializeField]
    private GameObject _circlePrefab = null;

    private GameObject _root;
    private SortedDictionary<int, List<InterpolableLine>> _interpolatedLines;
    private SortedDictionary<int, List<InterpolableCircle>> _interpolatedCircles;

    void Start()
    {
        _root = new GameObject();
        _root.name = "RenderManager";
        _interpolatedLines = new SortedDictionary<int, List<InterpolableLine>>();
        _interpolatedCircles = new SortedDictionary<int, List<InterpolableCircle>>();
    }
    
    public void CreateInterpolableCircle(int order, Vector3 centerStart, Vector3 centerEnd, float sizeStart, float sizeEnd)
    {
        GameObject gameObject = GameObject.Instantiate(_circlePrefab);
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.transform.position = centerStart;
        sprite.transform.localScale = Vector3.zero;
        gameObject.transform.SetParent(_root.transform);

        InterpolableCircle interpolatedCircle = new InterpolableCircle();
        interpolatedCircle.Sprite = sprite;
        interpolatedCircle.CenterStart = centerStart;
        interpolatedCircle.CenterEnd = centerEnd;
        interpolatedCircle.SizeStart = sizeStart;
        interpolatedCircle.SizeEnd = sizeEnd;

        if (!_interpolatedCircles.ContainsKey(order))
        {
            _interpolatedCircles[order] = new List<InterpolableCircle>();
        }
        _interpolatedCircles[order].Add(interpolatedCircle);
    }
    
    public void CreateInterpolableLine(int order, Vector3 pointStartA, Vector3 pointEndA, Vector3 pointStartB, Vector3 pointEndB)
    {
        GameObject gameObject = GameObject.Instantiate(_linePrefab);
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, pointStartA);
        line.SetPosition(1, pointStartB);
        gameObject.transform.SetParent(_root.transform);

        InterpolableLine interpolatedLine = new InterpolableLine();
        interpolatedLine.PointStartA = pointStartA;
        interpolatedLine.PointEndA = pointEndA;
        interpolatedLine.PointStartB = pointStartB;
        interpolatedLine.PointEndB = pointEndB;
        interpolatedLine.Line = line;
        
        if (!_interpolatedLines.ContainsKey(order))
        {
            _interpolatedLines[order] = new List<InterpolableLine>();
        }
        _interpolatedLines[order].Add(interpolatedLine);
    }

    public void Expand()
    {
        StartCoroutine(ExpandCor(_interpolatedLines));
        StartCoroutine(ExpandCor(_interpolatedCircles));
    }

    public void Clear()
    {
        Clear(_interpolatedLines);
        Clear(_interpolatedCircles);
    }
    
    IEnumerator ExpandCor<T>(SortedDictionary<int, List<T>> shapes) where T : IInterpolableShape
    {
        float duration = 0.2f;

        foreach (KeyValuePair<int, List<T>> pair in shapes)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                foreach (T shape in pair.Value)
                {
                    shape.Update(Mathf.Min(timer, duration) / duration);
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }

    void Clear<T>(SortedDictionary<int, List<T>> shapes) where T : IInterpolableShape
    {
    
        if (shapes != null)
        {
            foreach (KeyValuePair<int, List<T>> pair in shapes)
            {
                foreach (T shape in pair.Value)
                {
                    shape.Destroy();
                }
            }
            shapes.Clear();
        }
    }
}
