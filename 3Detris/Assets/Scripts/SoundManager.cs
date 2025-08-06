using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public enum SoundType
{
    Move,
    Rotate,
    CantRotate,
    Place,
    Hold,
    CantHold,
    ClearLine,
    ClickButton,
    HoverButton,
    GameStart,
    GameOver,
    ColorChange,
    Explode
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [SerializeField] private SoundList[] sounds;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SoundManager.PlayMusic();
    }

    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        instance.audioSource.PlayOneShot(instance.sounds[(int)sound].SoundClip, volume);
    }
    public static void PlayMusic()
    {
        instance.musicSource.Play();
    }
    public static void StopMusic()
    {
        instance.musicSource.Stop();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref sounds, names.Length);
        for (int i = 0; i < sounds.Length; i++)
            sounds[i].name = names[i];
    }
#endif
}

[Serializable] 
public struct SoundList
{
    readonly public AudioClip SoundClip { get => soundClip; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip soundClip;
}