using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <Summary>
///ScriptableObject that holds the base stats for an enemy
///

[CreateAssetMenu(fileName ="Enemy Configuration", menuName ="ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    public Enemy Prefab;
    public AttackScriptableObject AttackConfiguration;

    // Enemy Stats
    public int Health = 100;

    // Movement Stats
    public EnemyState DefaultState;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;
    [Range(2, 10)]
    public int Waypoints = 4;
    public float LineOfSightRange = 6f;
    public float FieldOfView = 90f;

    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;
    // -1 means everything
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;

    public void SetupEnemy(Enemy enemy)
    {
        enemy.Agent.acceleration = Acceleration;
        enemy.Agent.angularSpeed = AngularSpeed;
        enemy.Agent.areaMask = AreaMask;
        enemy.Agent.avoidancePriority = AvoidancePriority;
        enemy.Agent.baseOffset = BaseOffset;
        enemy.Agent.height = Height;
        enemy.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.Agent.radius = Radius;
        enemy.Agent.speed = Speed;
        enemy.Agent.stoppingDistance = StoppingDistance;

        enemy.Movement.UpdateRate = AIUpdateInterval;
        enemy.Movement.DefaultState = DefaultState;
        enemy.Movement.IdleMovespeedMultiplier = IdleMovespeedMultiplier;
        enemy.Movement.IdleLocationRadius = IdleLocationRadius;
        enemy.Movement.Waypoints = new Vector3[Waypoints];
        enemy.Movement.LineOfSight.FieldOfView = FieldOfView;
        enemy.Movement.LineOfSight.Collider.radius = LineOfSightRange;
        enemy.Movement.LineOfSight.LineOfSightLayers = AttackConfiguration.LineOfSightLayers;

        enemy.Health = Health;

        AttackConfiguration.SetupEnemy(enemy);
    }
}
