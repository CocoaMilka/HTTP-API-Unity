using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReplicatePrediction : MonoBehaviour
{
    private string apiUrl = "https://api.replicate.com/v1/predictions";
    private string apiToken = "r8_6a9DbRkhmsgg9wFKGo0by8hkQ2v9Pgm3YJixU";
    private string modelVersion = "371aeee1ce0c5efd25bbef7a4527ec9e59188b963ebae1eeb851ddc145685c17";
    private string inputImage = "https://www.carbonfibersupport.com/wp-content/uploads/2021/04/ConcreteCrack.jpg";

    private string predictionId;
    private bool isPredictionComplete;

    //public RawImage inputImageRaw;
    public RawImage resultImage;

    public Texture2D inputTexture; // Assign the input texture in the Inspector

    private void Start()
    {

    }

    private void Update()
    {
        //inputImageRaw.texture = inputTexture;
    }

    public void StartPrediction()
    {
        StartCoroutine(GetPrediction());
        Debug.Log("Starting request...");
    }

    private IEnumerator GetPrediction()
    {
        // Create POST request to create prediction
        string postData = "{\"version\": \"" + modelVersion + "\", \"input\": {\"input_image\": \"" + inputImage + "\"}}";
        UnityWebRequest predictionRequest = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
        predictionRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        predictionRequest.downloadHandler = new DownloadHandlerBuffer();
        predictionRequest.SetRequestHeader("Authorization", "Token " + apiToken);
        predictionRequest.SetRequestHeader("Content-Type", "application/json");

        /*
        byte[] inputImageData = inputTexture.EncodeToPNG();
        UnityWebRequest predictionRequest = new UnityWebRequest(apiUrl, "POST");
        predictionRequest.uploadHandler = new UploadHandlerRaw(inputImageData);
        predictionRequest.downloadHandler = new DownloadHandlerBuffer();
        predictionRequest.SetRequestHeader("Authorization", "Token " + apiToken);
        predictionRequest.SetRequestHeader("Content-Type", "image/png");
        */

        yield return predictionRequest.SendWebRequest();

        if (predictionRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error creating prediction: " + predictionRequest.error);
            yield break;
        }

        string predictionResponse = predictionRequest.downloadHandler.text;
        var predictionJson = JsonUtility.FromJson<PredictionResponse>(predictionResponse);
        predictionId = predictionJson.id;

        // Poll the API until the prediction is complete
        while (!isPredictionComplete)
        {
            // Polling interval
            yield return new WaitForSeconds(5);

            UnityWebRequest predictionStatusRequest = UnityWebRequest.Get(apiUrl + "/" + predictionId);
            predictionStatusRequest.SetRequestHeader("Authorization", "Token " + apiToken);
            predictionStatusRequest.SetRequestHeader("Content-Type", "application/json");

            yield return predictionStatusRequest.SendWebRequest();

            if (predictionStatusRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error checking prediction status: " + predictionStatusRequest.error);
                yield break;
            }

            string statusResponse = predictionStatusRequest.downloadHandler.text;
            var statusJson = JsonUtility.FromJson<PredictionStatusResponse>(statusResponse);

            if (statusJson.status == "succeeded")
            {
                isPredictionComplete = true;
                Debug.Log("Prediction completed. Output: " + statusJson.output);

                // Display result image
                StartCoroutine(DisplayImage(statusJson.output));
            }
            else if (statusJson.status == "failed")
            {
                Debug.LogError("Prediction failed.");
                // Handle the failure as needed.
                break;
            }
        }
    }


    private IEnumerator DisplayImage(string imageUrl)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading image: " + imageRequest.error);
            yield break;
        }

        // Set the downloaded texture as the RawImage's texture
        Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
        resultImage.texture = texture;
    }

    [System.Serializable]
    private class PredictionResponse
    {
        public string id;
    }

    [System.Serializable]
    private class PredictionStatusResponse
    {
        public string status;
        public string output;
    }
}
