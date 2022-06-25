using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rb;
    public int thrownByPlayer;
    public Vector3 initialForce;
    public float explosionRadius = 5f;
    public float explosionDamage = 200f;

    void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        ServerSend.ProjectileSpawned(this, thrownByPlayer);

        rb.AddForce(initialForce, ForceMode.Impulse);
        StartCoroutine(ExplodeAfterTime());
    }

    void FixedUpdate(){
        ServerSend.ProjectilePosition(this);
    }

    public void Initialize(Vector3 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer){
        initialForce = _initialMovementDirection * _initialForceStrength;
        thrownByPlayer = _thrownByPlayer;
    }

    void Explode(){
        ServerSend.ProjectileExploded(this);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider _collider in _colliders){
            if(_collider.CompareTag("Player")){
                _collider.gameObject.GetComponent<Player>().TakeDamage(Server.clients[thrownByPlayer].player, explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    IEnumerator ExplodeAfterTime(){
        yield return new WaitForSeconds(3f);
        Explode();
    }
}
