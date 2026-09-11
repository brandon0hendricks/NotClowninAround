using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.Audio;


public enum SoundType
{
    //SFX Names
    HammerAttack,
    EnemyAttack,
    Placeholder,
    PlayerLaugh,
    BalloonPop,
    Cannon,
    BossAttack,
    BossDeath,
    BossHit,
    BossLaugh,
    BossTaunt,
    Dash,
    EnemyDeath,
    EnemyDeath2,
    PlayerDeath,
    PeanutHit,
    WhipHit,
    EnemyHit,
    PlayerHit,
    PlayerJump,
    PlayerLandGrass,
    PlayerLandTarp,
    PeanutToss,
    StepsGrass,
    StepsTarp,
    WhipCrack,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private soundList[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;
    [SerializeField] private AudioObject audioObjectPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public static AudioObject PlayAudioObject(SoundType sound, Vector2 position,  float volume = 1f, bool loop = false, float spatial = 0f, float minDist = 0.5f, float maxDist = 3f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        AudioObject audioObject = Instantiate(instance.audioObjectPrefab, position, Quaternion.identity);
        audioObject.PlayAudioObject(randomClip, volume, loop, spatial, minDist, maxDist);

        return audioObject;
    }

    public static void KillSoundEarly(AudioObject audioObject)
    {
        if (audioObject != null)
        {
            audioObject.EndAudioObjectEarly();
        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif

}

[Serializable]
public struct soundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
