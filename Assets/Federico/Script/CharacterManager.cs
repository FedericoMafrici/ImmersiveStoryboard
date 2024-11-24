
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private PhraseGenerator phraseGenerator;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] public string type;
    [SerializeField] public InteractionManagerAddOn characterInteractionManagerAddOn;
    public bool simulation = false;
    public bool isWalking;
    public bool walkMode = true;
    public List<string> nearObjects;
    public Animator charAnim;
    public bool loadedObject = false;

    private bool primaryHandTriggerPress = false;

    public string lastAction = "";
    public int lastTimeAction = 0;

    public Transform groundCheck;
    public float groundDistance = 0.4f;

    private bool checkDestination = false;

    public string place;
    private bool newPlace;
    public Collider[] hitColliders;

    private GameObject CursorVisual;
    public static EventHandler<EventArgs> OnDestinationReached;
    public bool isBoundingBox=false;

    private Vector3 destination = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        phraseGenerator = FindObjectOfType<PhraseGenerator>();
        simulationManager = FindObjectOfType<SimulationManager>();
        charAnim = this.GetComponent<Animator>();
        if (!loadedObject || type=="")
        {
            type = gameObject.name;
        }
          nearObjects = new List<string>();
     
          place = "ground";

        isWalking = false;
        newPlace = false;
    }

    // Update is called once per frame
    void Update()
    {
        var navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        // Controlla se il personaggio è in movimento
        if (isWalking)
        {
            // Controlla se il percorso non è in sospeso (ovvero, se è stato calcolato)
            if (!navMeshAgent.pathPending)
            {
                // Verifica se la distanza rimanente alla destinazione è inferiore o uguale alla stoppingDistance
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    // Controlla anche se l'agente ha effettivamente raggiunto la destinazione
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        // Il personaggio ha raggiunto la destinazione, ferma il movimento
                        OnDestinationReached?.Invoke(this,EventArgs.Empty);
                        StopWalking();
                       characterInteractionManagerAddOn.characterAnchorManager.AttachObjectToAnchor();
                        //  characterInteractionManagerAddOn.DisableNavMeshAgent();
                    }
                }
            }
        }
    }
   
    public void CheckForward()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        Vector3 startPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);

        //Debug.DrawRay(raycastObject.transform.position, fwd * 50, Color.green);
        if (Physics.Raycast(startPosition + transform.forward * 0.5f, fwd, out hit, 50))
        {
            bool p = false;
            if (hit.transform.gameObject.GetComponent<CharacterManager>() != null)
            { p = hit.transform.gameObject.name == hit.transform.gameObject.GetComponent<CharacterManager>().type; }


            Debug.Log(hit.transform.name);
           
             phraseGenerator.UpdateForward(hit.transform.gameObject.name,p);
        }
      //  else phraseGenerator.UpdateForward("");
    }
    public void EnableCharacterMovement()
    {   
        if (this == null || gameObject == null)
        {
            Debug.LogError("Questo oggetto o il gameObject è stato distrutto.");
            return;
        }

        if (characterInteractionManagerAddOn == null)
        {
            Debug.LogError("characterInteractionManagerAddOn è null.");
            return;
        }
        
        this.walkMode = true;
        
        characterInteractionManagerAddOn.EnableNavMeshAgent(); 
    }
    public void Move(RaycastHit hitInfo)
    {
       
        if (characterInteractionManagerAddOn == null)
        {
            Debug.LogError("L'interactionmanagerAddon collegato al personaggio è risultato nullo");
        }
        characterInteractionManagerAddOn.characterAnchorManager.DetachFromAnchor();
        var agent = this.GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent non trovato sul personaggio");
            return;
        }
    
        agent.speed = 5.0f;  // Valore della velocità
        agent.acceleration = 0.3f;
    
        var result = hitInfo.point;
        var q = Quaternion.LookRotation(result - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, q, 3 * Time.deltaTime);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(result, out hit, 5.0f, NavMesh.AllAreas))
        {
            Debug.Log("Destinazione trovata, inizio a far muovere il personaggio");
            agent.SetDestination(hit.position);
            destination = hit.position;
            this.GetComponent<Animator>().SetBool("walking", true);
            isWalking = true;  // Assicurati che questo valore sia impostato correttamente
            simulationManager.MoveAlert("room","room");
        }
        else
        {
            Debug.LogError("Problema nella sample position, non riesco a trovare una destinazione valida");
        }
    }

    public void StopWalking()
    {
        var agent = this.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;  // Ferma l'agente
            agent.ResetPath();       // Resetta il percorso dell'agente
        }
    
        this.GetComponent<Animator>().SetBool("walking", false);  // Ferma l'animazione
        isWalking = false;
        checkDestination = false;  // Disabilita il controllo sulla destinazione
        destination = Vector3.zero;
        Debug.Log("Personaggio fermato");
        
    }

  
    
    public void StopOrPlayCharAnimation()
    {
        var animator = this.GetComponent<Animator>();
        var agent = this.GetComponent<NavMeshAgent>();

        if (animator.speed != 0)
        {
            Debug.Log("Agent stopped, speed set to 0");
            // Stop the animation
            animator.speed = 0;

            // Stop the NavMeshAgent
            if (agent != null && agent.hasPath)
            {
                Debug.Log("Agent isStopped set to true");
                agent.isStopped = true;        // Stop the agent's movement
                agent.velocity = Vector3.zero; // Immediately halt movement
                 //  agent.ResetPath();             // Clear the current path
            }

            // Disable any animation parameter that may cause movement
            animator.SetBool("walking", false);
        }
        else
        {
            
               // agent.SetDestination(destination);
                Debug.Log("Agent resumes walking, speed set to 1");
                // Restart the animation
                animator.speed = 1;

                // Restart the NavMeshAgent
                if (agent != null)
                {
                    Debug.Log("Agent isStopped set to false");
                    agent.isStopped = false; // Resume agent's movement
                    animator.SetBool("walking", true);
                    // Optionally, you might need to set a new destination here
                }
            
        }
    }
  
    
    
    public void CheckDestination()
    {

        checkDestination = true;
    }
    

    
}
