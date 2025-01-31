using System;
using System.IO;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

[System.Serializable]
public class RequestData
{
    public string subject;
    public int personality;
    public int intelligence;
    public int interest;
    public int happyness;
    public string audio;

    public RequestData(string subject, int personality, int intelligence, int interest, int happyness, string audio)
    {
        this.subject = subject;
        this.personality = personality;
        this.intelligence = intelligence;
        this.interest = interest;
        this.happyness = happyness;
        this.audio = audio;
    }
}

public class QuestionDispatcher : MonoBehaviour
{
    private StudentHandler studentHandler;
    private bool isTextOnly = false;

    void Start()
    {
        studentHandler = GameObject.Find("StudentHandler").GetComponent<StudentHandler>();
    }

    public void AddQuestionRequest(AudioClip audioClip, SmartStudentController studentController, Personality personality, Intelligence intelligence, Interest interest, Happyness happyness)
    {
        StartCoroutine(SendAudioToServer(audioClip, studentController, personality, intelligence, interest, happyness));
    }

    private IEnumerator SendAudioToServer(AudioClip audioClip, SmartStudentController studentController, Personality personality, Intelligence intelligence, Interest interest, Happyness happyness)
    {
        byte[] audioData = ConvertAudioClipToWav(audioClip);

        RequestData requestData = new RequestData(
            SessionManager.Instance.Subject,
            (int)personality,
            (int)intelligence,
            (int)interest,
            (int)happyness,
            Convert.ToBase64String(audioData)
        );

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("Dati JSON inviati: " + jsonData);

        var www = new UnityWebRequest("http://127.0.0.1:5000/generate_audio_response", "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Errore nella richiesta al server: {www.error}");
            yield break;
        }

        string jsonResponse = www.downloadHandler.text;
        Debug.Log("Risposta JSON ricevuta (task ID): " + jsonResponse);

        var taskResponse = JsonUtility.FromJson<TaskResponse>(jsonResponse);
        string taskId = taskResponse.task_id;

        bool isCompleted = false;
        string audioBase64 = null;
        string answerText = "prova";

        while (!isCompleted)
        {
            var statusRequest = UnityWebRequest.Get($"http://127.0.0.1:5000/result/{taskId}");
            yield return statusRequest.SendWebRequest();

            if (statusRequest.result == UnityWebRequest.Result.Success)
            {
                var statusResponse = JsonUtility.FromJson<TaskStatusResponse>(statusRequest.downloadHandler.text);

                if (statusResponse.status == "completed")
                {
                    audioBase64 = statusResponse.audio;
                    //answerText = statusResponse.text;
                    isCompleted = true;
                }
                else
                {
                    Debug.Log("Task ancora in corso, attendo...");
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                Debug.LogError($"Errore nella richiesta di stato del task: {statusRequest.error}");
                yield break;
            }
        }

        if (!string.IsNullOrEmpty(audioBase64))
        {
            Debug.Log("Audio Base64 ricevuto: " + audioBase64);
            AudioClip responseAudioClip = ConvertBase64ToAudioClip(audioBase64);
            if (responseAudioClip != null)
            {
                studentHandler.AddAudioToQueue(responseAudioClip, studentController);
                studentHandler.AddTextToQueue(answerText, studentController);
            }
            else
            {
                Debug.LogError("Errore nella conversione dell'audio in AudioClip.");
            }
        }
        else
        {
            Debug.LogError("Risposta audio non valida o mancante.");
        }
    }

    private AudioClip ConvertBase64ToAudioClip(string audioBase64)
    {
        if (string.IsNullOrEmpty(audioBase64))
        {
            Debug.LogError("Base64 audio data is null or empty.");
            return null;
        }

        try
        {
            byte[] audioData = Convert.FromBase64String(audioBase64);
            Debug.Log("Lunghezza dei byte audio ricevuti: " + audioData.Length);

            // Create a temporary file to store the audio
            string tempFilePath = Path.Combine(Application.persistentDataPath, "tempAudio.wav");
            File.WriteAllBytes(tempFilePath, audioData);

            // Load the audio file as an AudioClip
            WWW www = new WWW("file://" + tempFilePath);
            while (!www.isDone) { }

            AudioClip audioClip = www.GetAudioClip(false, false);
            return audioClip;
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore nella conversione dell'audio Base64 in AudioClip: " + ex.Message);
            return null;
        }
    }

    // Funzione per convertire l�audio in formato WAV
    private byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;

            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + samples * channels * 2);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(hz);
            writer.Write(hz * channels * 2);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(samples * channels * 2);

            float[] data = new float[samples * channels];
            clip.GetData(data, 0);

            foreach (float s in data)
            {
                short value = (short)(s * short.MaxValue);
                writer.Write(value);
            }

            return stream.ToArray();
        }
    }

    public void SetIsTextOnly(bool value)
    {
        this.isTextOnly = value;
    }

    [Serializable]
    private class TaskResponse
    {
        public string task_id;
    }

    [Serializable]
    private class TaskStatusResponse
    {
        public string status;
        public string audio;
        //public string text;
    }
}
