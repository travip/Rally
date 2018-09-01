using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public enum ActionType { None, RightBend, RightTurn, RightSquare, RightAcute, LeftBend, LeftTurn, LeftSquare, LeftAcute, Fast, Slow }
    private readonly Queue<ActionType> PlayerActions = new Queue<ActionType>();

    public int MaxInputs { get; private set; }

    public Car playerCar;

    public ActionType[] GetPlayerActions()
    {
        return PlayerActions.ToArray();
    }

    public bool PlayerInput(ActionType action)
    {
        Debug.Log("Player input:" + action);
        PlayerUI.Instance.AddTurnImageToList(action);
        if (PlayerActions.Count < MaxInputs)
        {
            PlayerActions.Enqueue(action);
            playerCar.OnPlayerInput(action);
            return true;
        }
        else
            return false;
    }

    public int NumActionsQueued()
    {
        return PlayerActions.Count;
    }

    public ActionType GetNextAction()
    {
        PlayerUI.Instance.DequeueAction();
        return PlayerActions.Dequeue();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        MaxInputs = 5;
    }

    public void TEXT_DEQUEUE()
    {
        this.GetNextAction();
    }
}
