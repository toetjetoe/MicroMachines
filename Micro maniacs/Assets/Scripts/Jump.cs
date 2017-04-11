using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {

    public float strength;
    public ParticleSystem boostFX;

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Boost(col.transform.GetComponent<Rigidbody>());
        }
    }

    private void Boost(Rigidbody player)
    {
        player.AddForce(transform.forward * (strength - player.velocity.magnitude )* Time.deltaTime * 50);
        boostFX.Play();
    }
}
