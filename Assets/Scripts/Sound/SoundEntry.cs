using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundEntry
{
    public SoundID id;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;

    [Header("Pitch Randomization")]
    public bool randomizePitch = false;
    [Range(0f, 0.5f)] public float pitchVariance = 0.1f;

    public float GetPitch() =>
        randomizePitch ? pitch + UnityEngine.Random.Range(-pitchVariance, pitchVariance) : pitch;
}