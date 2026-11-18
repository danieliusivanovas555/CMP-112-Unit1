//Dan Ivanovas

using UnityEngine;


public class TowerSounds : MonoBehaviour
{
    //This is really simple code just function call different audio files
    //Could potentially be more effiecient by making it universal but no time
    public AudioClip placementSound;
    public AudioClip selectionSound;
    public AudioClip hoverSound;
    public AudioClip elementClickSound;
    public AudioClip upgradeSound;
    public AudioClip sellSound;

    [Range(0f, 1f)]
    public float volume = 0.75f;

    public float minPitch = 0.92f;
    public float maxPitch = 1.08f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPlacement()
    {
        PlaySound(placementSound);
    }

    public void PlaySelection()
    {
        PlaySound(selectionSound);
    }

    public void PlayHover()
    {
        PlaySound(hoverSound);
    }
    
    public void PlaySell()
    {
        PlaySound(sellSound);
    }
    public void PlayElementClick()
    {
        PlaySound(elementClickSound);
    }

    public void PlayUpgrade()
    {
        PlaySound(upgradeSound);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip, volume);
    }
}
