using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;

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

    public void ReadyToStart()
    {
        gameObject.SetActive(true);
        ChangeState(State.ready);
    }

    void MoveOperation()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += moveSpeed * Vector3.left * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += moveSpeed * Vector3.right * Time.deltaTime;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.ready:
                ChangeState(State.live);
                break;
            case State.live:
                MoveOperation();
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