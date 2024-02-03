using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineCam;
    private float shakeIntensity=5f;
    private float shakeTime=0.2f;
    private float timer;
    private CinemachineBasicMultiChannelPerlin _cbmp;
    // Start is called before the first frame update
    void Start()
    {
        cinemachineCam = GetComponent<CinemachineVirtualCamera>();
        StopShake();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            ShakeCamera();
        }
        if (timer > 0) { 
            timer -= Time.deltaTime;
            if (timer < 0) {
                StopShake();
            }
        }
    }
    void ShakeCamera() {
        CinemachineBasicMultiChannelPerlin _cbmp = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmp.m_AmplitudeGain = shakeIntensity;
        timer = shakeTime;
    }
    void StopShake() {
        CinemachineBasicMultiChannelPerlin _cbmp = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmp.m_AmplitudeGain = 0f;
        timer = 0;
    }
}
