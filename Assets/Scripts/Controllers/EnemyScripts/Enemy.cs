using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : PoolableObject, IDamageable
{
    public AttackRadius AttackRadius;
    public Animator Animator;
    public EnemyMovement Movement;
    public NavMeshAgent Agent;
    public EnemyScriptableObject enemyScriptableObject;
    public int Health = 100;
    public delegate void DeathEvent(Enemy enemy);
    public DeathEvent OnDie;

    private Coroutine LookCoroutine;
    public const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        AttackRadius.OnAttack += OnAttack;
    }

    private void OnAttack(IDamageable Target)
    {
        Animator.SetTrigger(ATTACK_TRIGGER);
        if(LookCoroutine !=null)
        {
            StopCoroutine(LookCoroutine);
        }
        LookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
    }
    private IEnumerator LookAt(Transform Target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
        float time = 0;

        while (time<1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            
            time+= Time.deltaTime *2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }


    public virtual void OnEnable()
    {
        SetupAgentFromCongig();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Agent.enabled = false;
        OnDie =null;
    }
    public virtual void SetupAgentFromCongig()
    {
        Agent.acceleration = enemyScriptableObject.Acceleration;
        Agent.angularSpeed = enemyScriptableObject.AngularSpeed;
        Agent.areaMask = enemyScriptableObject.AreaMask;
        Agent.avoidancePriority = enemyScriptableObject.AvoidancePriority;
        Agent.baseOffset = enemyScriptableObject.BaseOffset;
        Agent.height = enemyScriptableObject.Height;
        Agent.obstacleAvoidanceType = enemyScriptableObject.ObstacleAvoidanceType;
        Agent.radius = enemyScriptableObject.Radius;
        Agent.speed = enemyScriptableObject.Speed;
        Agent.stoppingDistance = enemyScriptableObject.StoppingDistance;
        
        Movement.UpdateRate = enemyScriptableObject.AIUpdateInterval;

        Health = enemyScriptableObject.Health;

        AttackRadius.Collider.radius = enemyScriptableObject.AttackConfiguration.AttackRadius;
        AttackRadius.AttackDelay = enemyScriptableObject.AttackConfiguration.AttackDelay;
        AttackRadius.Damage=  enemyScriptableObject.AttackConfiguration.Damage;
    }

    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        if(Health<=0)
        {   
            OnDie?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

}
