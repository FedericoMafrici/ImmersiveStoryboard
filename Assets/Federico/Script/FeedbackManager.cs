using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] public GameObject FeedbackCanvas;
    [SerializeField] public GameObject ScreenShotTakenFeedback;
    [SerializeField] public Image ScreenShotTakenBackground;
    [SerializeField] public TextMeshProUGUI ScreenShotTakenText;
    [SerializeField] public float fadeDuration = 2.0f; // Durata del fade
   [Header("Storyboard Saved Components")]
   [SerializeField] public GameObject StoryboardSaved;
   [SerializeField] public Image StoryboardSavedBackground;
   [SerializeField] public TextMeshProUGUI StoryboardSavedText;
   [Header("Object Placed Components")]
   [SerializeField] public GameObject ObjectPlaced;
   [SerializeField] public Image ObjectPlacedBackground;
   [SerializeField] public TextMeshProUGUI ObjectPlacedText;
   [Header("Bounding box detected")]
   [SerializeField] public GameObject boundingBoxDetected;
   [SerializeField] public Image boundingBoxDetectedImage;
   [SerializeField] public TextMeshProUGUI boundingBoxDetectedText;
   
   
   // GESTIONE AUDIO
   [SerializeField] public AudioSource genericFeedbackClip;
    private XROrigin _arSessionOrigin;
    private void Start()
    {
        ScreenshotManager.screenShotTaken += ScreenShotTaken;
        OutputGenerator.storyboardSaved += StoryboardSavedFeedback;
        ControllerManager.OnObjectPlaced += OnObjectPlaced;
        ControllerManager.OnBoundingBoxFounded += BoundingBoxFounded;
        InteractionManagerAddOn.onObjectSelected += OnObjectPlaced;
        _arSessionOrigin = FindObjectOfType<XROrigin>();
        
        
    }

    private void OnDisable()
    {
        ScreenshotManager.screenShotTaken += ScreenShotTaken;
        OutputGenerator.storyboardSaved += StoryboardSavedFeedback;
        ControllerManager.OnObjectPlaced += OnObjectPlaced;
        InteractionManagerAddOn.onObjectSelected += OnObjectPlaced;
    }

    private void Update()
    {
        
        if (_arSessionOrigin != null)
        {
            // Ottieni la posizione della telecamera AR
            Vector3 cameraPosition = _arSessionOrigin.GetComponentInChildren<Camera>().transform.position;
            Quaternion cameraRotation = _arSessionOrigin.GetComponentInChildren<Camera>().transform.rotation;
            this.transform.position = cameraPosition;
            this.transform.rotation = cameraRotation;
            // Usa cameraPosition e cameraRotation come necessario
        }
        
        
    }

    private IEnumerator FadeOut(Image imageComponent, Graphic textComponent, float fadeDuration)
    {
        // Colore originale
        FeedbackCanvas.SetActive(true);
        Color imageColor = imageComponent.color;
        Color textColor = textComponent.color;
        imageColor.a = 1.0f;
        textColor.a = 1.0f;
        // Velocità di fading
        float fadeSpeed = 1.0f / fadeDuration;

        // Ciclo per il fading
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * fadeSpeed)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, t);

            // Modifica l'alfa dell'immagine
            imageComponent.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);

            // Modifica l'alfa del testo
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
    
            // Aspetta un frame
            yield return null;
        }

        // Assicurati che siano completamente trasparenti alla fine
        imageComponent.color = new Color(imageColor.r, imageColor.g, imageColor.b, 0.0f);
        textComponent.color = new Color(textColor.r, textColor.g, textColor.b, 0.0f);
        FeedbackCanvas.SetActive(false);
    }
    
    
    public void ScreenShotTaken(object sender, EventArgs obj)
    {
        genericFeedbackClip.Play();
        //StartCoroutine(FadeOut(ScreenShotTakenBackground,ScreenShotTakenText,fadeDuration));
    }

    public void StoryboardSavedFeedback(object sender, EventArgs obj)
    {
        genericFeedbackClip.Play();
        //StartCoroutine(FadeOut(StoryboardSavedBackground, StoryboardSavedText, fadeDuration));
    }
 private void OnObjectPlaced(object sender, EventArgs e)
    {
        genericFeedbackClip.Play();
     //   StartCoroutine(FadeOut(ObjectPlacedBackground, ObjectPlacedText, fadeDuration));
    }

    public void BoundingBoxFounded(object sender, EventArgs e)
    {
        genericFeedbackClip.Play();
      //  StartCoroutine(FadeOut(boundingBoxDetectedImage, boundingBoxDetectedText, fadeDuration));
    }
}
