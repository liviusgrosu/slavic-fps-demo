using UnityEngine;


public class PlayerWalkingSound : MonoBehaviour
{
    [Tooltip("Should be set to 'Environment'")]
    public LayerMask _environmentMask;
    public static PlayerWalkingSound Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    public void TriggerWalkSound()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 0.5f, _environmentMask))
        {
            var tagName = hit.collider.tag;
            var soundName = $"{tagName} Footstep {Random.Range(1, 7)}";
            SoundManager.Instance.PlaySoundFXClip(soundName, transform);
        }
    }

    public void TriggerLandingSound()
    {
        TriggerWalkSound();
        TriggerWalkSound();
    }
}
