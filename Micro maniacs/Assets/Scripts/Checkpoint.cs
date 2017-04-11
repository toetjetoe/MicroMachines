using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int count;

    public GameMaster gameMaster;

    public ParticleSystem particles;

    private AudioSource _audio;

    public Transform spawn1;
    public Transform spawn2;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            CheckPlayer(other);
        }
    }

    private void CheckPlayer(Collider player)
    {
        _audio.Play();
        particles.Play();
        if (player.transform.name == "Player1")
        {
            gameMaster.latestCheckPoint1 = transform;
            gameMaster.player1checks++;
            player.GetComponent<CarBehaviour>().Respawn = spawn1;
        }
        if (player.transform.name == "Player2")
        {
            gameMaster.latestCheckPoint2 = transform;
            gameMaster.player2checks++;
            player.GetComponent<CarBehaviour>().Respawn = spawn2;
        }
    }
}