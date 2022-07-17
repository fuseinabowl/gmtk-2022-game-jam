using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource source = null;
    public void TriggerAudio()
    {
        source.Play();
    }
}
