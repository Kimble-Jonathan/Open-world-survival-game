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
    bool isAttackPressed;
    public Animator animator;





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
        while(isAttackPressed)
        {
            StartCoroutine(Attack());
        }StopCoroutine(Attack());
        animator.SetBool("Attack",false);
    }

    private IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);

        animator.SetBool("Attack", true);

        yield return Wait;



    }

}
