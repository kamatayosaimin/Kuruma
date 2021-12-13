using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(90f, target.eulerAngles.y, 0f);

        Vector3 position = target.position;

        position.y = 100f;

        transform.position = position;
    }
}
