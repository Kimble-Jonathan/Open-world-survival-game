using UnityEngine;

[CreateAssetMenu(fileName = "Attack Configuration", menuName = "ScriptableObject/Attack Configuration")]
public class AttackScriptableObject : ScriptableObject
{

    public int Damage = 5;
    public float AttackRadius = 1.5f;
    public float AttackDelay = 1.5f;

    // Ranged Configs

    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask LineOfSightLayers;

    public void SetupEnemy(Enemy enemy)
    {
        (enemy.AttackRadius.Collider == null ? enemy.AttackRadius.GetComponent<SphereCollider>() : enemy.AttackRadius.Collider).radius = AttackRadius;
        enemy.AttackRadius.AttackDelay = AttackDelay;
        enemy.AttackRadius.Damage = Damage;
    }
}