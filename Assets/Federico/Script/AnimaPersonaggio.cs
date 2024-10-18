using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AnimaPersonaggio : MonoBehaviour
{
    [Header("Manager")]
    private String[] _selectedActionsList = null; // lista che contiene tutte le azioni eseguibili dall'oggetto che si è selezionato 
    [SerializeField] private ActionsDataBase _actionsDB; // Database con la lista delle azioni per ogni oggetto
    [SerializeField] private SimulationManager _simulationManager;
   
    [Header("Personaggio ed oggetti attivi")] 
    [SerializeField] public GameObject _character;
    [SerializeField] public GameObject _interactionObject;
    
    [Header("Personaggio ed oggetti attivi")] 
    [SerializeField] public PhraseGenerator phraseGenerator;
    
    [Header("Variabili Booleane")]
    // Gestione camminata 
    [SerializeField] private bool _self;
    public bool ActiveWalk = true;
    public bool isWalking = false;
    [Header("UI ELEMENT")]
    // Elementi della UI
    [SerializeField] private GameObject _ScrollviewElementPrefab;

    [SerializeField] private GameObject ObjectMenu;
    

   
    
    
    // Start is called before the first frame update
    void Start()
    {
      
        if (transform.parent != null)
        {
            Debug.Log("Parent: " + transform.parent.gameObject.name);
        }
        else
        {
            Debug.Log("No parent found.");
        }
        phraseGenerator = FindObjectOfType<PhraseGenerator>();
        if(phraseGenerator==null)
            Debug.LogError("Phrase Generator di Anima personaggio non trovato +"+ gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (_simulationManager.activeCharacter != null )
        {
            if ((_simulationManager.activeCharacter.GetComponent<State>().GetCurrentState() == "sitting" ||
                 _simulationManager.activeCharacter.GetComponent<State>().GetCurrentState() == "playing") &&
                _simulationManager.status == 1)
            {

                _simulationManager.activeCharacter.GetComponent<CharacterManager>().StopWalking();
                ChangeWalker();

            }
        }

    }
    /*
     *  Selezione dell'oggetto ad attivo 
     */
    public void SetCharacter(GameObject obj)
    {
        // null check section
        if (obj == null)
        {
            Debug.LogError("Errore nella set Character, l'obj è nullo");
            return;
        }

        _character = obj; //preleviamo il gameobject dal figlio
       
       
       //TODO  qui si potrebbe inserire la funzionalità per la casella del nome, per ora non è necessario
       
       _simulationManager.DestroyParticlesComplement();
       _simulationManager.CreateParticleComplement(_character);
   

       if (obj.CompareTag("Player") && _simulationManager.activeCharacter == obj)
       { 
           if (GameObject.Find("ParticleActive") == false)
           { _simulationManager.CreateParticleActive(_simulationManager.activeCharacter); }
           _self = true; // per ora lo mettimao non sono sicuro serva a qualcosa 
           ShowAloneActions();
           return; 
       }
        // se non è un personaggio attivo allora ho selezionato un oggetto 
        _interactionObject = obj;

       if (_simulationManager.activeCharacter != null && _simulationManager.activeCharacter != _character)
       {
           _self = false;
           ShowActions();
       }
    }

    private void ShowAloneActions()
    {
        Debug.Log("Entered in ShowAloneActions");
        
        // recupero le azioni possibili dell'oggeto ed attivo la UI

        var ui =  _simulationManager.activeCharacter.transform.parent.Find("CharacterUI").gameObject;
        if (ui == null)
        {
            Debug.LogError("ui del personaggio non trovata personaggio: "+ this.gameObject.name);
        }
        ui.SetActive(true);
        var tmp = ui.transform.Find("Front");
        ui= _simulationManager.activeCharacter.transform.parent.Find("PersonaggioAttivo").gameObject;
        ui.SetActive(true);
        
        var container = ui.transform.Find("Front/Scrollview Canvas/Panel/Pannello/Viewport/Content");
        if (container == null)
        {
            Debug.LogError("Null reference exception nella gestione della ui dell'oggetto " + _character.name);
        }
        
        
        _selectedActionsList =
            _actionsDB.ReturnActions(_simulationManager.activeCharacter.GetComponent<CharacterManager>().type,_character.GetComponent<CharacterManager>().type,
                _character.GetComponent<State>().state, _self);
        if (_selectedActionsList == null)
        {
            Debug.LogError("ACTIONS NOT FOUND FOR OBJECT:"+ _character.name);
        }
        // personalizzazione della UI in base al numero di azioni possibili dell'utente prima si pulisce il contenuto precedente
        ClearContainer(container);
        foreach (string s in _selectedActionsList)
        {
            if( s==null)
            return;

          var newElement= CreateElement(s,container);
          
            // TODO possibilità di cambiare colore in base ai bottoni ma si vede dopo 
        }
        
    }
    
    

    // Funzione per pulire il container
    
    public void ShowActions()
    {
        // abilita la UI
        var ui =  _simulationManager.activeCharacter.transform.parent.Find("CharacterUI").gameObject;
        if (ui == null)
        {
            Debug.LogError("ui del personaggio non trovata personaggio: "+ this.gameObject.name);
        }
        ui.SetActive(true);
       // var tmp = ui.transform.Find("Front");
       // ui.transform.localPosition = new Vector3(2.78f, -0.78f, 1.07f);
        ui= _simulationManager.activeCharacter.transform.parent.Find("PersonaggioAttivo").gameObject;
        ui.SetActive(true);
        var container = ui.transform.Find("Front/Scrollview Canvas/Panel/Pannello/Viewport/Content");
        if (container == null)
        {
            Debug.LogError("Null reference exception nella gestione della ui dell'oggetto " + _character.name);
        }
        _selectedActionsList =
            _actionsDB.ReturnActions(_simulationManager.activeCharacter.GetComponent<CharacterManager>().type,_character.GetComponent<CharacterManager>().type,
                _character.GetComponent<State>().state, _self);
        ClearContainer(container);
        // update dei contenuti 
        foreach (string s in _selectedActionsList)
        {
            if( s==null)
                return;

            var newElement= CreateElement(s,container);
            // TODO possibilità di cambiare colore in base ai bottoni ma si vee dopo 
        }
        
    }

    
      public void StopOldLongAnimation()
    {
        if (!_simulationManager.contemporaryAction)
        {
            //string ultimaAzione = simulationManager.activeCharacter.GetComponent<CharacterManager>().lastAction;

            if (_simulationManager.activeCharacter.GetComponent<State>().GetCurrentState() == "working out")
            {
                string azione = "stop work out";
                _simulationManager.activeCharacter.GetComponent<State>().ChangeState(azione);
                _simulationManager.SetActiveCharacterActionClick(_simulationManager.activeCharacter);

            }
            else if (_simulationManager.activeCharacter.GetComponent<State>().GetCurrentState() == "dancing")
            {
                string azione = "stop dance";
                _simulationManager.activeCharacter.GetComponent<State>().ChangeState(azione);
                _simulationManager.SetActiveCharacterActionClick(_simulationManager.activeCharacter);

            }
            else if (_simulationManager.activeCharacter.GetComponent<State>().GetCurrentState() == "playing")
            {
                string azione = "stop play";
                _simulationManager.activeCharacter.GetComponent<State>().ChangeState(azione);
                _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = true;
                _simulationManager.SetActiveCharacterActionClick(_simulationManager.activeCharacter);

            }
            else if (_simulationManager.activeCharacter.GetComponent<State>().GetCurrentState() == "sitting")
            {
                string azione = "stand up";
                _simulationManager.activeCharacter.GetComponent<State>().ChangeState(azione);
                _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = true;
                _simulationManager.activeCharacter.transform.Translate(0, 0, +0.05f);
                _simulationManager.SetActiveCharacterActionClick(_simulationManager.activeCharacter);

            }

        }
    }
    
    public void ActivateWalkMode(bool walk)
    {
        Color DefaltCustomColor;

        if (ActiveWalk)
        {
            ActiveWalk = false;
            DefaltCustomColor = new Color(0.682353f, 0.2666667f, 0.3529412f, 1); //granata leggero AE445A
           
            isWalking = false;
           
        }
        else
        {
            DefaltCustomColor = new Color(0.5526878f, 0.582599f, 0.8679245f, 1);
            ActiveWalk = true;
            StopOldLongAnimation();


        }

        // UI UPDATE?

        if (_simulationManager.activeCharacter != null)
        {
            _simulationManager.activeCharacter.GetComponent<CharacterManager>().isWalking = true;
        }
        WalkMode(walk);
    }
    
    public void ChangeWalker()
    {
        if (ActiveWalk)
            ActivateWalkMode(true);
        
    }
    
     public void ActionClick(string action)
    {
        WalkMode(false);

        StopOldLongAnimation();

        _simulationManager.activeCharacter.GetComponent<CharacterManager>().lastAction = action;
        _simulationManager.activeCharacter.GetComponent<CharacterManager>().lastTimeAction = _simulationManager.GetScreenshotCount();

       

        //  TODO cambia testo e icona tasto Play e Stop
        //  buttonConfigHelperStartStop.SetQuadIconByName("IconHandMesh");
        //  buttonConfigHelperStartStop.MainLabelText = "STOP";

        if (action == "smile")
        {
            _simulationManager.activeCharacter.GetComponent<Animator>().speed = 0.5f;
        }

        // per geenrare il get control button
        if (_character.CompareTag("Player") && _simulationManager.activeCharacter.CompareTag("Player") && _character != _simulationManager.activeCharacter)
        {
            var q = Quaternion.LookRotation(_character.transform.position - _simulationManager.activeCharacter.transform.position);
            _simulationManager.activeCharacter.transform.rotation = q;

            var r = Quaternion.LookRotation(_simulationManager.activeCharacter.transform.position - _character.transform.position);
            _character.transform.rotation = r;
        }
       

        if (action == "talk" || action == "talk to")
        {
            //TODO GENERATORE DI FRASI DA IMPLEMENTARE 
            phraseGenerator.StartSpeech();
           // SetKeyboardForDictaction(true);
         

            //distruggi particelle
            _simulationManager.DestroyParticlesActive();
            //crea particlle
            _simulationManager.CreateParticleActive(_simulationManager.activeCharacter);
        }

        else
        {

            //notifica lo state di avviare l'eventuale animazione dell'oggetto che subisce l'azione
            _character.GetComponent<State>().PlayAnimation(action);

            //notifica il simulation manager di avviare animazione del personaggio attivo
            _simulationManager.PlayActiveCharacterAnimation(action);

            //TODO Genera la frase
             phraseGenerator.GenerateSimplePhrase(_simulationManager.activeCharacter.name, _simulationManager.activeCharacter.GetComponent<CharacterManager>().type, action, _character.name, _character.GetComponent<CharacterManager>().type, _self);

            

            if (action != "sit" && action != "stand up" && action != "play" && action != "stop play" && action != "dance" && action != "stop dance" && action != "work out" && action != "stop work out")
            {
                //notifica lo State del gameobject la cui azione � stata cliccata per effettuare un controllo di cambio di stato
                _character.GetComponent<State>().ChangeState(action);

            }
            else
            {

                _simulationManager.activeCharacter.GetComponent<State>().ChangeState(action);

            }

            //pick & place
            if (action == "pick")
            {
                _character.transform.parent = _simulationManager.activeCharacter.transform;
                //selectedObject.transform.position = new Vector3(simulationManager.activeCharacter.transform.position.x + 1f, selectedObject.transform.position.y + 0.5f, simulationManager.activeCharacter.transform.localPosition.z);
                _character.transform.position = _simulationManager.activeCharacter.transform.position + _simulationManager.activeCharacter.transform.forward * 1f; ;
            }
            else if (action == "place")
            {
                _character.transform.parent = GameObject.Find("BuildingManager").transform;
                _character.transform.position = new Vector3(_character.transform.position.x, _character.transform.position.y, _character.transform.position.z);
            }
            else if (action == "sit")
            {
                _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = false;

                if (_character.GetComponent<CharacterManager>().type == "bench")
                {
                    _simulationManager.activeCharacter.transform.position = new Vector3(_character.transform.position.x, _character.transform.position.y - 0.005f, _character.transform.position.z - 0.035f);
                }

                if (_character.GetComponent<CharacterManager>().type == "chair")
                {
                    // siedi sulla sedia:
                    SitOnTheChair();
                }
                _simulationManager.activeCharacter.transform.localRotation = Quaternion.Euler(_character.transform.localRotation.eulerAngles);
            }
            else if (action == "stand up" )
            {
                _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = true;
                _simulationManager.activeCharacter.transform.Translate(0,0, +0.05f);

            }

            else if (action == "stop play")
            {

                if (_interactionObject == null)
                {
                    Debug.LogError("L'OGGETTO CON CUI SI INTERAGISCE è NULLO");
                }
                /*
                XRGrabInteractable _xrInt =_interactionObject.GetComponent<XRGrabInteractable>();
                if (_xrInt == null)
                {
                    Debug.LogError("errore nel reperire il grabbable component di "+ gameObject.name);
                }   
                
                
                _xrInt.interactionLayers = LayerMask.GetMask("Default");
                Debug.Log("ho ripristinato il layer mask al default oggetto: "+_interactionObject.name + _xrInt.interactionLayers.ToString());
                */
                InteractionManagerAddOn interaction = _interactionObject.GetComponent<InteractionManagerAddOn>();
                if (interaction == null)
                {
                    Debug.LogError("errore nel reperire il grabbable component di "+ _interactionObject.name);
                }
                else
                {
                    interaction.interactionEnabled = true;
                }
                _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = true;
                
                _interactionObject = null;
            }

            else if (action == "play")
            {
                // interaction object 
                var rbobj = _interactionObject.GetComponent<Rigidbody>();
                if (rbobj != null)
                {
                    rbobj.isKinematic = true;
                }
                // active character rigid body disabilitato
                var rb =_simulationManager.activeCharacter.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    
                }
                else
                {
                    Debug.LogError("Errore rigid body non trovato per l'oggetto: " + gameObject.name);
                }
                _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = false;
                _simulationManager.activeCharacter.transform.localRotation = Quaternion.Euler(_character.transform.localRotation.eulerAngles.x,  _character.transform.localRotation.eulerAngles.y + 180f , _character.transform.localRotation.eulerAngles.z);
                var bench = _character.transform.Find("PianoBench/spawnPoint").transform;
                if (bench == null)
                {
                    Debug.LogError("errore panchina non trovata");
                }
                _simulationManager.activeCharacter.transform.position = new Vector3(bench.position.x , bench.position.y ,bench.position.z ); //+0.1f
            }

            //

            //HideActions();

            if (action == "work out" || action == "dance" || action == "play" || action == "sit" || action == "stop dance" || action == "stop work out" || action == "stop play" || action == "stand up")
            {
                _simulationManager.SetActiveCharacterActionClick(_simulationManager.activeCharacter);
            }
            else
            {
                //distruggi particelle
               _simulationManager.DestroyParticlesActive();
                //crea particlle
                _simulationManager.CreateParticleActive(_simulationManager.activeCharacter);
            }



            Debug.Log("Action: " + action);
            

        }

        ChangeWalker();

        if (!_simulationManager.contemporaryAction)
        {
            _simulationManager.activeCharacter.GetComponent<CharacterManager>().StopWalking();
        }

         phraseGenerator.AggiornaTesto();
    }

  /*   
    public void ResetDestination()
    {
        foreach (GameObject obj in objectManager.objectsInScene) {
            if (obj.CompareTag("Player")) {
                var navMeshAgent = obj.GetComponent<NavMeshAgent>();
                if(navMeshAgent!= null && navMeshAgent.isActiveAndEnabled)
                    navMeshAgent.ResetPath();
            }
        }
        
    }
   */
  public void SitOnTheChair()
  {
      // interaction object 
      var rbobj = _interactionObject.GetComponent<Rigidbody>();
      if (rbobj != null)
      {
          rbobj.isKinematic = true;
      }
      // active character rigid body disabilitato
      var rb =_simulationManager.activeCharacter.GetComponent<Rigidbody>();
      if (rb != null)
      {
          rb.useGravity = false;
          rb.isKinematic = true;
                    
      }
      else
      {
          Debug.LogError("Errore rigid body non trovato per l'oggetto: " + gameObject.name);
      }
      _simulationManager.activeCharacter.GetComponent<NavMeshAgent>().enabled = false;
      _simulationManager.activeCharacter.transform.localRotation = Quaternion.Euler(_character.transform.localRotation.eulerAngles.x,  _character.transform.localRotation.eulerAngles.y + 180f , _character.transform.localRotation.eulerAngles.z);
      var bench = _character.transform.Find("spawnPoint").transform;
      if (bench == null)
      {
          Debug.LogError("errore panchina non trovata");
      }
      _simulationManager.activeCharacter.transform.position = new Vector3(bench.position.x , bench.position.y-0.3f ,bench.position.z ); //+0.1f
      
  }
    public void WalkMode(bool walk)
    {
        if (ActiveWalk)
        {
            isWalking = walk;
            if (_simulationManager.activeCharacter != null)
            {
                _simulationManager.activeCharacter.GetComponent<CharacterManager>().isWalking = true; // ?????? 
               
            }
        }
        

    }
    
    private void ClearContainer(Transform container)
    {
        // Distrugge tutti i figli del container
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    // Funzione di utilità: 
    GameObject CreateElement(string text,Transform container)
    {
        // Clona l'elemento prefab
        GameObject newElement = Instantiate(_ScrollviewElementPrefab, container);

        // Ottieni il componente Text (o TextMeshPro) e imposta il testo
        Text elementText = newElement.GetComponentInChildren<Text>();
        if (elementText != null)
        {
            elementText.text = text;
        }
        else
        {
            Debug.LogError("Il prefab non contiene un componente Text.");
        }

        return newElement;
    }
    
    /*
     * ABILITA LA USER INTERFACE DELL'OGGETTO
     */
    
}
