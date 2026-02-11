using UnityEngine;

public class FireSound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    AudioSource ad;

    private void Start()
    {
        ad = GetComponent<AudioSource>();
    }


    public void fireStart()
    {
        Debug.Log("Working");
        if (ad != null)
        {
            ad.Play();
        }
    }
}
