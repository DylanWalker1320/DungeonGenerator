using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    private readonly int attackTriggerId = Animator.StringToHash("Attack");
    private readonly int specialAttackTriggerId = Animator.StringToHash("SpecialAttack");

    [SerializeField] AnimatorOverrideController punchOverrideControler;
    [SerializeField] AnimatorOverrideController kickOverrideController;

    private WeaponInfo weaponInfo = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown((int) MouseButton.Left))
        {
            animator.SetTrigger(attackTriggerId);
        }
        else if (Input.GetMouseButtonDown((int) MouseButton.Right))
        {
            animator.SetTrigger(specialAttackTriggerId);
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

    public void PickUpWeapon(WeaponInfo weaponInfo)
    {
        this.weaponInfo = weaponInfo;
        animator.runtimeAnimatorController = weaponInfo.attackOverrideController;
        Debug.Log($"Player picked up {weaponInfo.weaponName}!");
    }
}
