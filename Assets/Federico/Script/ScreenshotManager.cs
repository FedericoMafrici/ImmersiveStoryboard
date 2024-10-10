using System.Collections.Generic;
using Trev3d.Quest.ScreenCapture;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> _screenshots=null;
    private int _screenshotCounter=0;
    private QuestScreenCaptureTextureManager _screenCaptureTextureManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _screenCaptureTextureManager= FindAnyObjectByType<QuestScreenCaptureTextureManager>();
       _screenshotCounter = 0;
    }

    public void TakeScreenshot()
    {
        if (_screenCaptureTextureManager == null)
        {
            Debug.LogError("Errore capturetexturemanager non trovato");
        }
        _screenCaptureTextureManager.TakeScreenShot(_screenshots[_screenshotCounter]);
        _screenshotCounter++;
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
