using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelSpawn : MonoBehaviour
{

    public MeshRenderer fuelItem;
    private WaitForSeconds wait;
    private AudioSource _audio;

    // Use this for initialization
    void Start()
    {
        wait = new WaitForSeconds(5);
        _audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && fuelItem.enabled)
        {
            StartCoroutine(Refeul(other.GetComponent<CarBehaviour>()));
        }
    }

    private IEnumerator Refeul(CarBehaviour player)
    {
        if (player.nitro < 100)
        {
            _audio.Play();
            fuelItem.enabled = false;
            player.nitro = player.nitro + 20;
            yield return wait;
            fuelItem.enabled = true;
        }
    }

}
