using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFire : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;

        main.startColor = GetColor();
    }

    // Update is called once per frame
    void Update()
    {
    }

    Color GetColor()
    {
        float h = Random.value;

        return Color.HSVToRGB(Random.value, 1f, 1f, false);
    }
}
