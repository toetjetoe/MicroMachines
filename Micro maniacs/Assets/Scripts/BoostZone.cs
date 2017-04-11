using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : MonoBehaviour {

    public float strenght;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Boost(other.transform.GetComponent<Rigidbody>());
        }
    }

    private void Boost(Rigidbody player)
    {
        player.AddForce(transform.up * strenght);
    }

}
