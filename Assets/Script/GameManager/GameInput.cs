using UnityEngine;

public class GameInput : MonoBehaviour
{
    private static GameInput instance;
    private InputPlayer playerInputAction;
    public Player player;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInputAction = new InputPlayer();

        playerInputAction.Player.Enable();
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
        /*

        if (inputVector.x == 1 || inputVector.x == -1 || inputVector.y == 1 || inputVector.y == -1)
        {
            if (Player.instance.canMove)
            {
                player.Aim.SetFloat("LastMoveX", inputVector.x);
                player.Aim.SetFloat("LastMoveY", inputVector.y);
            }

        }
        */
        inputVector = inputVector.normalized;

        return inputVector;
    }
}
