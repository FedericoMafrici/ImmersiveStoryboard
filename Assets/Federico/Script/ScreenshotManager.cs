using System;
using System.Collections;
using System.Collections.Generic;
using Trev3d.Quest.ScreenCapture;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    [SerializeField] public List<RawImage> _screenshots=null;
    public List<byte[]> images = null;
    private int _screenshotCounter=0;
    public Hashtable focalTable = new Hashtable();
    private string _focalLength = "15mm ";
    public Hashtable actionTimes = new Hashtable();
    private int _currActionTime;
    private int _code = 0;
    private SimulationManager _simulationManager;
    private OutputGenerator _outputGenerator;
    private AnimaPersonaggio _animaPersonaggio;
    
    private QuestScreenCaptureTextureManager _screenCaptureTextureManager;

    public static EventHandler<EventArgs> screenShotTaken;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _simulationManager = FindObjectOfType<SimulationManager>();
        _outputGenerator = FindObjectOfType<OutputGenerator>();
        _animaPersonaggio = FindObjectOfType<AnimaPersonaggio>();
        _screenCaptureTextureManager= FindAnyObjectByType<QuestScreenCaptureTextureManager>();
       _screenshotCounter = 0;
    }

    public void TakeScreenshot()
    {
        if (_screenCaptureTextureManager == null)
        {
            Debug.LogError("Errore capturetexturemanager non trovato");
        }
        // informazioni utili per lo screenshot
        _code = _outputGenerator.sceneCode;
        _screenCaptureTextureManager.TakeScreenShot(_screenshots[_screenshotCounter]);
        _screenshots[_screenshotCounter].name =_code.ToString() + "_img" + _simulationManager.GetScreenshotCount().ToString();
        //aggiorni i valori tabulati
        _currActionTime = _simulationManager.GetTime();
        AddActionTime(_simulationManager.GetScreenshotCount().ToString(), _currActionTime);
        //chiama il metodo per generare una frase di camminata e una di vicinanza
        _simulationManager.GenerateCondition()
            ;
        AddNewValue(_simulationManager.GetScreenshotCount().ToString(), _focalLength); //memorizza la focale al tempo time 
        //incrementa il counter degli screenshot 
        _screenshotCounter++;
        screenShotTaken.Invoke(this,EventArgs.Empty);
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
    
    // Update is called once per frame
    void Update()
    {
        
    }
}