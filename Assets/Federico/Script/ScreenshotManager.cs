using System;
using System.Collections;
using System.Collections.Generic;
using Trev3d.Quest.ScreenCapture;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    [SerializeField] public List<RawImage> _screenshots=new List<RawImage>();
    [SerializeField] public List<Texture2D> screenshotsTexture = new List<Texture2D>();
    [SerializeField] public GameObject UI;
    [SerializeField] public CenterUIPanel panelHandler;
    public List<byte[]> images = null;
    private int _screenshotCounter=0;
    public Hashtable focalTable = new Hashtable();
    private string _focalLength = "15";
    public Hashtable actionTimes = new Hashtable();
    private int _currActionTime;
    private int _code = 0;
    private SimulationManager _simulationManager;
    private OutputGenerator _outputGenerator;
    private AnimaPersonaggio _animaPersonaggio;
    private PhraseGenerator _phraseGenerator;
    
    private QuestScreenCaptureTextureManager _screenCaptureTextureManager;

    public static EventHandler<EventArgs> screenShotTaken;

    public static readonly Vector2Int Size = new(1024, 1024);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _simulationManager = FindObjectOfType<SimulationManager>();
        _outputGenerator = FindObjectOfType<OutputGenerator>();
        _animaPersonaggio = FindObjectOfType<AnimaPersonaggio>();
        _screenCaptureTextureManager= FindAnyObjectByType<QuestScreenCaptureTextureManager>();
        _phraseGenerator = FindObjectOfType<PhraseGenerator>();
        _screenshotCounter = 0;
        if (_simulationManager == null || _phraseGenerator == null || _screenCaptureTextureManager == null
            || _animaPersonaggio == null)
        {
            Debug.LogError("errore nella generazione dello script");
        }
       int length = _screenshots.Count;
       for (int i = 0; i < length; i++)
       {
            screenshotsTexture.Add(new Texture2D(Size.x, Size.y, TextureFormat.RGBA32, 1, false));
       }
    }
    /*
    public void TakeScreenshot()
    {
        panelHandler.HidePanels();
        // yield return new WaitForSeconds(1);
        if (_screenCaptureTextureManager == null)
        {
            Debug.LogError("Errore capturetexturemanager non trovato");
        }
        // informazioni utili per lo screenshot
        _code = _outputGenerator.sceneCode;
        _screenCaptureTextureManager.ScreenShotButtonPressed(_screenshots[_screenshotCounter],screenshotsTexture[_screenshotCounter]);
        _screenshots[_screenshotCounter].name =_code.ToString() + "_img" + _simulationManager.GetScreenshotCount().ToString();
        //aggiorni i valori tabulati
        _currActionTime = _simulationManager.GetTime();
        AddActionTime(_simulationManager.GetScreenshotCount().ToString(), _currActionTime);
        //chiama il metodo per generare una frase di camminata e una di vicinanza
        _simulationManager.GenerateCondition();
        AddNewValue(_simulationManager.GetScreenshotCount().ToString(), _focalLength); //memorizza la focale al tempo time 
        //incrementa il counter degli screenshot 
        _screenshotCounter++;
        _simulationManager.screenshotCount++;
        screenShotTaken.Invoke(this,EventArgs.Empty);
        panelHandler.ShowNotHiddenPanels();
    }
    */
    public void TakeScreenshot()
    {
        // Avvia la coroutine che gestisce l'attesa e lo screenshot
        StartCoroutine(TakeScreenshotCoroutine());
    }

    private IEnumerator TakeScreenshotCoroutine()
    {
        panelHandler.HidePanels();
        yield return new WaitForSeconds(1); // Attende 1 secondo

        if (_screenCaptureTextureManager == null)
        {
            Debug.LogError("Errore: _screenCaptureTextureManager non trovato");
            yield break; // Esce dalla coroutine se c'è un errore
        }

        // Informazioni utili per lo screenshot
        _code = _outputGenerator.sceneCode;
        _screenCaptureTextureManager.ScreenShotButtonPressed(_screenshots[_screenshotCounter], screenshotsTexture[_screenshotCounter]);
        _screenshots[_screenshotCounter].name = $"{_code}_img{_simulationManager.GetScreenshotCount()}";

        // Aggiorna i valori tabulati
        _currActionTime = _simulationManager.GetTime();
        AddActionTime(_simulationManager.GetScreenshotCount().ToString(), _currActionTime);

        // Genera le condizioni necessarie
        _simulationManager.GenerateCondition();
        AddNewValue(_simulationManager.GetScreenshotCount().ToString(), _focalLength);

        // Incrementa il counter degli screenshot
        _screenshotCounter++;
        _simulationManager.screenshotCount++;

        // Invoca l'evento se ci sono sottoscrittori
        screenShotTaken?.Invoke(this, EventArgs.Empty);
            panelHandler.ShowPanels();
    //    panelHandler.ShowNotHiddenPanels();
    }
    public void DeleteScreenshot()
    {
        if (_screenshotCounter > 0)
        {
            // Decrementa il counter degli screenshot
            _screenshotCounter--;
        
            // Ottieni l'indice dello screenshot da eliminare
            string indexToDelete = _simulationManager.GetScreenshotCount().ToString();

            // Rimuovi gli elementi dalla Hashtable focalTable
            if (focalTable.ContainsKey(indexToDelete))
            {
                focalTable.Remove(indexToDelete);
            }
        
            // Rimuovi gli elementi dalla Hashtable actionTimes
            if (actionTimes.ContainsKey(indexToDelete))
            {
                actionTimes.Remove(indexToDelete);
            }

            // Rimuovi l'immagine dallo screenshot list
            if (_screenshots.Count > _screenshotCounter)
            {
                _screenshots.RemoveAt(_screenshotCounter);
            }

            // Rimuovi la texture dallo screenshotTexture list
            if (screenshotsTexture.Count > _screenshotCounter)
            {
                Texture2D textureToClear = screenshotsTexture[_screenshotCounter];
                RenderTexture tempRenderTexture = RenderTexture.GetTemporary(textureToClear.width, textureToClear.height);
                RenderTexture.active = tempRenderTexture;

                GL.Clear(true, true, Color.white);
                textureToClear.ReadPixels(new Rect(0, 0, textureToClear.width, textureToClear.height), 0, 0);
                textureToClear.Apply();

                RenderTexture.ReleaseTemporary(tempRenderTexture);

                // Rimuovi la texture dalla lista
                screenshotsTexture.RemoveAt(_screenshotCounter);
            }

            // Riduci il conteggio degli screenshot nel SimulationManager
            _simulationManager.screenshotCount--;
        
            Debug.Log($"Screenshot {indexToDelete} eliminato correttamente.");
        }
        else
        {
            Debug.LogError("Nessuno screenshot da eliminare.");
        }
        
    }
    public void AddNewValue(string index, string focal)
    {
        if (focalTable.ContainsKey(index))
        {
            focalTable[index] = focal;
        }
        else
        {
            focalTable.Add(index, focal);
        }
            
    }
    public void AddActionTime(string index, int time)
    {

        if (actionTimes.ContainsKey(index))
        {
            actionTimes[index] = time;
        }
        else
        {
            actionTimes.Add(index, time);
        }
        
    }

    public void setActionType()
    {
        _simulationManager.SetContemporaryAction();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
