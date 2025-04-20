using UnityEngine;
using UnityEngine.InputSystem;

public class TetrisInputManager : MonoBehaviour
{
    public static TetrisInputManager Instance { get; private set; }

    private TetrisInputActions inputActions;

    public Vector2 MouseMove { get; private set; }
    public bool RotateXPositive { get; private set; }
    public bool RotateXNegative { get; private set; }
    public bool RotateYPositive { get; private set; }
    public bool RotateYNegative { get; private set; }
    public bool RotateZPositive { get; private set; }
    public bool RotateZNegative { get; private set; }
    public bool PlaceBlock { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new TetrisInputActions();

        // Bind input actions
        inputActions.Gameplay.MouseMove.performed += ctx => MouseMove = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.RotateXPositive.performed += ctx => RotateXPositive = true;
        inputActions.Gameplay.RotateXNegative.performed += ctx => RotateXNegative = true;
        inputActions.Gameplay.RotateYPositive.performed += ctx => RotateYPositive = true;
        inputActions.Gameplay.RotateYNegative.performed += ctx => RotateYNegative = true;
        inputActions.Gameplay.RotateZPositive.performed += ctx => RotateZPositive = true;
        inputActions.Gameplay.RotateZNegative.performed += ctx => RotateZNegative = true;
        inputActions.Gameplay.PlaceBlock.performed += ctx => PlaceBlock = true;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void LateUpdate()
    {
        // Reset one-time actions
        RotateXPositive = false;
        RotateXNegative = false;
        RotateYPositive = false;
        RotateYNegative = false;
        RotateZPositive = false;
        RotateZNegative = false;
        PlaceBlock = false;
    }
}