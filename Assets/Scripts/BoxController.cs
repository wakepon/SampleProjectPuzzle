using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoxController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    public Vector2Int posIndex;
    
    public void Set(int x, int y)
    {
        posIndex.x = x;
        posIndex.y = y;
    }
    
    public void Move(Vector2 nextPosition)
    {
        StartCoroutine(MoveAnimation(nextPosition));
    }

    IEnumerator MoveAnimation(Vector2 nextPosition)
    {
        Vector3 diff = (Vector3) nextPosition - transform.position;
        Vector3 dir = diff;
        dir.Normalize();
        var delta = moveSpeed * Time.deltaTime;
        while (diff.sqrMagnitude > delta * delta)
        {
            yield return null;
            diff = (Vector3) nextPosition - transform.position;
            delta = moveSpeed * Time.deltaTime;
            transform.position += dir * delta;
        }

        transform.position = nextPosition;
    }
}
