using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Photon.Pun;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LeastSquares.Overtone;
using UnityEngine.UI;
using System.Text;

public class TextChat : MonoBehaviourPunCallbacks
{
    public static TextChat Instance;
    public TMP_InputField inputField;
    public Button sendButton;
    public bool isSelected = false;
    private GameObject commandInfo;
    private AudioSource audioSource;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        inputField.onSelect.AddListener(input => isSelected = true);
        inputField.onDeselect.AddListener(input => isSelected = false);

        sendButton.onClick.AddListener(() =>
        {
            if (inputField.text != "")
            {
                if (inputField.text.StartsWith("/ai "))
                {
                    string message = inputField.text.TrimStart("/ai ".ToCharArray());
                    photonView.RPC("ChatWithAIRpc", RpcTarget.MasterClient, message);
                }
                photonView.RPC("SendMessageRpc", RpcTarget.AllBuffered, PhotonNetwork.NickName, inputField.text, true);
                inputField.text = "";
                isSelected = false;
                EventSystem.current.SetSelectedGameObject(null);
                commandInfo.SetActive(true);
            }
        });

        commandInfo = GameObject.Find("CommandInfo");
        audioSource = GetComponent<AudioSource>();
    }

    public void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return) && !isSelected)
        {
            isSelected = true;
            // Set the selected GameObject to the input field
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.caretPosition = inputField.text.Length;
            commandInfo.SetActive(false);
        }

        else if (Input.GetKeyUp(KeyCode.Escape) && isSelected)
        {
            isSelected = false;
            // Reset the selected GameObject 
            EventSystem.current.SetSelectedGameObject(null);
            commandInfo.SetActive(true);
        }

        // else if (Input.GetKeyUp(KeyCode.Return) && inputField.text != "")
        // {
        //     TTSPlayer.Instance.Speak(inputField.text);
        //     photonView.RPC("SendMessageRpc", RpcTarget.AllBuffered, PhotonNetwork.NickName, inputField.text, true);
        //     inputField.text = "";
        //     isSelected = false;
        //     EventSystem.current.SetSelectedGameObject(null);
        //     commandInfo.SetActive(true);
        // }
    }

    [PunRPC]
    public void SendMessageRpc(string sender, string msg, bool notify = false)
    {
        string message = $"<color=\"yellow\">{sender}</color>: {msg}";
        Logger.Instance.LogInfo(message);
        LogManager.Instance.LogInfo($"{sender} wrote in the chat: \"{msg}\"");

        if (notify)
        {
            audioSource.Play();
        }
    }
    private IEnumerator ChatWithAI(string question)
    {
        string postData = JsonConvert.SerializeObject(
            new
            {
                model = "gemma:2b-instruct",
                prompt = question,
                stream = false
            }
        );
        UnityWebRequest request = UnityWebRequest.Post("http://localhost:11434/api/generate", postData, "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string text = Encoding.UTF8.GetString(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
            print(text);
            string response = JsonConvert.DeserializeObject<JToken>(text.Replace("\n", ""))["response"].ToString();
            print(response);
            photonView.RPC("AISpeakRpc", RpcTarget.All, response);
        }
    }
    [PunRPC]
    public void ChatWithAIRpc(string question)
    {
        StartCoroutine(ChatWithAI(question));
    }
    [PunRPC]
    public async void AISpeakRpc(string content)
    {
        await TTSPlayer.Instance.Speak(content);
    }
}