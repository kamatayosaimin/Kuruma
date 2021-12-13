using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackMirror : MonoBehaviour
{
    private float mainFogDensity;
    [SerializeField]
    private float fogDensity;

    // Use this for initialization
    void Start()
    {
        Camera cam = GetComponent<Camera>();

        cam.projectionMatrix = GetProjectionMatrix(cam);

        mainFogDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnPreRender()
    {
        GL.invertCulling = true;

        RenderSettings.fogDensity = fogDensity;
    }

    void OnPostRender()
    {
        GL.invertCulling = false;

        RenderSettings.fogDensity = mainFogDensity;
    }

    Matrix4x4 GetProjectionMatrix(Camera cam)
    {
        Vector3 v = new Vector3(-1f, 1f, 1f);

        return cam.projectionMatrix * Matrix4x4.Scale(v);
    }
}
