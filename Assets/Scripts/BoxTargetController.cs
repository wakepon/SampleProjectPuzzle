using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoxTargetController : MonoBehaviour
{
    public Vector2Int posIndex;
    
    public void Set(int x, int y)
    {
        posIndex.x = x;
        posIndex.y = y;
    }
}
