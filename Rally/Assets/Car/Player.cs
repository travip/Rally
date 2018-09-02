using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public static int NUM_DIFF_TURNS = 9;
    public enum ActionType { LeftAcute, LeftSquare, LeftTurn, LeftBend, Straight, RightBend, RightTurn, RightSquare, RightAcute }
    private readonly Queue<ActionType> PlayerActions = new Queue<ActionType>();

    public int MaxInputs { get; private set; }

    public Car playerCar;
    public bool GamePlaying = false;

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
        if (PlayerActions.Count == 0)
            return ActionType.Straight;
        else
        {
            PlayerUI.Instance.DequeueAction();
            return PlayerActions.Dequeue();
        }
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
        PlayerUI.Instance.FadeIn();
    }

    public void BeginCountdown()
    {
        StartCoroutine(PlayerUI.Instance.BeginCountdown());
    }

    public void StartGame()
    {
        playerCar.BeginFollowPath();
        GamePlaying = true;
    }


    public void TEXT_DEQUEUE()
    {
        this.GetNextAction();
    }
}
