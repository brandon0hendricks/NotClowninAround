using UnityEngine;

public class AudioObject : MonoBehaviour
{
    private AudioSource audioSource;

    public void PlayAudioObject(AudioClip clip, float volume, bool loop, float spatial, float minDist, float maxDist)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.spatialBlend = spatial;
        audioSource.minDistance = minDist;
        audioSource.maxDistance = maxDist;

        if (spatial > 0)
        {
            audioSource.spread = 180f;
        }

        audioSource.Play();

        if (!loop)
        {
            Destroy(gameObject, clip.length);
        }
        else if (loop)
        {
            audioSource.loop = loop;
        }
    }

    public void EndAudioObjectEarly()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(gameObject);
        }
    }
}
//john gay
