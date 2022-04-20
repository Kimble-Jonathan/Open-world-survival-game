using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(SphereCollider))]
public class PlayerAttack : MonoBehaviour
{
    public SphereCollider Collider;
    private List<IDamageable> Damageables = new List<IDamageable>();
    public int Damage = 10;
    public float AttackDelay = 0.5f;
    public delegate void AttackEvent(IDamageable Target);
    public AttackEvent OnAttack;
    private Coroutine AttackCoroutine;
    PlayerInput playerInput;
    public bool isAttackPressed;
    public Animator animator;
    public bool AttackPressedChecker;





    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        playerInput = new PlayerInput();;
    }
    public void getAttack(InputAction.CallbackContext ctx)
    {
        isAttackPressed = ctx.ReadValueAsButton();
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {   
        Debug.Log(other);
    
        if(isAttackPressed ==  true)
        {
            Debug.Log("ATTACK");
            IDamageable damageable = GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damageables.Add(damageable);

                if (AttackCoroutine == null)
                {
                    AttackCoroutine = StartCoroutine(Attack());
                }
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);
        Debug.Log("ATTACK");
        animator.SetBool("Attack", true);

        yield return Wait;



    }

}
