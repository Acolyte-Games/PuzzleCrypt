using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Rigidbody2D rb;

    private Vector2 _movement;
    private InputAction _moveAction;
    private Animator animator;

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("TopDownMovement: Rigidbody2D is required but was not found.");
                enabled = false;
                return;
            }
        }

        // Create an InputAction for movement (keyboard WASD/arrow keys + gamepad left stick)
        var map = new InputActionMap("Player");
        _moveAction = map.AddAction("Move", InputActionType.Value);

        // Gamepad stick
        _moveAction.AddBinding("<Gamepad>/leftStick");

        // Composite binding for keyboard (WASD / Arrow keys)
        _moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        _moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        _moveAction.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _moveAction.canceled += ctx => _movement = Vector2.zero;

        _moveAction.Enable();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("TopDownMovement: Animator component not found. Animation will not work.");
        }
    }

    void OnEnable()
    {
        _moveAction?.Enable();
    }

    void OnDisable()
    {
        _moveAction?.Disable();
    }

    void OnDestroy()
    {
        if (_moveAction != null)
        {
            _moveAction.performed -= ctx => _movement = ctx.ReadValue<Vector2>();
            _moveAction.canceled -= ctx => _movement = Vector2.zero;
            _moveAction.Dispose();
            _moveAction = null;
        }
    }

    void FixedUpdate()
    {
        // Keep diagonal speed consistent (matches previous behaviour)
        Vector2 move = _movement;
        if (move.sqrMagnitude > 1f)
            move = move.normalized;

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}
