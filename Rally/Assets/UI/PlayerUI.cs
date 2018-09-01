﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    // Turning UI
    [SerializeField]
    private Transform playerTurnInputList;
    [SerializeField]
    private TurnImage turnImage;
    [SerializeField]
    private Sprite rightBend, rightTurn, rightSquare, rightAcute;
    [SerializeField]
    private Sprite leftBend, leftTurn, leftSquare, leftAcute;
    [SerializeField]
    private Sprite fast, slow;

    private readonly Queue<TurnImage> turnImageQueue = new Queue<TurnImage>();

    // Timer
    private float time;
    public TextMeshProUGUI timerText;

	// Use this for initialization
	void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        UpdateTimer();

    }

    private void UpdateTimer()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timerText.text = string.Format("{0:D2}:{1:D2}.{2:D1}", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt(timeSpan.Milliseconds/100));
    }

    public void DequeueAction()
    {
        turnImageQueue.Dequeue().RemoveThisItem();
    }

    public void AddTurnImageToList(Player.ActionType actionType)
    {
        if(turnImageQueue.Count == 5)
        {
            Destroy(turnImageQueue.Dequeue().gameObject);
        }
        TurnImage newTurn = Instantiate(turnImage, playerTurnInputList);
        newTurn.transform.SetAsFirstSibling();
        switch(actionType)
        {
            case Player.ActionType.Fast:
                newTurn.image.sprite = fast;
                break;
            case Player.ActionType.Slow:
                newTurn.image.sprite = slow;
                break;
            case Player.ActionType.RightBend:
                newTurn.image.sprite = rightBend;
                break;
            case Player.ActionType.RightTurn:
                newTurn.image.sprite = rightTurn;
                break;
            case Player.ActionType.RightSquare:
                newTurn.image.sprite = rightSquare;
                break;
            case Player.ActionType.RightAcute:
                newTurn.image.sprite = rightAcute;
                break;
            case Player.ActionType.LeftBend:
                newTurn.image.sprite = leftBend;
                break;
            case Player.ActionType.LeftTurn:
                newTurn.image.sprite = leftTurn;
                break;
            case Player.ActionType.LeftSquare:
                newTurn.image.sprite = leftSquare;
                break;
            case Player.ActionType.LeftAcute:
                newTurn.image.sprite = leftAcute;
                break;
        }
        turnImageQueue.Enqueue(newTurn);
    }
}