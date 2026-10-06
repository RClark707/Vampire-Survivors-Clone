using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{
    protected SpriteRenderer sr;
    [Header("Sorting Controls")]
    public bool sortingActive = true;
    public float minimumDistance = 0.2f;
    int lastSortOrder = 0;

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void LateUpdate()
    {
        int newSortOrder = (int)(-transform.position.y / minimumDistance);
        if (lastSortOrder != newSortOrder) sr.sortingOrder = newSortOrder;
    }
}
