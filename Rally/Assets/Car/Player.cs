﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public enum ActionType { RightBend, RightTurn, RightSquare, RightAcute, LeftBend, LeftTurn, LeftSquare, LeftAcute, Fast, Slow }
    private readonly Queue<ActionType> PlayerActions = new Queue<ActionType>();

    public int MaxInputs { get; private set; }

    public ActionType[] GetPlayerActions()
    {
        return PlayerActions.ToArray();
    }

    public bool PlayerInput(ActionType action)
    {
        Debug.Log("Player input:" + action);
        if (PlayerActions.Count < MaxInputs)
        {
            PlayerActions.Enqueue(action);
            return true;
        }
        else
            return false;
    }

    public ActionType GetNextAction()
    {
        return PlayerActions.Dequeue();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
}