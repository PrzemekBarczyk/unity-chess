using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioClip _captureSFX;
    [SerializeField] AudioClip _moveSFX;

	AudioSource _audioSource;

	void Start()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayCaptureSFX()
	{
		_audioSource.PlayOneShot(_captureSFX);
	}

    public void PlayMoveSFX()
	{
		_audioSource.PlayOneShot(_moveSFX);
	}
}
