using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    UnityEvent<AttackInfo> eventAttack = null;

    private Animator animator;
    private Rigidbody2D rb;

    private readonly int attackTriggerId = Animator.StringToHash("Attack");
    private readonly int specialAttackTriggerId = Animator.StringToHash("SpecialAttack");

    [SerializeField] AnimatorOverrideController punchOverrideControler;
    [SerializeField] AnimatorOverrideController kickOverrideController;

    private Weapon weapon = null;  // we should have a no weapon weapon

    float speed = 2.0f;

    int baseDmg = 3;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        eventAttack = new();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            animator.SetTrigger(attackTriggerId);
            AttackInfo atkInfo = new(baseDmg);
            eventAttack.Invoke(atkInfo);
            Debug.Log($"Player did {atkInfo.TotalDamage()} damage!");
        }
        else if (Input.GetMouseButtonDown((int)MouseButton.Right))
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

    public void PickUpWeapon(Weapon weapon)
    {
        if (this.weapon is not null)
        {
            foreach (WeaponComponent component in this.weapon.Components)
            {
                component.Unapply();
            }
        }

        this.weapon = weapon;
        foreach (WeaponComponent component in weapon.Components)
        {
            component.Apply();
        }
        animator.runtimeAnimatorController = weapon.WeaponInfo.attackOverrideController;
        Debug.Log($"Player picked up {weapon.WeaponInfo.weaponName}!");
    }

    public void IncreaseSpeed(float speedChange)
    {
        speed += speedChange;
    }

    public void SubscribeToAttack(UnityAction<AttackInfo> action)
    {
        eventAttack.AddListener(action);
    }

    public void UnsubscribeToAttack(UnityAction<AttackInfo> action)
    {
        eventAttack.RemoveListener(action);
    }
}
