using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float skinWidth = 0.02f; // small buffer to avoid penetrating colliders
    [SerializeField] private LayerMask collisionMask = Physics2D.DefaultRaycastLayers;

    private Rigidbody2D _rb;
    private Collider2D _col;
    private Vector2 _input;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();

        if (_rb == null)
            Debug.LogWarning("PlayerController: Rigidbody2D missing. Add one (Kinematic is fine).");

        if (_col == null)
            Debug.LogWarning("PlayerController: Collider2D missing. Shape-casting won’t work.");
    }

    void Update()
    {
        // Read raw input in Update so input sampling is responsive
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (_col == null || _rb == null)
        {
            // Fallback: simple transform movement if components missing
            transform.Translate(_input.normalized * speed * Time.fixedDeltaTime);
            return;
        }

        // Desired movement for this physics step
        Vector2 desired = _input.normalized * speed * Time.fixedDeltaTime;

        // Move separately on X then Y to allow sliding along obstacles
        Vector2 pos = _rb.position;
        if (!Mathf.Approximately(desired.x, 0f))
        {
            pos = MoveAxis(pos, new Vector2(desired.x, 0f));
        }

        if (!Mathf.Approximately(desired.y, 0f))
        {
            pos = MoveAxis(pos, new Vector2(0f, desired.y));
        }

        _rb.MovePosition(pos);
    }

    private Vector2 MoveAxis(Vector2 startPos, Vector2 delta)
    {
        Vector2 direction = delta.normalized;
        float distance = Mathf.Abs(delta.magnitude);

        // Cast the collider along the movement direction using the collider's shape
        int hitCount = _col.Cast(direction, _hits, distance + skinWidth, true);

        // Filter hits by layer mask and ignore self
        float nearest = float.PositiveInfinity;
        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hits[i];
            if (hit.collider == null || hit.collider.gameObject == gameObject)
                continue;

            if ((collisionMask & (1 << hit.collider.gameObject.layer)) == 0)
                continue;

            if (hit.distance < nearest)
                nearest = hit.distance;
        }

        if (nearest == float.PositiveInfinity)
        {
            // No collisions: move full delta
            return startPos + delta;
        }

        // Move up to the nearest contact minus skinWidth
        float allowed = Mathf.Max(0f, nearest - skinWidth);
        return startPos + direction * allowed;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (_col == null) return;
        Gizmos.color = Color.cyan;
        Bounds b = _col.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
#endif
}