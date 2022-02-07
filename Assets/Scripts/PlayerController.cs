using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    public Vector2Int posIndex;
    public bool IsMoving { get; private set; }

    enum State
    {
        ready,
        live,
        die
    };

    private State state;

    void Awake()
    {
        ChangeState(State.ready);
    }

    public void Set(int x, int y)
    {
        posIndex.x = x;
        posIndex.y = y;
    }

    public void ReadyToStart()
    {
        gameObject.SetActive(true);
        ChangeState(State.ready);
    }

    public void Move(Vector2 nextPosition)
    {
        IsMoving = true;
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

        IsMoving = false;
        transform.position = nextPosition;
    }

    void Update()
    {
        switch (state)
        {
            case State.ready:
                ChangeState(State.live);
                break;
            case State.live:
                break;
            case State.die:
                gameObject.SetActive(false);
                break;
        }
    }

    void ChangeState(State next)
    {
        state = next;
    }
}