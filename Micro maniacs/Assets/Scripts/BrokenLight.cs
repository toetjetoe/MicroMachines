using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenLight : MonoBehaviour {

    public float interval;
    public Color[] colors;

    public ParticleSystem sparks;
    public Light pointLight;

    private MeshRenderer meshRender;
    private int selectedColor = -1;
    private WaitForSeconds wait;
    private float setIntensity;

	// Use this for initialization
	void Start () {
        meshRender = GetComponent<MeshRenderer>();
        wait = new WaitForSeconds(interval);
        setIntensity = pointLight.intensity;

        StartCoroutine(Blinktime());
        StartCoroutine(SparkTimer());
    }
	
    private IEnumerator Blinktime()
    {
        while (true)
        {
            Blink();
            yield return wait;
        }
    }

    private IEnumerator SparkTimer()
    {
        while (true)
        {
            Spark();
            float stop = Random.Range(2, 5);
            yield return new WaitForSeconds(stop);
        }
    }

    private void Spark()
    {
        sparks.Play();
        pointLight.color = Color.yellow;
        pointLight.intensity = setIntensity * 3;
    }

    private void Blink()
    {
        if(selectedColor < colors.Length-1  )
        {
            selectedColor++;
        }
        else
        {
            selectedColor = 0;
        }
        meshRender.material.color = colors[selectedColor];
        pointLight.color = colors[selectedColor];
        pointLight.intensity = setIntensity;
    }

}
