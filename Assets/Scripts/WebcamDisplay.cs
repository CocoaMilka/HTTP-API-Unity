using UnityEngine;
using UnityEngine.UI;

public class WebcamDisplay : MonoBehaviour
{
    public RawImage rawImage;
    public RawImage frame;

    public WebCamTexture webcamTexture; 

    private Texture2D capturedFrame; 

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

            // Create a Texture2D for capturing frames
            capturedFrame = new Texture2D(webcamTexture.width, webcamTexture.height);
        }
        else
        {
            Debug.LogError("No webcam found.");
        }
    }

    public void grabFrame()
    {
        if (!webcamTexture.isPlaying)
        {
            Debug.LogWarning("Webcam is not playing.");
            return;
        }

        // Capture the current frame from the webcam texture
        capturedFrame.SetPixels(webcamTexture.GetPixels());
        capturedFrame.Apply();

        // Assign the captured frame to the frame RawImage component
        frame.texture = capturedFrame;
    }
}
