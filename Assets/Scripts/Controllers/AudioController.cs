using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public static AudioController audioController;
    public AudioSource[] source;

    public AudioClip bgMusic;

    public AudioClip powerUp;
    public AudioClip boost;
    public AudioClip parry;
    public AudioClip damage;
    public AudioClip explosion;

    public AudioClip click;

    public static float slowedTempo = 0.3f;
    public float tempoLerpDuration = 1f;
    IEnumerator TempoLerper;

    // Start is called before the first frame update
    void Awake()
    {
        audioController = this;
        source = GetComponents<AudioSource>();
        ChangeTempo(1f, false);
    }

    public void StopMusic()
    {
        source[0].Stop();
    }

    public void PlaySound(AudioClip clip, bool loop, int layer)
    {
        if (!loop) source[layer].PlayOneShot(clip);
        else source[layer].clip = clip;

        if(!source[layer].isPlaying) source[layer].Play();
    }

    public void ChangeTempo(float tempo, bool lerpTo)
    {
        if(!lerpTo)source[0].pitch = tempo;
        else
        {
            if (TempoLerper != null) StopCoroutine(TempoLerper);
            TempoLerper = LerpTempo(tempo);
            StartCoroutine(TempoLerper);
        }
    }

    IEnumerator LerpTempo(float endTempo)
    {
        float duration = 0f;
        float startTempo = source[0].pitch;
        while(duration != 1)
        {
            duration = Mathf.Clamp(duration + (Time.unscaledDeltaTime / tempoLerpDuration), 0f, 1f);
            source[0].pitch = startTempo + ((endTempo - startTempo) * duration);
            yield return null;
        }
    }

    public void ButtonClickSound()
    {
        PlaySound(click, false, 1);
    }
}
