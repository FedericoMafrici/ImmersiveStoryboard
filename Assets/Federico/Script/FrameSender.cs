using System;
using System.Net.Sockets;
using Trev3d.Quest.ScreenCapture;
using UnityEngine;

public class FrameSender : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    public string serverIP = "192.168.235.138"; //IP DEL PROPRIO PC
    public int serverPort = 5000; // PORTA DEL PROPRIO SERVER 
    public float sendInterval = 0.5f; // INTERVALLO DI TEMPO TRA UN FRAME E L'ALTRO
    private float nextSendTime = 0f;
    [SerializeField]
    private Texture2D textureToSend;
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

    private void Update()
    {
       /*
        if (Time.time >= nextSendTime)
        {
            nextSendTime = Time.time + sendInterval;

            if (isConnected)
            {
                SendTextureToServer(textureToSend);
            }
            else
            {
                ConnectToServer();
            }
        }
        */
    }
    
    private void SendTextureToServer(Texture2D texture)
    {
        if (stream == null || !stream.CanWrite)
        {
            isConnected = false;
            Debug.LogWarning("Stream non disponibile, tentativo di riconnessione...");
            ConnectToServer();
            return;
        }

        if (texture == null)
        {
            Debug.LogWarning("Nessuna Texture2D specificata per l'invio.");
            return;
        }

        // Converti la texture in JPG o PNG
        byte[] imageBytes = texture.EncodeToPNG(); // Usa PNG o JPG a seconda delle esigenze

        // Invia la lunghezza dell'immagine (4 byte)
        byte[] lengthPrefix = BitConverter.GetBytes(imageBytes.Length);
        try
        {
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            // Invia l'immagine
            stream.Write(imageBytes, 0, imageBytes.Length);
            stream.Flush();
            Debug.Log("Texture inviata al server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante l'invio della texture al server: " + e.Message);
            isConnected = false;
            // Tenta di riconnetterti
            ConnectToServer();
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
        // Verifica se la texture è leggibile e in un formato supportato
        if (!texture.isReadable || !IsTextureFormatSupported(texture.format))
        {
            // Crea una copia leggibile e non compressa della texture
            Texture2D readableTexture = CreateReadableTexture(texture);
            // **Flip della texture prima di inviare**
            //FlipTextureVertically(readableTexture);
            
            // Invia il frame al server
            SendFrameToServer(readableTexture);

            // Rilascia la texture temporanea per evitare perdite di memoria
            Destroy(readableTexture);
        }
        else
        {
            // **Flip della texture prima di inviare**
          //  FlipTextureVertically(texture);

            // La texture è già leggibile e in un formato supportato
            // Invia il frame al server
            SendFrameToServer(texture);
        }
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
    private void SendTestMessageToServer()
    {
        if (stream == null || !stream.CanWrite)
        {
            Debug.LogWarning("Stream non disponibile, impossibile inviare il messaggio.");
            return;
        }

        try
        {
            string message = "Ciao dal client!";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

            // Invia la lunghezza del messaggio (4 byte)
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length);
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);

            // Invia il messaggio
            stream.Write(data, 0, data.Length);
            stream.Flush();

            Debug.Log("Messaggio di test inviato al server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante l'invio del messaggio al server: " + e.Message);
            isConnected = false;
            ConnectToServer();
        }
    }
    private bool IsTextureFormatSupported(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.ARGB32:
            case TextureFormat.RGBA32:
            case TextureFormat.RGB24:
            case TextureFormat.Alpha8:
                return true;
            default:
                return false;
        }
    }

// Crea una texture leggibile e non compressa
    private Texture2D CreateReadableTexture(Texture2D texture)
    {
        // Crea una RenderTexture temporanea
        RenderTexture tempRT = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        // Copia la texture originale nella RenderTexture
        Graphics.Blit(texture, tempRT);

        // Salva la RenderTexture corrente
        RenderTexture previous = RenderTexture.active;

        // Imposta la RenderTexture attiva
        RenderTexture.active = tempRT;

        // Crea una nuova Texture2D leggibile e non compressa
        Texture2D readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // Copia i pixel dalla RenderTexture alla nuova Texture2D
        readableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        readableTexture.Apply();

        // Ripristina la RenderTexture attiva
        RenderTexture.active = previous;

        // Rilascia la RenderTexture temporanea
        RenderTexture.ReleaseTemporary(tempRT);

        return readableTexture;
    }
    // Funzione per ruotare verticalmente una texture
    private void FlipTextureVertically(Texture2D texture)
    {
        int w = texture.width;
        int h = texture.height;
        Color[] pixels = texture.GetPixels();
        Color[] flippedPixels = new Color[pixels.Length];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                flippedPixels[(h - 1 - y) * w + x] = pixels[y * w + x];
            }
        }

        texture.SetPixels(flippedPixels);
        texture.Apply();
    }
    
}
