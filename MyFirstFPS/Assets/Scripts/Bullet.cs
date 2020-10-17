using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float lifetime;
    private float shootTime;

    public GameObject hitParticle;

    void OnEnable()
    {
        shootTime = Time.time;    
    }

    void Update()
    {
        // disable the bullet after 'lifetime' seconds
        if (Time.time - shootTime >= lifetime)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // did we hit the player?
        if (other.CompareTag("Player"))
            other.GetComponent<Player>().TakeDamage(damage);
        else if (other.CompareTag("Enemy"))
            other.GetComponent<Enemy>().TakeDamage(damage);

        // create the hit particle
        GameObject obj = Instantiate(hitParticle, transform.position, Quaternion.identity);
        Destroy(obj, 0.5f);

        // disable the bullet
        gameObject.SetActive(false);
    }
}
