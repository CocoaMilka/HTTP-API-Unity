using UnityEngine;
using UnityEngine.UI;

public class WebcamDisplay : MonoBehaviour
{
    public RawImage rawImage; // Reference to the RawImage component

    public WebCamTexture webcamTexture; // Reference to the webcam texture

    void Start()
    {
        // Check if there's at least one available webcam
        if (WebCamTexture.devices.Length > 0)
        {
            // Get the first available webcam
            webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name);

            // Start the webcam
            webcamTexture.Play();

            // Assign the webcam texture to the RawImage component
            rawImage.texture = webcamTexture;
        }
        else
        {
            Debug.LogError("No webcam found.");
        }
    }

    void Update()
    {
        // You can add any additional logic or functionality here if needed
    }
}
