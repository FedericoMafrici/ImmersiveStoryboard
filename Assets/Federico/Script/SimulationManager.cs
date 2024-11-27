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
using Unity.XR.CoreUtils;
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

    [SerializeField] private Toggle toggleSaveStoryboard;

    [SerializeField] private Toggle passtrough;

    private bool usingToggle = true;
    
    
    
    
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
    [SerializeField] private TextMeshPro numberIllustration = null;
    public GameObject feedbackParticleActive;
    public GameObject feedbackParticleComplement;
    [SerializeField] GameObject _pannelloFine;
    private List<string> buffer;
    void Start()
    {
      BoundingBoxManagerUI.OnSceneInizializationCompleted+=ChangeSimulationState;
      ValueVisualizer.onActionTimeChanged += SetTime;
      ScreenshotManager.screenShotTaken += CompletedAction;
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
        consoleDebuggin.SetText("starStoryboarding Chiamata" + status);
        Debug.Log("Start Storyboarding avviato");

        if (txtcomponent == null)
        {
            Debug.LogError("txtcomponent è null! Assicurati di passarlo correttamente.");
            return;
        }


        if (status == 0)
        {
            status = 1;
            startStoryboarding?.Invoke(this, new EventArgs());
            txtcomponent.text = "Stop";
            consoleDebuggin.SetText("Storyboarding Avviato" + status);
            Debug.Log("Start Storyboarding avviato personaggio attivo" + activeCharacter);
            if (activeCharacter != null)
            {
                activeCharacter.GetComponent<AnimaPersonaggio>().HideActions();
            }

            activeCharacter = null;
            characterAnimationManager._character = null;
            characterAnimationManager._interactionObject = null;
            consoleDebuggin.SetText("Storyboarding fermato" + status);
            Debug.Log("Start Storyboarding fermato");

        }
        else
            {
                status = 0;
                pauseStoryboarding?.Invoke(this, new EventArgs());
                txtcomponent.text = "Start";
                if (activeCharacter != null)
                {
                    characterAnimationManager.HideActions();
                    DestroyParticles();
                    DestroyParticlesActive();
                    DestroyParticlesComplement();
                }

                activeCharacter = null;
                characterAnimationManager._character = null;
                characterAnimationManager._interactionObject = null;
                consoleDebuggin.SetText("Storyboarding fermato" + status);
                Debug.Log("Start Storyboarding fermato");
            }

            if (status == 0)
            {
                status = 1;
                startStoryboarding?.Invoke(this, new EventArgs());
                toggleSaveStoryboard.isOn = true;
                consoleDebuggin.SetText("Storyboarding Avviato" + status);
                Debug.Log("Start Storyboarding avviato personaggio attivo" + activeCharacter);
                if (activeCharacter != null)
                {
                    activeCharacter.GetComponent<AnimaPersonaggio>().HideActions();
                }

                activeCharacter = null;
                characterAnimationManager._character = null;
                characterAnimationManager._interactionObject = null;
                consoleDebuggin.SetText("Storyboarding fermato" + status);
                Debug.Log("Start Storyboarding fermato");

            }
            else
            {
                status = 0;
                pauseStoryboarding?.Invoke(this, new EventArgs());
                toggleSaveStoryboard.isOn = true;
                if (activeCharacter != null)
                {
                    characterAnimationManager.HideActions();
                    DestroyParticles();
                    DestroyParticlesActive();
                    DestroyParticlesComplement();
                }

                activeCharacter = null;
                characterAnimationManager._character = null;
                characterAnimationManager._interactionObject = null;
                consoleDebuggin.SetText("Storyboarding fermato" + status);
                Debug.Log("Start Storyboarding fermato");
            }
    }
    

    public void StartStoryBoardingForToggle()
    {
        consoleDebuggin.SetText("starStoryboarding Chiamata"+ status);
        Debug.Log("Start Storyboarding avviato");
            if (status == 0)
            {
                status = 1;
                startStoryboarding?.Invoke(this, new EventArgs());
               // toggleSaveStoryboard.isOn = true;
                consoleDebuggin.SetText("Storyboarding Avviato" + status);
                Debug.Log("Start Storyboarding avviato personaggio attivo" + activeCharacter);
                if (activeCharacter != null)
                {
                    activeCharacter.GetComponent<AnimaPersonaggio>().HideActions();
                }

                activeCharacter = null;
                characterAnimationManager._character = null;
                characterAnimationManager._interactionObject = null;
                consoleDebuggin.SetText("Storyboarding fermato" + status);
                Debug.Log("Start Storyboarding fermato");

            }
            else
            {
                status = 0;
                pauseStoryboarding?.Invoke(this, new EventArgs());
              //  toggleSaveStoryboard.isOn = false;
                if (activeCharacter != null)
                {
                    characterAnimationManager.HideActions();
                    DestroyParticles();
                    DestroyParticlesActive();
                    DestroyParticlesComplement();
                }
                activeCharacter = null;
                characterAnimationManager._character = null;
                characterAnimationManager._interactionObject = null;
                consoleDebuggin.SetText("Storyboarding fermato" + status);
                Debug.Log("Start Storyboarding fermato");
            }
        }
    
    
    public void changePasstroughToggle()
    {
        if (passtrough.isOn)
        {
            passtrough.isOn = false;
        }
        else
        {
            passtrough.isOn = true;
        }
    }


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
        consoleDebuggin.SetText("Simulation Manager r326: " + obj.name+ "TAG"+ obj.tag);
            if (status == 1) {
            if (activeCharacter != null && activeCharacter != obj && obj.CompareTag("Player"))
            {
                consoleDebuggin.SetText(activeCharacter.name +" oggetto:"+ obj.name);
              characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj.transform.GetChild(0).gameObject); 
            }
            else
            {
                if (obj.CompareTag("Player"))
                {
                    activeCharacter = obj.transform.GetChild(0).gameObject;
                    consoleDebuggin.SetText(activeCharacter.name +" oggetto:"+ obj.name);
                    characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(obj.transform.GetChild(0).gameObject);
                }
                else
                {
                    //consoleDebuggin.SetText(activeCharacter.name +" oggetto:"+ obj.name);
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
        Debug.Log("Personaggio attivo"+characterAnimationManager.GetComponent<AnimaPersonaggio>()._character);
        characterAnimationManager.GetComponent<AnimaPersonaggio>().HideActions();
        activeCharacter=characterAnimationManager.GetComponent<AnimaPersonaggio>()._character;
        activeCharacterAnimator = activeCharacter.GetComponent<Animator>();
        consoleDebuggin.SetText("animatore impostato "+activeCharacterAnimator.name);
       characterAnimationManager.GetComponent<AnimaPersonaggio>().SetCharacter(activeCharacter);
    }
    
    public void SetupMovement()
    {
       //setUpMovement.Invoke(this,new EventArgs());
       var charManager = activeCharacter.GetComponent<CharacterManager>();
       if (charManager != null)
       {
           charManager.EnableCharacterMovement();
       }
       else
       {
           Debug.LogError(" r313 Componenete char manager non trovato, nome personaggio"+activeCharacter.name);
       }
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
            if (activeCharacter != null && activeCharacter == obj && obj.CompareTag("Player"))
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
                consoleDebuggin.SetText("Azione lanciata "+ action + " personaggio "+ activeCharacter.name +" animator:"+activeCharacterAnimator.name);
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
             else
             {
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

