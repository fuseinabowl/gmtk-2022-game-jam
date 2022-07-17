using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExplosionEffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem targetSystem = null;

    public void StartParticleEffect()
    {
        targetSystem.Play();
    }
}
