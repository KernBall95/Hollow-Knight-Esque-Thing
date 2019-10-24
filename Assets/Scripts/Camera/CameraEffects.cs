using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraEffects : MonoBehaviour
{
    public float playerHitShakeIntensity;
    public float playerHitShakeLength;
    public float playerHitAmpGain;

    public float enemyHitShakeIntensity;
    public float enemyHitShakeLength;
    public float enemyHitAmpGain;

    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;

    void Awake()
    {
        vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public IEnumerator CameraShake(float ampGain, float shakeIntensity, float shakeLength)
    {
        Noise(ampGain, shakeIntensity);
        yield return new WaitForSeconds(shakeLength);
        Noise(0, 0);
    }

    private void Noise(float amplitudeGain, float frequencyGain)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }
}
