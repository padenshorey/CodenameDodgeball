using UnityEngine;
using System.Collections;

public class CamShakeSimple : MonoBehaviour 
{
    Vector3 originalCameraPosition;

    float shakeAmt = 0;

    public Camera mainCamera;

    public void Awake()
    {
        originalCameraPosition = mainCamera.transform.position;
    }

    public void ShakeCamera(float amount) 
    {
        shakeAmt = amount * .0025f;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("StopShaking", 0.3f);
    }

    void CameraShake()
    {
        if(shakeAmt>0) 
        {
            float quakeAmt = Random.Range(-1f, 1f)*shakeAmt;
            Vector3 pp = mainCamera.transform.position;
            pp.x += quakeAmt;
            pp.y += quakeAmt; // can also add to x and/or z
            mainCamera.transform.position = pp;
        }
    }

    void StopShaking()
    {
        CancelInvoke("CameraShake");
        mainCamera.transform.position = originalCameraPosition;
    }

}