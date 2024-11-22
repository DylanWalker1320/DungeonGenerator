using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    private readonly int attackTriggerId = Animator.StringToHash("Attack");

    [SerializeField] AnimatorOverrideController punchOverrideControler;
    [SerializeField] AnimatorOverrideController kickOverrideController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown((int) MouseButton.Left))
        {
            animator.SetTrigger(attackTriggerId);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.runtimeAnimatorController = punchOverrideControler;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            animator.runtimeAnimatorController = kickOverrideController;
        }
    }

    private void FixedUpdate()
    {
        const float speed = 5.0f;
        Vector2 velocity = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            velocity += Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            velocity += Vector2.right;
        }

        velocity *= speed;
        rb.linearVelocity = velocity;
    }

    public void PickUpWeapon()
    {

    }
}
