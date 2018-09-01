using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    private const int MAX_PRESSES = 4;

    // Player Inputs
    private enum Inputs { Left, Right }
    private Inputs lastPressed;

    private float timeSinceLastPress = 0;
    private float maxTimeBetweenPresses = 0.3f;

    private int seqKeyPresses = 0;
    private bool inputStarted = false;

    // KeyCodes (for customization
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode confirmKey;

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
        timeSinceLastPress += Time.deltaTime;
        if (timeSinceLastPress > maxTimeBetweenPresses && seqKeyPresses > 0)
            FinalInput();
        if (Input.GetKeyDown(leftKey))
            InputKeyPressed(Inputs.Left);
        else if (Input.GetKeyDown(rightKey))
            InputKeyPressed(Inputs.Right);
        else if (Input.GetKeyDown(confirmKey))
            ConfirmKeyPressed();
	}

    private void InputKeyPressed(PlayerInput.Inputs input)
    {
        seqKeyPresses++;
        lastPressed = input;
        timeSinceLastPress = 0.0f;

        // Already 3 presses, a max 4 turn
        if(seqKeyPresses == MAX_PRESSES)
            FinalInput();
    }

    private void ConfirmKeyPressed()
    {
        FinalInput();
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
                    Player.Instance.PlayerInput(Player.ActionType.RightBend);
                    break;
                case 2:
                    Player.Instance.PlayerInput(Player.ActionType.RightTurn);
                    break;
                case 3:
                    Player.Instance.PlayerInput(Player.ActionType.RightSquare);
                    break;
                case 4:
                    Player.Instance.PlayerInput(Player.ActionType.RightAcute);
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
                    Player.Instance.PlayerInput(Player.ActionType.LeftBend);
                    break;
                case 2:
                    Player.Instance.PlayerInput(Player.ActionType.LeftTurn);
                    break;
                case 3:
                    Player.Instance.PlayerInput(Player.ActionType.LeftSquare);
                    break;
                case 4:
                    Player.Instance.PlayerInput(Player.ActionType.LeftAcute);
                    break;
            }
        }
        else
        {
            Debug.Log("? Not left or right input");
        }
        // Clean up input
        inputStarted = false;
        timeSinceLastPress = 0f;
        seqKeyPresses = 0;
    }
}
