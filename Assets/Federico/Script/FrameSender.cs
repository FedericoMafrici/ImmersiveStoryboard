using System;
using System.Net.Sockets;
using Trev3d.Quest.ScreenCapture;
using UnityEngine;

public class FrameSender : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    public string serverIP = "192.168.1.100"; //IP DEL PROPRIO PC
    public int serverPort = 5000; // PORTA DEL PROPRIO SERVER 
    public float sendInterval = 0.5f; // INTERVALLO DI TEMPO TRA UN FRAME E L'ALTRO
    private float nextSendTime = 0f;

    private QuestScreenCaptureTextureManager captureManager;

    private bool isConnecting = false;
    private bool isConnected = false;

    private void Start()
    {
        // Recupera l'istanza del QuestScreenCaptureTextureManager
        captureManager = QuestScreenCaptureTextureManager.Instance;

        if (captureManager == null)
        {
            Debug.LogError("QuestScreenCaptureTextureManager instance not found!");
            return;
        }

        // Connetti al server
        ConnectToServer();

        // Iscriviti all'evento OnNewFrame
        captureManager.OnNewFrame.AddListener(OnNewFrame);
    }

    private void OnDestroy()
    {
        // Disconnetti dal server
        DisconnectFromServer();

        if (captureManager != null)
        {
            // Rimuovi l'iscrizione all'evento
            captureManager.OnNewFrame.RemoveListener(OnNewFrame);
        }
    }

    private void ConnectToServer()
    {
        if (isConnecting || isConnected)
            return;

        isConnecting = true;

        try
        {
            client = new TcpClient();
            client.BeginConnect(serverIP, serverPort, new AsyncCallback(OnConnect), client);
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante la connessione al server: " + e.Message);
            isConnecting = false;
        }
    }

    private void OnConnect(IAsyncResult ar)
    {
        try
        {
            client.EndConnect(ar);
            if (client.Connected)
            {
                stream = client.GetStream();
                isConnected = true;
                Debug.Log("Connesso al server con successo.");
            }
            else
            {
                Debug.LogError("Impossibile connettersi al server.");
                isConnected = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante la connessione al server: " + e.Message);
            isConnected = false;
        }
        finally
        {
            isConnecting = false;
        }
    }

    private void DisconnectFromServer()
    {
        isConnected = false;

        if (stream != null)
        {
            stream.Close();
            stream = null;
        }
        if (client != null)
        {
            client.Close();
            client = null;
        }
    }

    private void OnNewFrame()
    {
        // Controlla se è il momento di inviare il prossimo frame
        if (Time.time < nextSendTime)
            return;

        nextSendTime = Time.time + sendInterval;

        if (!isConnected)
        {
            ConnectToServer();
            return;
        }

        // Recupera il frame corrente
        Texture2D texture = captureManager.ScreenCaptureTexture;

        // Invia il frame al server
        SendFrameToServer(texture);
    }

    private void SendFrameToServer(Texture2D texture)
    {
        if (stream == null || !stream.CanWrite)
        {
            isConnected = false;
            Debug.LogWarning("Stream non disponibile, tentativo di riconnessione...");
            ConnectToServer();
            return;
        }

        // Converti la texture in JPG per ridurre la dimensione
        byte[] imageBytes = texture.EncodeToJPG();

        // Invia la lunghezza dell'immagine (4 byte)
        byte[] lengthPrefix = BitConverter.GetBytes(imageBytes.Length);
        try
        {
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            // Invia l'immagine
            stream.Write(imageBytes, 0, imageBytes.Length);
            stream.Flush();
            Debug.Log("Frame inviato al server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante l'invio del frame al server: " + e.Message);
            isConnected = false;
            // Tenta di riconnetterti
            ConnectToServer();
        }
    }
}
