using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text completeText;
    [SerializeField] private PlayerController playerController;

    enum State
    {
        ready,
        run,
        result
    }

    private State _state = State.ready;
    private float _stateTimer = 0.0f;

    private void Start()
    {
        ChangeState(State.ready);
    }

    void Update()
    {
        switch (_state)
        {
            case State.ready:
                completeText.gameObject.SetActive(false);
                playerController.ReadyToStart();
                ChangeState(State.run);
                break;
            case State.run:
                break;
            case State.result:
                completeText.gameObject.SetActive(true);
                if (_stateTimer > 1.0f && Input.anyKeyDown)
                {
                    ChangeState(State.ready);
                }
                break;
        }

        _stateTimer += Time.deltaTime;
    }

    void ChangeState(State nextState)
    {
        _state = nextState;
        _stateTimer = 0.0f;
    }
}