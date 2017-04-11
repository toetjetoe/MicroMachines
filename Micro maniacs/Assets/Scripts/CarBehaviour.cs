using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum playerCount { player1, player2}

public class CarBehaviour : MonoBehaviour {

    public playerCount player;
    public Transform _camera;
    public Transform cameraPosition;
    private bool cameraTracking = true;

    [Header("Specials"), Range(0, 100)]
    public float nitro;
    public float boostSpeed;
    public Image nitroBar;

    [Header("Car settings")]
    public Transform Respawn;
    public bool controller;

    public float speed;
    private float setSpeed;
    public float rotSpeed;

    [Header("FX")]
    public ParticleSystem deathFX;
    public GameObject boostFX;

    [Header("Audio")]
    public AudioSource _audio;
    public AudioClip deathSound;
    public AudioClip spawnSound;
    public AudioClip boostSound;

    public bool alive = true;

    //Controllbehaviour
    private bool onRoad;
    public bool stop;
    private Rigidbody rb;
    private RaycastHit hitInfo;

    // Use this for initialization
    void Start() {
        SetController();

        rb = GetComponent<Rigidbody>();

        var startPos = new GameObject().transform;
        startPos.position = transform.position;
        startPos.rotation = transform.rotation;
        Respawn = startPos;
    }

    //check settings for controller
    private void SetController()
    {
        controller = (PlayerPrefs.GetInt("Controller") > 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y < 4 && alive)
        {
            StartCoroutine(Death(0));
        }

        //Camera
        Vector3 lookPostion = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);
        if (cameraTracking)
        {
            _camera.position = cameraPosition.position;
            cameraPosition.LookAt(lookPostion);
            _camera.rotation = Quaternion.Lerp(_camera.rotation, cameraPosition.rotation, 0.2f);
        }
        else
        {
            _camera.LookAt(lookPostion);
        }

        //Controlls
        if (!stop)
        {
            if (onRoad)
            {
                setSpeed = speed;
            }
            else
            {
                setSpeed = speed / 10;
            }

            if (!controller)
            {
                if (player == playerCount.player1)
                {
                    if (Input.GetKey(KeyCode.W) && onRoad)
                    {
                        rb.AddForce(transform.forward * Time.deltaTime * setSpeed);
                    }
                    if (Input.GetKey(KeyCode.S) && onRoad)
                    {
                        rb.AddForce(transform.forward * Time.deltaTime * -setSpeed);
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        Boost();
                        if (!_audio.isPlaying)
                        {
                            _audio.clip = boostSound;
                            _audio.Play();
                        }
                    }
                    else
                    {
                        boostFX.SetActive(false);
                    }
                }
                if (player == playerCount.player2)
                {
                    if (Input.GetKey(KeyCode.UpArrow) && onRoad)
                    {
                        rb.AddForce(transform.forward * Time.deltaTime * setSpeed);
                    }
                    if (Input.GetKey(KeyCode.DownArrow) && onRoad)
                    {
                        rb.AddForce(transform.forward * Time.deltaTime * -setSpeed);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
                    }
                    if (Input.GetKey(KeyCode.RightControl))
                    {
                        Boost();
                        if (!_audio.isPlaying)
                        {
                            _audio.clip = boostSound;
                            _audio.Play();
                        }
                    }
                    else
                    {
                        boostFX.SetActive(false);
                    }
                }
            }
            if (controller)
            {
                if (player == playerCount.player1)
                {
                    rb.AddForce(transform.forward * Time.deltaTime * -setSpeed * Input.GetAxis("Triggers1"));
                    transform.Rotate(transform.up * Time.deltaTime * rotSpeed * Input.GetAxis("Horizontal"));
                    if (Input.GetButton("Submit1") && nitro > 0)
                    {
                        Boost();
                        if (!_audio.isPlaying)
                        {
                            _audio.clip = boostSound;
                            _audio.Play();
                        }
                    }
                    else
                    {
                        boostFX.SetActive(false);
                    }
                }
                if (player == playerCount.player2)
                {
                    rb.AddForce(transform.forward * Time.deltaTime * -setSpeed * Input.GetAxis("Triggers2"));
                    transform.Rotate(transform.up * Time.deltaTime * rotSpeed * Input.GetAxis("Horizontal 2"));
                    if (Input.GetButton("Submit2") && nitro > 0)
                    {
                        Boost();
                        if (!_audio.isPlaying)
                        {
                            _audio.clip = boostSound;
                            _audio.Play();
                        }
                    }
                    else
                    {
                        boostFX.SetActive(false);
                    }
                }
            }

            //UI
            if(nitro > 100)
            {
                nitro = 100;
            }
            nitroBar.fillAmount = nitro / 100;
        }
    }


    private void Boost()
    {
        rb.AddForce(transform.forward * Time.deltaTime * boostSpeed);
        nitro = nitro - 1f;
        boostFX.SetActive(true);
    }


    public IEnumerator Death(float bounce)
    {
        alive = false;
        cameraTracking = false;
        rb.AddForce(transform.up * bounce); _audio.clip = deathSound;
        _audio.Play();
        yield return new WaitForSeconds(0.2f);
        deathFX.transform.position = transform.position;
        deathFX.Play();

        yield return new WaitForSeconds(0.7f);
        ResetPosition(Respawn);
    }

    //Respwn to last checkpoint
    public void ResetPosition(Transform spawn)
    {
        _audio.clip = spawnSound;
        _audio.Play();

        rb.velocity = Vector3.zero;
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
        cameraTracking = true;
        alive = true;
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Bump(col.transform);
        }
        if (col.transform.CompareTag("Water") && alive)
        {
            StartCoroutine(Death(3000));
        }
        if (col.transform.CompareTag("Trap") && alive)
        {
            StartCoroutine(Death(0));
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        onRoad = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        onRoad = false;
    }


    private void Bump(Transform collisionTransform)
    {
        float force = rb.velocity.magnitude * 200;
        if (collisionTransform.GetComponent<Rigidbody>().velocity.magnitude * 200 < force)
        {
            collisionTransform.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, 200);
        }
    }
}
