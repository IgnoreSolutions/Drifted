using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class RainSystem : MonoBehaviour
{
    // fields
    float[] intervalTimes = new float[] { 2.0f, 3.0f, 4.0f, 5.0f, 6.0f}; // interval times before it starts raining again

    public float timeLeft;
    private float nowTime;
    int count;
    int num;

    public ParticleSystem ps;

    public AudioSource rain;

    // Start is called before the first frame update   
    void Start()
    {
        ps.Stop();

        num = Random.Range(0, 5);
        timeLeft = intervalTimes[num];
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            ps.Play();
        }
    }

    void Update()
    {
        if (ps.isStopped)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                num = Random.Range(0, 5);
                timeLeft = intervalTimes[num];

                ps.Play();
            }
        }

        if (!ps.isPlaying)
        {
            rain.Play();
        }
    }
}
