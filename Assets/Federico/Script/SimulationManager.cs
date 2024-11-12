using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.AI;
using System;
using System.Linq;
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
    public bool contemporaryAction = true;
    public int actionTime = 0;
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
    [SerializeField]  private GameObject MenuOggetti;

    [SerializeField] private ConsoleDebugger consoleDebuggin;

    [SerializeField] private GameObject ScreenshotPanel;
     
    
    [Header("Componenti UI ")]
    
    [SerializeField]  private Toggle toggleContemporaryAction;
    
    
    
    
    
    
    
    [Header("Componenti inutili che la collega prima di me ha lasciato di cui ancora non conosco l'utilità")]

 //   public ObjectManagerVR objectManagerVR;
    private GameObject canvasNewLight;

  //  public CameraManager cameraManager;
   
  

    public GameObject ParticleActive;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject StopAndPlayButton;
    

    public GameObject light;
    private GameObject newLight;
    //private int loopCount = 0;
    public int screenshotCount = 0;
    private int screenshotCountUndo = 0;
    
    public bool firstAction = true;
    

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
    void Start()
    {
      BoundingBoxManagerUI.OnSceneInizializationCompleted+=ChangeSimulationState;
      ValueVisualizer.onActionTimeChanged += SetTime;
      ScreenshotManager.screenShotTaken += CompletedAction;
     // Debug.Log("Storyboarding Avviato");
     // DebuggingStartStoryBoarding();
        
    }

    public void  Update()
    {
        
    }
    public void DebuggingStartStoryBoarding( )
    {
        if (status == 0)
        {
            status = 1;
            startStoryboarding.Invoke(this, new EventArgs());
            
        }
        else
        {
            status = 0;
            pauseStoryboarding.Invoke(this, new EventArgs());
          
        }
    }
    
    public void StartStoryBoarding(TMP_Text txtcomponent)
    {
        consoleDebuggin.SetText("starStoryboarding Chiamata"+ status);
        Debug.Log("Start Storyboarding avviato");
        if (status == 0)
        {
            status = 1;
            startStoryboarding?.Invoke(this, new EventArgs());
            txtcomponent.text = "Stop";
        }
        else
        {
            status = 0;
            pauseStoryboarding?.Invoke(this, new EventArgs());
            txtcomponent.text = "Start";
        }
    }
   
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
    
        /*
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
  /*
        moveButton.transform.SetParent(ManuAzioniTasti.transform);
        StopAndPlayButton.transform.SetParent(StopAndPlayButtonPadre.transform);
        actionsPanel.transform.SetParent(ManuAzioniTasti.transform);
        SfondoAzioni.transform.SetParent(StopAndPlayButtonPadre.transform);
    }
*/
    

    public void ChangeMaxTime() { }
    public void ChangeCurrentTime() { }
    public void ChangeCurrentTimeManual() { }
    public void SpendTime() { }
    public void SetTime(object e, ActionTimeChangedEventArgs args)
    {
        actionTime = (int) args.NewValue;
    }
    public int GetTime() 
    {
        
      return actionTime ; //TODO da rimuovere e ripristinare con quello sopra
    }
    public void ControlCamera(){ }

    public void SetContemporaryAction()
    {
        contemporaryAction = !contemporaryAction;
        toggleContemporaryAction.isOn = contemporaryAction;
        var txt=   toggleContemporaryAction.transform.Find("Image/Text").GetComponent<Text>();
        if (contemporaryAction)
        {
            txt.text = "attivata";
        }
        else
        { 
            txt.text = "disattivata";
        }
        characterAnimationManager.ChangeWalker(); 
    }

    //FUNZIONI USATE DA FEDERICO ( QUELLE SOPRA SONO STATE REIMPORTATE DAGLI SCRIPT DELLE COLLEGHE//
    /*
     * SE QUALCUNO DOPO DI ME LAVORERà ALLA TESI VI CONSIGLIO DI FARVI MANDARE LA TESI DELLE COLLEGHE PRECEDENTI DAI PROF
     * E POI LEGGERE LA MIA
     * 
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
           
                //TODO da eseguire se il personaggio deve essere selezionato come nuovo personaggio attivo
                // non so cosa faccia l'instruzione sotto scoprilo 
                //activeCharacter = obj.transform.GetChild(0).gameObject;
                //characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(activeCharacter); 
                // abilita in modo il pulsante di get Control per far diventare quel personaggio il principale 
                
              //   activeCharacter = obj.transform.GetChild(0).gameObject;
                characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj.transform.GetChild(0).gameObject); 
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

    public void getControlOfsecondCharacter()
    {
        // 
        characterAnimationManager.GetComponent<AnimaPersonaggio>().HideActions();
        activeCharacter=characterAnimationManager.GetComponent<AnimaPersonaggio>()._character;
       characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(activeCharacter);
    }
    
    //TODO probabilmente può essere cancellata visto che ho creato un sistema ad eventi per gesitre il movimento
    public void SetupMovement()
    {
       setUpMovement.Invoke(this,new EventArgs());
    }

    public void MoveAlert(string place, string placeType)
    {
        phraseGenerator.GenerateMovementPhrase(activeCharacter.name, activeCharacter.GetComponent<CharacterManager>().type, "room", "room");
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
        if(screenshotCount>0)
        screenshotCountUndo= screenshotCount-1;
        
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
    // GESTIONE SIMULAZIONE: 

    public void ChangeSimulationState(object sender, EventArgs e)
    {
        MenuOggetti.SetActive(true);
        ScreenshotPanel.SetActive(true);
        status = 0;
        Debug.Log("Cambio stato simulazione avvenuto con successo");
    }
    public void StopAll()
    {
     
        foreach (GameObject o in spawnedGameObjects)
        {
            if (o.GetComponent<Animator>() != null)
            {
                o.GetComponent<Animator>().speed = 0;
                
                if (o.GetComponent<NavMeshAgent>() != null && o.GetComponent<NavMeshAgent>().enabled)
                {
                    o.GetComponent<NavMeshAgent>().Stop();
                }
            }
        }
    }
    public void CompletedAction(object sender,EventArgs e)
    {
        Debug.Log("Completed Action Chiamata");
        //salvare tutte le azioni che sono state completate 
        if (firstAction)
            firstAction = false;
        
        GameObject[] objects = new GameObject[spawnedGameObjects.Count];
        spawnedGameObjects.CopyTo(objects);
        //ogni screenshot � un azione che salvo

        string basePath = Path.Combine(Application.persistentDataPath, "screenshotActions.csv");
        // Controlla se il file esiste
        if (!File.Exists(basePath))
        {
            // Se il file non esiste, crealo
            using (var writer = new StreamWriter(basePath))
            {
                // Scrivi eventualmente un'intestazione o lascia vuoto
                writer.WriteLine("");  // Crea un file vuoto o metti un'intestazione se necessario
            }
        }

        
        var backup = new List<string>();
        backup.Clear();
        using (var reader = new StreamReader(basePath))
        {
            while (!reader.EndOfStream)
            {
                backup.Add(reader.ReadLine());
            }
        }

        using (var writer = new StreamWriter(basePath))
        {
            string line = "action" + GetScreenshotCountUndo() + "|";

            GameObject obj;
            foreach (GameObject o in objects)
            {
                if (o.CompareTag("Player"))
                {
                    obj = o.transform.GetChild(0).gameObject;
                }
                else
                {
                    obj = o;
                }
                
                
                line += obj.GetInstanceID() + ";" + obj.transform.localPosition.x + ";" + obj.transform.localPosition.y + ";" + obj.transform.localPosition.z + ";"
                    + obj.transform.rotation.eulerAngles.y + ";";
                
              
                if (obj.GetComponent<State>().state.Count != 0)
                {
                    line += obj.GetComponent<State>().state[0] + ";";
                }
                else { line += "none" + ";"; }

                if (obj.CompareTag("Player") && obj.GetComponent<CharacterManager>() != null && obj.GetComponent<CharacterManager>().lastTimeAction == GetScreenshotCountUndo())
                {
                    line += obj.GetComponent<CharacterManager>().lastAction;
                }
                else { line += "none"; }

                line += "|";
                ;
            }
            writer.WriteLine(line);
            foreach (string s in backup)
            {
                writer.WriteLine(s);
            }
        }
    }
    public void UndoLastAction()
         {
             
             if (screenshotCount > 0)
             {
                 characterAnimationManager = FindObjectOfType<AnimaPersonaggio>();
               
                 string basePath = Path.Combine(Application.persistentDataPath, "screenshotActions.csv");
     
     
                 characterAnimationManager.WalkMode(false);
                 ResetDestination();
                 StopAll(); 
                 List<int> cameraList2 = new List<int>();
     
                 using (var reader = new StreamReader(basePath))
                 {
                     while (!reader.EndOfStream)
                     {
                         var line = reader.ReadLine();
     
                         if (line.Split('|')[0] == "action" + (GetScreenshotCountUndo()))
                         {
                             //istanzia ogni oggetto della linea
                             var objs = line.Split('|');
                            objs[0]=objs[0].Replace("action", "");
                             var i = 0;
     
                             foreach (string previousAction in objs.Skip(1))
                             {
                                 var parts = previousAction.Split(';');
         
                                 if (parts.Length < 7) {
                                     Debug.LogWarning("La riga non ha il numero corretto di campi: " + previousAction);
                                     continue;  // Salta questa riga
                                 }
     
                                 int objectID;
                                 if (!int.TryParse(parts[0] , out objectID)) {
                                     Debug.LogError("Errore di parsing dell'objectID: " + parts[0]);
                                     continue;
                                 }
                                 
     
                                 float x, y, z, r;
                                 if (!float.TryParse(parts[1], out x) ||
                                     !float.TryParse(parts[2], out y) ||
                                     !float.TryParse(parts[3], out z) ||
                                     !float.TryParse(parts[4], out r)) {
                                     Debug.LogError("Errore di parsing delle coordinate o della rotazione: " + previousAction);
                                     continue;
                                 }
     
                                 string state = parts[5];
                                 string action = parts[6];
     
                                 GameObject obj = null;
                                     foreach (GameObject o in spawnedGameObjects)
                                     {
                                         if (o.CompareTag("Player"))
                                         {
                                             obj = o.transform.GetChild(0).gameObject;
                                         }
                                         else
                                         {
                                             obj = o;
                                         }
                                         
                                         if (obj.GetInstanceID() == objectID)
                                         {
                                             obj.transform.localPosition = new Vector3(x, y, z);
                                             obj.transform.rotation = Quaternion.Euler(0, r, 0);
                                             if (state != "none") obj.GetComponent<State>().SetState(state, action);
                                         }
                                     }
     
                                
                                 i++;
                             }
                         }
     
                     }
                 }
     
             //    PlayAll();
     
                 SetActiveCharacterActionClick(activeCharacter);
     
                
                 DeleteSentencesUndo();
                 phraseGenerator.AggiornaTesto();
     
             }
         }

    public void ResetDestination()
    {
        foreach (GameObject obj in spawnedGameObjects)
        {
            if (obj.CompareTag("Player"))
            {
                var navMeshAgent = obj.GetComponent<NavMeshAgent>();
                if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
                    navMeshAgent.ResetPath();
            }
        }
    }

    public void PlayAll()
    {
       
        foreach (GameObject o in spawnedGameObjects)
        {
            if (o.GetComponent<Animator>() != null )
            {
                o.GetComponent<Animator>().speed = 0.5f;
                
                if (o.GetComponent<NavMeshAgent>() != null && o.GetComponent<NavMeshAgent>().enabled)
                {
                    o.GetComponent<NavMeshAgent>().Resume();
                }
            }
        }
    }
    
   

    public List<string> DeleteSentencesUndo()
    {
        buffer = GetComponent<PhraseGenerator>().ClearBuffer();
        List<string> sentencesToRemove = new List<string>();

        foreach (string b in buffer)
        {
            if (Convert.ToInt32(b.Split('_')[2]) == screenshotCountUndo)
            {
                sentencesToRemove.Add(b);
            }
        }

        foreach (string sentence in sentencesToRemove)
        {
            buffer.Remove(sentence);
        }

        return buffer;
    }

}

