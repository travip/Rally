using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    private const int MAX_TURN_PRESSES = 3;
    // Player Inputs
    private enum Inputs { Left, Right, Forward }
    private Inputs lastPressed;

    private bool canInput = true;

    private float timeSinceLastPress = 0;
    private float maxTimeBetweenPresses = 0.3f;

    private int seqKeyPresses = 0;
    private bool inputStarted = false;

    // KeyCodes (for customization
    public KeyCode forwardKey = KeyCode.UpArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode confirmKey = KeyCode.Space;

	// Use this for initialization
	void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
	}

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.GamePlaying)
        {
            if (Player.Instance.NumActionsQueued() >= 5)
                return;
            timeSinceLastPress += Time.deltaTime;
            if (timeSinceLastPress > maxTimeBetweenPresses && seqKeyPresses > 0)
                FinalInput();
            if (Input.GetKeyDown(leftKey))
                TurnKeyPressed(Inputs.Left);
            else if (Input.GetKeyDown(rightKey))
                TurnKeyPressed(Inputs.Right);
            else if (Input.GetKeyDown(confirmKey))
                ConfirmKeyPressed();
        }
        else if (PlayerUI.Instance.GameOverScreen)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                Player.Instance.playerCar.RestartGame();
            }
        }
	}

    private void TurnKeyPressed(Inputs input)
    {
        if (lastPressed != input && seqKeyPresses > 0)
            FinalInput();

        seqKeyPresses++;
        lastPressed = input;
        timeSinceLastPress = 0.0f;

        // Already 3 presses, a max 4 turn
        if(seqKeyPresses == MAX_TURN_PRESSES)
            FinalInput();
    }

    private void ConfirmKeyPressed()
    {
        FinalInput();
    }

    private IEnumerator PreventInput()
    {
        canInput = false;
        yield return new WaitForSeconds(0.1f);
        canInput = true;
    }

    private void FinalInput()
    {
        if (lastPressed == Inputs.Right)
        {
            switch (seqKeyPresses)
            {
                case 0:
                    Debug.Log("Something went wrong, 0 key input");
                    return;
                case 1:
                    Player.Instance.PlayerInput(Player.ActionType.RightTurn);
                    break;
                case 2:
                    Player.Instance.PlayerInput(Player.ActionType.RightSquare);
                    break;
                case 3:
                    Player.Instance.PlayerInput(Player.ActionType.RightAcute);
                    PreventInput();
                    break;
            }
        }
        else if (lastPressed == Inputs.Left)
        {
            switch (seqKeyPresses)
            {
                case 0:
                    Debug.Log("Something went wrong, 0 key input");
                    return;
                case 1:
                    Player.Instance.PlayerInput(Player.ActionType.LeftTurn);
                    break;
                case 2:
                    Player.Instance.PlayerInput(Player.ActionType.LeftSquare);
                    break;
                case 3:
                    Player.Instance.PlayerInput(Player.ActionType.LeftAcute);
                    PreventInput();
                    break;
            }
        }
        else
        {
            Debug.Log("? Not left or right or forward inpuit");
        }
        // Clean up input
        inputStarted = false;
        timeSinceLastPress = 0f;
        seqKeyPresses = 0;
    }
}
