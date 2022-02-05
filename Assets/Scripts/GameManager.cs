using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text completeText;
    [SerializeField] private SokobanManager sokobanManager;

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

    public void ResetStage()
    {
        sokobanManager.CreateStage("stage1");
    }

    void Update()
    {
        switch (_state)
        {
            case State.ready:
                completeText.gameObject.SetActive(false);
                ResetStage();
                ChangeState(State.run);
                break;
            case State.run:
                sokobanManager.PlayerMoveOperation();
                if (sokobanManager.IsComplete())
                {
                    ChangeState(State.result);
                }

                break;
            case State.result:
                completeText.gameObject.SetActive(true);
                if (_stateTimer > 2.0f && Input.anyKeyDown)
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