using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _audioSource;

    [System.Serializable]
    public class AudioClipEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0,1)]
        public float volume;
    }

    [SerializeField] private List<AudioClipEntry> clips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    public void PlaySoundFXClip(string clipName, Transform spawnTransform)
    {
        AudioSource audioDrop = Instantiate(_audioSource, spawnTransform.position, Quaternion.identity);
        var clip = clips.FirstOrDefault(clip => clip.key == clipName);
        audioDrop.clip = clip.clip;
        audioDrop.volume = clip.volume;
        audioDrop.Play();
        var clipLength = audioDrop.clip.length;
        Destroy(audioDrop.gameObject, clipLength);
    }
}
