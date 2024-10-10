using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.AI;
using System;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class SimulationManager : MonoBehaviour
{
    [Header("Manager")]
    [FormerlySerializedAs("menuManoAnimazioni")] [SerializeField] public AnimaPersonaggio characterAnimationManager;
    [Header("Personaggi e componenti attivi")]
    public GameObject activeCharacter;
    public Animator activeCharacterAnimator;
    [Header("Oggetti Instanziati nella scena")]
    public List<GameObject> spawnedGameObjects = new List<GameObject>();

    [Header("Personaggi e componenti attivi")]
    public int status=0; // 0 -> object placement; 1 -> storyboarding 
    
    [SerializeField] GameObject objectCollection;
    [SerializeField] List<GameObject> listaOggettiManipolabili;
    
    [SerializeField] GameObject timeSlider;

    [Header("Generazione Frasi ")]
    [SerializeField] PhraseGenerator phraseGenerator;
    // sistema ad eventi per gestire il flusso dell'applicazione
    public static EventHandler<EventArgs> startStoryboarding;
    public static EventHandler<EventArgs> pauseStoryboarding;
    public static EventHandler<EventArgs> setUpMovement;
    public static EventHandler<EventArgs> stopAnimation;
    
    [Header("HEADER UI ")]
    [SerializeField] GameObject _buttonRenameScene;
    [SerializeField] GameObject _buttonSaveScene;
    [SerializeField] GameObject _buttonStartStoryboarding;
    [SerializeField] GameObject _buttonStopStoryboarding;
    [SerializeField] GameObject _buttonStoryboard;
    [SerializeField] GameObject _buttonUseCase1;
    [SerializeField] GameObject _buttonUseCase2;
    [SerializeField] GameObject BottoneRinominaOggetto;

    [SerializeField] GameObject _canvasUseCase1;
    [SerializeField] GameObject _canvasUseCase2;

   // public GridObjectCollection SceneMenuButtonGridObjectCollection;
    
    public Material ComandiOculusWorldBuilding;
    public Material ComandiOculusStoryboard;
    public GameObject imageOculus;

    public AudioClip _notificationSound;
    private GameObject _soundManager;

    [SerializeField] GameObject GetControlButton;

   
   
    
    public GameObject activeCharacterText;
 //   public ObjectManagerVR objectManagerVR;
    private GameObject canvasNewLight;

  //  public CameraManager cameraManager;
   
    public GameObject PhotoCaptureManager;

    public GameObject secondCamera;
    public GameObject ButtonScreenshot;
    public GameObject StepPinchSliderFocalLength;

    public GameObject sfondoTitolo;

    public GameObject ParticleActive;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject StopAndPlayButton;

    public GameObject ManuAzioniTasti;
    public GameObject SfondoAzioni;

    public GameObject StopAndPlayButtonPadre;
    public GameObject actionsPanel;

    public GameObject light;
    private GameObject newLight;
    //public GameObject cameraHint;
    //public GameObject sceneInputName;
    //public GameObject sceneNameText;
    //public GameObject dropdownHandlder;
    public string sName;
    //private int loopCount = 0;
    private int screenshotCount = 0;
    private int screenshotCountUndo = 0;

    public bool contemporaryAction = true;
    public bool firstAction = true;

    public bool startSimulationPremuto;

    private Camera mainCamera;
    // public GameObject timeManager;
    //public Slider timeSlider;

    //public int time;

    public bool controlCamera;

    public bool dialogue; //booleano che � a true se � in corso un dialogo (pannello dialogo attivo). Settato dal phraseGenerator e controllato dal ThirdPersonMovement

    public GameObject contemporaryButton;

    public GameObject consecutiveButton;

    [SerializeField] private TextMeshPro numberIllustration = null;


    public GameObject positionActiveObject;
    public TextMeshPro NomeActiveCh;

    public Material bordoScuro;

    public Material bordoChiaro;

    public TextMeshPro NomePersonaggioAttivo;

    public GameObject feedbackParticleActive;
    public GameObject feedbackParticleComplement;
    public GameObject particleParent;

    public GameObject NextPannelButton;

    [SerializeField] GameObject _pannelloFine;
    
    private List<string> buffer;

    /*public GameObject tutorial;
    private string[] tutorialArray = new string[] { "Use the dropdown menu to select environment elements, props or characters.", 
                                                    "Add the object with the '+' button and click on the spot where you want to place it.",
                                                    "Toggle the guide grid to place elements in a better way.",
                                                    "Select the already-placed objects to perform editing actions. Use right click to close.",
                                                    "Control camera with WASD + QE for translations; Press also spacebar for pan/roll/tilt.",
                                                    "Click on 'Start Simulation' when you are ready.",
                                                    "Select the character you want to control and use WASD to move it.",
                                                    "When your character is near another object, click on it to show actions.",
                                                    "Click on the camera icon to take control of the camera and see further commands.",
                                                    "Control time moving the timeline forward. You can also increase time limit.",
                                                     "Click 'P' to take a screenshot and 'Generate Output' to generate the storyboard.",
                                                    };

    public int tutorialIndex;
    public TextMeshProUGUI tutorialText;
    public GameObject tutorialPanel;*/


    void Start()
    {
        
    }

    public void  Update()
    {
        
    }
    public void StartStoryBoarding(TMP_Text txtcomponent)
    {
        if (status == 0)
        {
            status = 1;
            startStoryboarding.Invoke(this, new EventArgs());
            txtcomponent.text = "Stop";
        }
        else
        {
            status = 0;
            pauseStoryboarding.Invoke(this, new EventArgs());
            txtcomponent.text = "Start";
        }
    }
    //change the tracking of all scene object
  /*  
    void Start()
    {
    /*     
        startSimulationPremuto = false;
       
       // _buttonStopStoryboarding.SetActive(false);

        //_canvasUseCase1.SetActive(false);
//        _canvasUseCase2.SetActive(false);

  //      _buttonUseCase1.SetActive(true);
        _buttonUseCase2.SetActive(true);

        _soundManager = GameObject.Find("SoundManager");
      //  cameraManager = FindObjectOfType<CameraManager>();
        // Texture t2d = AssetPreview.GetMiniThumbnail(prova) as Texture;

        // image.GetComponent<RawImage>().texture = t2d;

     //  objectManagerVR = FindObjectOfType<ObjectManagerVR>();

        canvasNewLight = GameObject.Find("ParentCanvas").transform.Find("Canvas_NewLightAdded").gameObject;
        status = 0;
        activeCharacter = null;
        activeCharacterText.SetActive(false);
        characterAnimationManager.SetActive(false);

        _pannelloFine.SetActive(false);
        //animaPersonaggio = FindObjectOfType<AnimaPersonaggio>();

     //   NextPannelButton.GetComponent<Interactable>().IsEnabled = false;

        //secondCamera.SetActive(false);
        ButtonScreenshot.SetActive(false);
        StepPinchSliderFocalLength.SetActive(false);

        mainCamera = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();

       //mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        dialogue = false;

        //time = 1;
        sName = "no name";

        //pulire il file con le azioni
        string basePath = Path.Combine(Application.streamingAssetsPath, "screenshotActions.csv");
        File.WriteAllText(basePath, string.Empty);

   //     phraseGenerator = FindObjectOfType<PhraseGenerator>();
    
    
    }
*/
    /*tutorialIndex = 0;

    tutorialText = tutorial.GetComponent<TextMeshProUGUI>();
    tutorialText.text = tutorialArray[tutorialIndex].ToString();




public void IncrementTutorialIndex() {
    if (tutorialIndex < 10) tutorialIndex++;
    else if (tutorialIndex == 10) tutorialIndex = 0;

    tutorialText.text = tutorialArray[tutorialIndex].ToString();

}
public void DecrementTutorialIndex()
{
    if (tutorialIndex > 0) tutorialIndex--;
    else if (tutorialIndex == 0) tutorialIndex = 10;

    tutorialText.text = tutorialArray[tutorialIndex].ToString();

}
public void ToggleTutorial()
{
    if (tutorialPanel.activeSelf)
        tutorialPanel.SetActive(false);
    else tutorialPanel.SetActive(true);

}*/

/*
    void Update()

    {
        // al momento il tempo scorre a seconda degli screen fatti 
        //time = PhotoCaptureManager.GetComponent<SR_RenderCamera>().FileCounter;


        /*sceneNameText.GetComponent<TextMeshPro>().text = sName;
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (controlCamera)
            {
                controlCamera = false;
                activeCharacterText.GetComponent<TextMeshPro>().text = "Active player: none";
                //cameraHint.SetActive(true);
            }
            else
            {
                //cameraHint.SetActive(true);
                ControlCamera();
            }
        }*/


       /* // GENERATE STORYBOARD FINALE FUNZIONA SOLO IN STATUS STORYBOARD
        if ((Input.GetKeyDown(KeyCode.Y) || OVRInput.GetDown(OVRInput.RawButton.Y)) && status == 1)
        {
            gameObject.GetComponent<OutputGenerator>().GenerateFile();
            _soundManager.GetComponent<AudioSource>().PlayOneShot(_notificationSound);
            _pannelloFine.SetActive(true);
        }*/

        // NEW CAMERA FUNZIONA SOLO IN STATUS STORYBOARD
        /*
        if ((Input.GetKeyDown(KeyCode.B) || OVRInput.GetDown(OVRInput.RawButton.B)) && status == 1 && secondCamera.activeSelf)
        {
            cameraManager.NewCamera();
            _soundManager.GetComponent<AudioSource>().PlayOneShot(_notificationSound);
        }

        // UNDO FUNZIONA SOLO IN STATUS STORYBOARD
        if ((Input.GetKeyDown(KeyCode.P) || OVRInput.GetDown(OVRInput.RawButton.Y)) && status == 1)
        {
            UndoLastAction();
            _soundManager.GetComponent<AudioSource>().PlayOneShot(_notificationSound);
        }

        // NUOVA LUCE
        if ((Input.GetKeyDown(KeyCode.L) || OVRInput.GetDown(OVRInput.RawButton.A)) && light.activeSelf)
        {
            NewLight();
            _soundManager.GetComponent<AudioSource>().PlayOneShot(_notificationSound);
        }

        if (activeCharacter != null && ParticleActive != null && status == 1)
        {
            ParticleActive.transform.position = new Vector3(activeCharacter.transform.position.x, activeCharacter.transform.position.y + 0.02f, activeCharacter.transform.position.z);
        }

        if (activeCharacter != null && NomePersonaggioAttivo.gameObject.activeSelf && status == 1)
        {
            NomePersonaggioAttivo.text = activeCharacter.name;
        }


        if (activeCharacter != null && activeCharacter != objectManagerVR.current && status == 1)
        {
            positionActiveObject.SetActive(true);
            NomeActiveCh.text = activeCharacter.name; // definisce nome oggetto

            positionActiveObject.transform.position = new Vector3(activeCharacter.transform.position.x + 0.05f, activeCharacter.transform.position.y + (activeCharacter.transform.position.y) / 2.5f, activeCharacter.transform.position.z); // posiziona nome oggetto in base al padre empty
        }
        else
        {
            positionActiveObject.SetActive(false);
            NomeActiveCh.text = "";
        
        
    }
*/
    private void NewLight()
    {
        newLight = Instantiate(light);
        newLight.transform.GetChild(0).name = newLight.transform.GetChild(0).name + UnityEngine.Random.Range(0,10000).ToString();


        newLight.transform.position = mainCamera.transform.position;
        newLight.transform.rotation = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles);
        //newLight.transform.SetPositionAndRotation(new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, mainCamera.transform.localPosition.z + 0.3f), Quaternion.Euler(mainCamera.transform.rotation.eulerAngles));

        canvasNewLight.SetActive(true);
        StartCoroutine(DeactivatecanvasNewLight());
    }


    private IEnumerator DeactivatecanvasNewLight()
    {
        yield return new WaitForSeconds(3);

        canvasNewLight.SetActive(false);
    }


    /*public void SetSceneName()
{
   //sName = sceneInputName.GetComponent<MRTKTMPInputField>().text;

}*/

    /*public void SaveScene()
    {
        GameObject.Find("SaveManager")?.GetComponent<SaveLoadStage>().SaveData(sName);
    }*/
    

    public void StartSimulation() 
    {
        startSimulationPremuto = true;
        
        //menuManoOggetti.SetActive(false);
        BottoneRinominaOggetto.SetActive(false); // lascio nome oggetto ma disattivo Bottone che serve a rinominare in fase di storyboard

        //sceneMenu.SetActive(false);
        _buttonRenameScene.SetActive(false);
        if (_buttonSaveScene.activeSelf) _buttonSaveScene.SetActive(false);
        _buttonStartStoryboarding.SetActive(false);

        _buttonStopStoryboarding.SetActive(true);

        _buttonStoryboard.SetActive(true);




    //    SceneMenuButtonGridObjectCollection.UpdateCollection();

       

        imageOculus.GetComponent<Image>().material = ComandiOculusStoryboard;

        characterAnimationManager.enabled = true;
        objectCollection.SetActive(false);

        //Attiva Camera e oggetti collegati quando premo Start Storyboarding
        secondCamera.SetActive(true);
        ButtonScreenshot.SetActive(true);
        StepPinchSliderFocalLength.SetActive(true);

        //characterAnimationManager.GetComponent<AnimaPersonaggio>().enabled = true;

       // objectManagerVR.StopManipulation();
        
        //controlla che ci sia almeno un personaggio
        var atleast1pg = false;
        GameObject empty = GameObject.FindGameObjectWithTag("emptyPlane");
        
        foreach (Transform child in empty.transform)
        {
            if (child.CompareTag("Player"))
                atleast1pg = true;
        }
        if (atleast1pg)
        {

            status = 1;
            //DeactivateTriggers();
            activeCharacterText.SetActive(true);
            activeCharacterText.GetComponent<TextMeshPro>().text = "Active player: none";
            
        }

       /* // tolgo parentela messa in StopSimulation
        moveButton.transform.parent = null;
        StopAndPlayButton.transform.parent = null;
        actionsPanel.transform.parent = null;*/

        // tolgo parentela messa in StopSimulation
        moveButton.transform.SetParent(ManuAzioniTasti.transform);
        StopAndPlayButton.transform.SetParent(StopAndPlayButtonPadre.transform);
        actionsPanel.transform.SetParent(ManuAzioniTasti.transform);
        SfondoAzioni.transform.SetParent(StopAndPlayButtonPadre.transform);
    }

    public void StartSimulationTutorial()
    {
        startSimulationPremuto = true;

        //menuManoOggetti.SetActive(false);
        BottoneRinominaOggetto.SetActive(false); // lascio nome oggetto ma disattivo Bottone che serve a rinominare in fase di storyboard

        //sceneMenu.SetActive(false);
        _buttonRenameScene.SetActive(false);
        if (_buttonSaveScene.activeSelf) _buttonSaveScene.SetActive(false);
        _buttonStartStoryboarding.SetActive(false);

        _buttonStopStoryboarding.SetActive(true);

        _buttonStoryboard.SetActive(true);




     //   SceneMenuButtonGridObjectCollection.UpdateCollection();



        imageOculus.GetComponent<Image>().material = ComandiOculusStoryboard;

        characterAnimationManager.enabled=true;
        objectCollection.SetActive(false);

        /*//Attiva Camera e oggetti collegati quando premo Start Storyboarding
        secondCamera.SetActive(true);
        ButtonScreenshot.SetActive(true);
        StepPinchSliderFocalLength.SetActive(true);*/

      //  characterAnimationManager.GetComponent<AnimaPersonaggio>().enabled = true;

      //  objectManagerVR.StopManipulation();

        //controlla che ci sia almeno un personaggio
        var atleast1pg = false;
        GameObject empty = GameObject.FindGameObjectWithTag("emptyPlane");

        foreach (Transform child in empty.transform)
        {
            if (child.CompareTag("Player"))
                atleast1pg = true;
        }
        if (atleast1pg)
        {

            status = 1;
           
            activeCharacterText.SetActive(true);
            activeCharacterText.GetComponent<TextMeshPro>().text = "Active player: none";

        }

      

        // tolgo parentela messa in StopSimulation
        moveButton.transform.SetParent(ManuAzioniTasti.transform);
        StopAndPlayButton.transform.SetParent(StopAndPlayButtonPadre.transform);
        actionsPanel.transform.SetParent(ManuAzioniTasti.transform);
        SfondoAzioni.transform.SetParent(StopAndPlayButtonPadre.transform);
    }

    public void StopSimulation()
    {
     
       
        BottoneRinominaOggetto.SetActive(true);
        characterAnimationManager.enabled = true;


        _buttonRenameScene.SetActive(true);
        
        _buttonStartStoryboarding.SetActive(true);

        _buttonStopStoryboarding.SetActive(false);

        _buttonStoryboard.SetActive(false);

     //   SceneMenuButtonGridObjectCollection.UpdateCollection();

        imageOculus.GetComponent<Image>().material = ComandiOculusWorldBuilding;

        // imparento ad un oggetto disattivo altrimenti si riattivavano al click del personaggio
        moveButton.transform.SetParent(characterAnimationManager.transform);
        StopAndPlayButton.transform.SetParent(characterAnimationManager.transform);
        actionsPanel.transform.SetParent(characterAnimationManager.transform);
        SfondoAzioni.transform.SetParent(characterAnimationManager.transform);

        // simulationMenu.SetActive(false); // DISATTIVARE STOP STORYBOARDING
        objectCollection.SetActive(true);
      //  characterAnimationManager.GetComponent<AnimaPersonaggio>().enabled = false;
      //  objectManagerVR.RestartManipulation();
        status = 0;

    }


    public void ChangeMaxTime() { }
    public void ChangeCurrentTime() { }
    public void ChangeCurrentTimeManual() { }
    public void SpendTime() { }

    public int GetTime() 
    {

      //  var slider = timeSlider.GetComponent<PinchSlider>().SliderValue;
      //    var time = Mathf.RoundToInt(slider * 10); 
      //  return time; 
      return 1; //TODO da rimuovere e ripristinare con quello sopra
    }
    public void ControlCamera(){ }

    public void SetContemporaryAction(bool b)
    {
        contemporaryAction = b;
   //     animaPersonaggio.ChangeWalker();
    }

        /*
    public void ChangeColorContemporary()
    {
    
        // colori tasto
        Color DefaltCustomColor = new Color(0.8509805f, 0.1686275f, 0.5686275f, 1);
        Color FocusCustomColor = new Color(0.09803922f, 0.1490196f, 0.254902f, 1);
        Color PressedCustomColor = new Color(0.5526878f, 0.582599f, 0.8679245f, 1);

        Color DefaltCustomColorDisabled = new Color(0.3215686f, 0.3215686f, 0.3215686f, 1);
        //Color ColorTextDisabled = new Color(0.2641509f, 0.240477f, 0.240477f, 1);

        Color ColorTextDisabled = Color.white;

        var newThemeType = ThemeDefinition.GetDefaultThemeDefinition<InteractableColorTheme>().Value;
        var interactableObject = contemporaryButton.transform.Find("BackPlate").Find("Quad").gameObject;

        // coloro accesso bottone attivo
        newThemeType.StateProperties[0].Values = new List<ThemePropertyValue>()
                            {
                                new ThemePropertyValue() { Color = DefaltCustomColor},  // Default ROSA
                                new ThemePropertyValue() { Color = FocusCustomColor}, // Focus
                                new ThemePropertyValue() { Color = PressedCustomColor},   // Pressed
                                new ThemePropertyValue() { Color = Color.black},   // Disabled
                            };


        contemporaryButton.GetComponent<Interactable>().Profiles = new List<InteractableProfileItem>()

        

        {
                                new InteractableProfileItem()
                                {
                                    Themes = new List<Theme>()
                                    {
                                        Interactable.GetDefaultThemeAsset(new List<ThemeDefinition>() { newThemeType })
                                    },
                                    Target = interactableObject,
                                },
                            };
    
        contemporaryButton.transform.Find("BackPlate").Find("Quad").gameObject.GetComponent<Renderer>().material = bordoChiaro;

        contemporaryButton.transform.Find("IconAndText").Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().color = Color.white;

        // spengo bottone opposto 
        var newThemeType2 = ThemeDefinition.GetDefaultThemeDefinition<InteractableColorTheme>().Value;
        var interactableObject2 = consecutiveButton.transform.Find("BackPlate").Find("Quad").gameObject;

        // Define a color for every state in our Default Interactable States
        newThemeType2.StateProperties[0].Values = new List<ThemePropertyValue>()
                            {
                                new ThemePropertyValue() { Color = DefaltCustomColorDisabled},  // Default GRIGIO
                                new ThemePropertyValue() { Color = FocusCustomColor}, // Focus
                                new ThemePropertyValue() { Color = PressedCustomColor},   // Pressed
                                new ThemePropertyValue() { Color = Color.black},   // Disabled
                            };


        consecutiveButton.GetComponent<Interactable>().Profiles = new List<InteractableProfileItem>()
                            {
                                new InteractableProfileItem()
                                {
                                    Themes = new List<Theme>()
                                    {
                                        Interactable.GetDefaultThemeAsset(new List<ThemeDefinition>() { newThemeType2 })
                                    },
                                    Target = interactableObject2,
                                },
                            };

        consecutiveButton.transform.Find("BackPlate").Find("Quad").gameObject.GetComponent<Renderer>().material = bordoScuro;
        consecutiveButton.transform.Find("IconAndText").Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().color = ColorTextDisabled;
    
    }


    public void ChangeColorConsecutive()
    {

        // colori tasto
        Color DefaltCustomColor = new Color(0.8509805f, 0.1686275f, 0.5686275f, 1);
        Color FocusCustomColor = new Color(0.09803922f, 0.1490196f, 0.254902f, 1);
        Color PressedCustomColor = new Color(0.5526878f, 0.582599f, 0.8679245f, 1);

        //Color ColorTextDisabled = new Color(0.2641509f, 0.240477f, 0.240477f, 1);

        Color ColorTextDisabled = Color.white;

        Color DefaltCustomColorDisabled = new Color(0.3215686f, 0.3215686f, 0.3215686f, 1);

        var newThemeType = ThemeDefinition.GetDefaultThemeDefinition<InteractableColorTheme>().Value;
        var interactableObject = consecutiveButton.transform.Find("BackPlate").Find("Quad").gameObject;

        // coloro accesso bottone attivo
                            newThemeType.StateProperties[0].Values = new List<ThemePropertyValue>()
                            {
                                new ThemePropertyValue() { Color = DefaltCustomColor},  // Default ROSA
                                new ThemePropertyValue() { Color = FocusCustomColor}, // Focus
                                new ThemePropertyValue() { Color = PressedCustomColor},   // Pressed
                                new ThemePropertyValue() { Color = Color.black},   // Disabled
                            };


                            consecutiveButton.GetComponent<Interactable>().Profiles = new List<InteractableProfileItem>()
                            {
                                new InteractableProfileItem()
                                {
                                    Themes = new List<Theme>()
                                    {
                                        Interactable.GetDefaultThemeAsset(new List<ThemeDefinition>() { newThemeType })
                                    },
                                    Target = interactableObject,
                                },
                            };


        consecutiveButton.transform.Find("BackPlate").Find("Quad").gameObject.GetComponent<Renderer>().material = bordoChiaro;
        consecutiveButton.transform.Find("IconAndText").Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().color = Color.white;

        // spengo bottone opposto 
        var newThemeType2 = ThemeDefinition.GetDefaultThemeDefinition<InteractableColorTheme>().Value;
        var interactableObject2 = contemporaryButton.transform.Find("BackPlate").Find("Quad").gameObject;

        // Define a color for every state in our Default Interactable States
                            newThemeType2.StateProperties[0].Values = new List<ThemePropertyValue>()
                            {
                                new ThemePropertyValue() { Color = DefaltCustomColorDisabled},  // Default GRIGIO
                                new ThemePropertyValue() { Color = FocusCustomColor}, // Focus
                                new ThemePropertyValue() { Color = PressedCustomColor},   // Pressed
                                new ThemePropertyValue() { Color = Color.black},   // Disabled
                            };


                            contemporaryButton.GetComponent<Interactable>().Profiles = new List<InteractableProfileItem>()
                            {
                                new InteractableProfileItem()
                                {
                                    Themes = new List<Theme>()
                                    {
                                        Interactable.GetDefaultThemeAsset(new List<ThemeDefinition>() { newThemeType2 })
                                    },
                                    Target = interactableObject2,
                                },
                            };

        contemporaryButton.transform.Find("BackPlate").Find("Quad").gameObject.GetComponent<Renderer>().material = bordoScuro;
        contemporaryButton.transform.Find("IconAndText").Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().color = ColorTextDisabled;
        
    }

    */
    // USAGE: 
    /*
     * SE C'è GIA UN PERSONAGGIO ATTIVO LO CAMBIA, IN CASO CONTRARIO ASSEGNA LO SLOT ALL'OBJ CORRENTE ED AVVIA LE PARTICELLE
     */
    public void SetActiveCharacter(GameObject obj) {
        // se il personaggio attivo è gia esistente allora potrei starne selezionando un altro se non c'è allora semplicemente sostituisci con la reference
        if (status == 1) {
            if (activeCharacter != null && activeCharacter != obj && obj.CompareTag("Player"))
            {
                // non so cosa faccia l'instruzione sotto scoprilo 
                characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj); 
                // abilita in modo il pulsante di get Control per far diventare quel personaggio il principale 
              GetControlButton.SetActive(true);
               
            }
            else
            {
                if (obj.CompareTag("Player"))
                {
                    activeCharacter = obj.transform.GetChild(0).gameObject;
                    characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj.transform.GetChild(0).gameObject);
                }
                else
                {
                    characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj.gameObject);
                }
               
                
                // GetControlButton.SetActive(false);
                if (activeCharacter != null && activeCharacter.GetComponent<Animator>() != null)
                {
                    activeCharacterAnimator = activeCharacter.GetComponent<Animator>();
                    DestroyParticles();
                    CreateParticleActive(activeCharacter);
                }
                //sfondoTitolo.SetActive(false);
                //animaPersonaggio.PosizioneTasti();

            }

            
        }

       
    }
    //TODO probabilmente può essere cancellata visto che ho creato un sistema ad eventi per gesitre il movimento
    public void SetupMovement()
    {
       setUpMovement.Invoke(this,new EventArgs());
    }

    public void StartOrStopAnimation()
    {
       var activeChar= activeCharacter.GetComponent<CharacterManager>();
       if (activeChar==null)
       {
           Debug.LogError("non c'è un personaggio attivo selezionato");
            return;    
       }
       
       activeChar.StopOrPlayCharAnimation();
    }
    public void SetActiveCharacterActionClick(GameObject obj)
    {

        if (status == 1)
        {
            if (activeCharacter != null && activeCharacter != obj && obj.CompareTag("Player"))
            {

                characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj);
                GetControlButton.SetActive(true);
              
            }
            else
            {
                activeCharacter = obj;
                characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj);
//                GetControlButton.SetActive(false); not really important at the moment
                if (activeCharacter.GetComponent<Animator>() != null)
                    activeCharacterAnimator = activeCharacter.GetComponent<Animator>();
                DestroyParticles();
                CreateParticleActive(activeCharacter);

                

            }


        }


    }
    /*
    public void getControl()
    {

    //    animaPersonaggio.ChangeWalker();
        activeCharacter.GetComponent<CharacterManager>().StopWalking();
        if (status == 1)
        {
       //     var obj = characterAnimationManager.GetComponent<AnimaPersonaggio>().character;
            activeCharacter = obj;
      //      characterAnimationManager.GetComponent<AnimaPersonaggio>().setCharacter(obj);
            GetControlButton.SetActive(false);
            if (activeCharacter.GetComponent<Animator>() != null)
                activeCharacterAnimator = activeCharacter.GetComponent<Animator>();

            DestroyParticles();
            CreateParticleActive(activeCharacter);
            //sfondoTitolo.SetActive(false);
      //      animaPersonaggio.PosizioneTasti();

            var speed = activeCharacter.GetComponent<Animator>().speed;

            if (speed == 0)
            {

                // cambia testo e icona tasto Play e Stop
        //        animaPersonaggio.buttonConfigHelperStartStop.SetQuadIconByName("IconPlay");
        //        animaPersonaggio.buttonConfigHelperStartStop.MainLabelText = "PLAY";

                
            }
            else
            {

                // cambia testo e icona tasto Play e Stop
         //       animaPersonaggio.buttonConfigHelperStartStop.SetQuadIconByName("IconHandMesh");
         //       animaPersonaggio.buttonConfigHelperStartStop.MainLabelText = "STOP";


            }
        }
            
    }
    */
    public void setActiveObject(GameObject obj)
    {
        if (status == 1)
        {
      //      characterAnimationManager.GetComponent<AnimaPersonaggio>().setCharacter(obj);
            if (activeCharacter.GetComponent<NavMeshAgent>().enabled)
            {
                var q = Quaternion.LookRotation(obj.transform.position - activeCharacter.transform.position);
                activeCharacter.transform.rotation = q;
            }
            GetControlButton.SetActive(false);
        }
           
    }

    //animazioni "attive"
    public void PlayActiveCharacterAnimation(string action)
    {
        if (activeCharacterAnimator != null)
        {
            foreach (AnimationClip ac in activeCharacterAnimator.runtimeAnimatorController.animationClips)
            {
                if (ac.name == action)
                {
                    activeCharacterAnimator.speed = 0.5f;
                    activeCharacterAnimator.Play(ac.name);
                    
                }
            }
            //In assenza di animazioni specifiche per l'azione selezionata, viene eseguita l'animazione generica di interazione
            foreach (AnimationClip ac in activeCharacterAnimator.runtimeAnimatorController.animationClips)
            {
                if (ac.name == "interact")
                {
                    activeCharacterAnimator.Play("interact");
                    return;
                }
            }
        }
        //activeCharacterAnimator.Play("idle");
    }
    
    
    public void CreateParticleActive(GameObject pos)
    {
      
        ParticleActive = Instantiate(feedbackParticleActive);
        ParticleActive.name = "ParticleActive";
        var particles = ParticleActive.GetComponent<ParticleSystem>().shape;
        particles.radius = pos.GetComponent<Collider>().bounds.size.x;
        var position = new Vector3(pos.transform.position.x, pos.transform.position.y + 0.02f, pos.transform.position.z);

        ParticleActive.transform.position = position;
        ParticleActive.GetComponent<ParticleSystem>().Play();
//        ParticleActive.transform.SetParent(particleParent.transform, false); ??

        Debug.Log("Created particle");
       
    }

    public void CreateParticleComplement(GameObject pos)
    {
        GameObject p = Instantiate(feedbackParticleComplement);
        p.name = "ParticleComplement";
  //      var particles = p.GetComponent<ParticleSystem>().shape;
        var position = new Vector3(pos.transform.position.x, pos.transform.position.y + 0.02f, pos.transform.position.z);

     //   p.transform.position = position;
    //    p.GetComponent<ParticleSystem>().Play();
 //       p.transform.SetParent(particleParent.transform, false); ?? 

        Debug.Log("Created particle Complement");
    }

    public void DestroyParticles()
    {
        ParticleSystem[] particles= FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem child in particles)
        {
            GameObject.Destroy(child.gameObject);
        }
        Debug.Log("Destroy all particles");
    }

    public void DestroyParticlesComplement()
    {
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem child in particles)
        {
            if (child.name == "ParticleComplement")
            {
                GameObject.Destroy(child.gameObject);
            }
            
        }

        Debug.Log("Destroy ParticleComplement");
    }

    public void DestroyParticlesActive()
    {
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem child in particles)
        {
            if (child.name == "ParticleActive")
            {
                GameObject.Destroy(child.gameObject);
            }

        }

        Debug.Log("Destroy ParticleActive");
    }

    public void GenerateCondition()
    {
        GetComponent<PhraseGenerator>().GenerateConditionPhrase();
    }


    public void IncrementScreenshotCount()
    {
        screenshotCount++;
        DisplaynumberIllustration();
    }

    public int GetScreenshotCount()
    {
        return screenshotCount;
    }

    public void IncrementScreenshotCountUndo()
    {
        screenshotCountUndo++;
    }

    public int GetScreenshotCountUndo()
    {
        return screenshotCountUndo;
    }

    public void DisplaynumberIllustration()
    {
        int nIllustration;
        nIllustration = 1 + GetScreenshotCount();

        numberIllustration.text = "# " + nIllustration.ToString();
    }
}

