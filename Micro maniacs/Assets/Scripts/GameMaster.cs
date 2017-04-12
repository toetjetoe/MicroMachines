using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class GameMaster : MonoBehaviour {


    private bool gameOver = false;
    private bool gameOn = false;

    [Header("Gamemode Settings"), Tooltip("Max distance between the players to end the game")]
    public float maxDistance;

    public Text timer;
    private float secondsCount;
    private int minuteCount;

    public Transform player1;
    public Transform player2;
    private CarBehaviour car1;
    private CarBehaviour car2;
    private float speed;

    private int currentRound = 1;
    private int score1 = 0;
    private int score2 = 0;


    [Header("UI")]
    public Text currentRoundText;
    public Image[] roundWinner; //image of the round and its winner

    public Text finishText;
    public Transform Sphere;    //Sphere to display on the minimap
    public Text winText1;       
    public Text winText2;

    public Text distanceText;
    public Text distanceMaxText;

    public Text countDown;

    [Header("Audio")]
    public AudioClip[] music;
    private AudioSource _audio;
    public AudioClip finish;

    public AudioSource FXAudio;
    public AudioClip buzz;


    [Header("Checkpoints & respawn")]
    public int player1checks;
    public int player2checks;

    public Transform latestCheckPoint1;
    public Transform latestCheckPoint2;
    private Checkpoint respawnPoint;

    private Vector3 player1Pos;
    private Vector3 player2Pos;
    private float distance;

    //waitforseconds
    private WaitForSeconds wait1;

	// Use this for initialization
	void Start ()
    {

        currentRoundText.text = "Round: " + currentRound;

        car1 = player1.GetComponent<CarBehaviour>();
        car2 = player2.GetComponent<CarBehaviour>();
        _audio = GetComponent<AudioSource>();
        
        speed = car1.speed;

        distanceMaxText.text = "/" + maxDistance.ToString();

        StartCoroutine(CountDown());

        wait1 = new WaitForSeconds(1);
    }

  private void PlayMusic()
    {
        _audio.clip = music[Random.Range(0, music.Length)];
        _audio.Play();
    }
    

    void Update()
    {
        UpdateTimer();

        if (gameOn)
        {
            GetDistance();
            if (distance > maxDistance)
            {
                getFirst();
            }
        }
        if (!_audio.isPlaying && !gameOver)
        {
            PlayMusic();
        }
    }

    public void UpdateTimer()
    {
        secondsCount += Time.deltaTime;
        timer.text = minuteCount + " : " + secondsCount.ToString("F2");
        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
    }

    //Get the distance between player 1 and 2 in a beeline
    private void GetDistance()
    {
        player1Pos = new Vector3(player1.position.x, 10,player1.position.z);
        player2Pos = new Vector3(player2.position.x, 10,player2.position.z);

        distance = Vector3.Distance(player1Pos, player2Pos) * 3;

        Sphere.position = (player1Pos - player2Pos) * 0.5f + player2Pos;
        Sphere.localScale = new Vector3(maxDistance / 3, 1, maxDistance / 3);

        distanceText.text = "Distance: " + distance.ToString("F2");
    }

    //get the player in who is on front
    private void getFirst()
    {
        int check1 = 0;
        int check2 = 0;

        if (latestCheckPoint1)
        {
            check1 = latestCheckPoint1.GetComponent<Checkpoint>().count;
        }

        if (latestCheckPoint2)
        {
            check2 = latestCheckPoint2.GetComponent<Checkpoint>().count;
        }

        if(check1 == 1 && check2 > 13)
        {
            StartCoroutine(EndRound(1));
        }
        else if (check2 == 1 && check1 > 13)
        {
            StartCoroutine(EndRound(2));
        }
        else
        {
            if (check1 > check2)
            {
                StartCoroutine(EndRound(1));
            }
            else
            {
                StartCoroutine(EndRound(2));
            }
        }
    }
    
    //end the round -> check if its the last -> then reset or finish
    private IEnumerator EndRound(int winningPlayer)
    {
        gameOn = false;
        winText1.text = "Player " + winningPlayer + " wins round " + currentRound + "!";
        winText2.text = "Player " + winningPlayer + " wins round " + currentRound + "!";



        if (winningPlayer == 1)
        {
            score1++;
            if(score1 >= 2)
            {
                StartCoroutine(FinishGame(1));
            }
            car2._camera.GetComponent<Grayscale>().enabled = true;

            car1.Respawn = latestCheckPoint1.transform;
            car2.Respawn = latestCheckPoint1.transform;

            roundWinner[currentRound-1].color = Color.blue;
        }
        else
        {
            score2++;
            if (score2 >= 2)
            {
                StartCoroutine(FinishGame(2));
            }
            car1._camera.GetComponent<Grayscale>().enabled = true;

            car1.Respawn = latestCheckPoint2;
            car2.Respawn = latestCheckPoint2;

            roundWinner[currentRound-1].color = Color.red;
        }

        if (!gameOver)
        {
            StartCoroutine(CountDown());
            yield return wait1;

            Reset(winningPlayer);
        }
        else
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        currentRound++;
        currentRoundText.text = "Round: " + currentRound;
    }

    //countdown before the round starts
    private IEnumerator CountDown()
    {

        yield return wait1;
        int count = 3;

        car1.stop = true;
        car2.stop = true;

        while (count > 0)
        {
            countDown.text = count.ToString();
            count--;
            FXAudio.clip = buzz;
            FXAudio.Play();
            yield return wait1;
        }

        car1.stop = false;
        car2.stop = false;

        countDown.text = "";
        gameOn = true;
    }

    //Resets the positions to the checkpoint of the winning player, then starts a new round
    private void Reset(int winningPlayer)
    {
        if(winningPlayer == 1)
        {
            respawnPoint = latestCheckPoint1.GetComponent<Checkpoint>();
        }
        else
        {
            respawnPoint = latestCheckPoint2.GetComponent<Checkpoint>();
        }

        car1.ResetPosition(respawnPoint.spawn1);
        car2.ResetPosition(respawnPoint.spawn2);

        car1._camera.GetComponent<Grayscale>().enabled = false;
        car2._camera.GetComponent<Grayscale>().enabled = false;

        winText1.text = "";
        winText2.text = "";
    }

    //Finish the game -> display winner -> back to menu
    private IEnumerator FinishGame(int winningPlayer)
    {
        gameOver = true;
        winText1.text = "";
        winText2.text = "";
        finishText.text = "Player " + winningPlayer + " has won!!!";

        _audio.clip = finish;
        _audio.Play();

        int count = 6;
        while(count > 0)
        {
            distanceText.text = "Exit to menu in: " + count;
            count--;
            yield return wait1;
        }

        SceneManager.LoadScene(0);
    }
}
