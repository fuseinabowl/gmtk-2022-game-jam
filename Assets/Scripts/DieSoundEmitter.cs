using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieSoundEmitter : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource = null;

    [SerializeField]
    private List<AudioClip> dieHitFloorSounds = new List<AudioClip>();

    [SerializeField]
    private AnimationCurve impulseToSoundChance = AnimationCurve.Linear(0f,0f, 1f,1f);

    [SerializeField]
    private AnimationCurve impulseToMinSoundVolume = AnimationCurve.Linear(0f,0.1f, 1f,1f);
    [SerializeField]
    private AnimationCurve impulseToMaxSoundVolume = AnimationCurve.Linear(0f,0.1f, 1f,1f);

    private void OnCollisionEnter(Collision collisionData)
    {
        var impulse = Mathf.Sqrt(collisionData.impulse.sqrMagnitude);
        var soundChance = impulseToSoundChance.Evaluate(impulse);
        if (Random.value < soundChance)
        {
            PlaySound(impulse);
        }
    }

    private void PlaySound(float impulse)
    {
        var selectedSoundIndex = Random.Range(0, dieHitFloorSounds.Count - 1);
        var selectedSound = dieHitFloorSounds[selectedSoundIndex];

        var minVolume = impulseToMinSoundVolume.Evaluate(impulse);
        var maxVolume = impulseToMaxSoundVolume.Evaluate(impulse);
        var soundVolume = Mathf.Lerp(minVolume, maxVolume, Random.value);

        audioSource.PlayOneShot(selectedSound, soundVolume);
    }
}
