using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class ScreenBoundary : MonoBehaviour
{
    List<Vector2> points = new List<Vector2>();
    public enum AnchorType { Left, Right, Top, Bottom };
    public AnchorType anchor = AnchorType.Left;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (points.Count > 0)
        {
            points.Clear();
        }

        Vector2 bottomLeftCorner = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector2 topRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float xMin = bottomLeftCorner.x;
        float yMin = bottomLeftCorner.y;
        float xMax = topRightCorner.x;
        float yMax = topRightCorner.y;

        switch (anchor)
        {
            case AnchorType.Left:
                points.Add(new Vector2(xMin, yMin));
                points.Add(new Vector2(xMin, yMax));
                break;
            case AnchorType.Right:
                points.Add(new Vector2(xMax, yMin));
                points.Add(new Vector2(xMax, yMax));
                break;
            case AnchorType.Top:
                points.Add(new Vector2(xMin, yMax));
                points.Add(new Vector2(xMax, yMax));
                break;
            case AnchorType.Bottom:
                points.Add(new Vector2(xMin, yMin));
                points.Add(new Vector2(xMax, yMin));
                break;
        }

        if (points.Count > 0)
        {
            GetComponent<EdgeCollider2D>().SetPoints(points);
        }
    }
}
