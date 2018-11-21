using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;

    public AudioSource clickSound;
    public AudioSource selectSound;

    void Awake()
    {
        instance = this;
    }
    public void SelectSound()
    {
        selectSound.Play();
    }
	public void ClickSound()
    {
        clickSound.Play();
    }
}
