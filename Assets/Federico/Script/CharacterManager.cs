
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : MonoBehaviour
{
   /* TODO quando implementari tutto il resto 
    
    public ObjectManagerVR objectManagerVR;
    public SimulationManager simulationManager;
    public AnimaPersonaggio animaPersonaggio;
    public PhraseGenerator phraseGenerator;
    public SaveLoadStage saveLoadManager;
    
    */
    //private TapToPlace ttp;
    //private BoundsControl bc;
    [SerializeField] public string type;
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

    // Start is called before the first frame update
    void Start()
    {
        charAnim = this.GetComponent<Animator>();
        SimulationManager.setUpMovement +=EnableCharacterMovement;
        
        if (!loadedObject || type=="")
        {
            type = gameObject.name;
        }
          nearObjects = new List<string>();
     
          place = "ground";

        isWalking = false;
        newPlace = false;

        CursorVisual = GameObject.Find("CursorVisual");

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
                        StopWalking();
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
           
            // phraseGenerator.UpdateForward(hit.transform.gameObject.name,p);
        }
        //else gameManager.GetComponent<PhraseGenerator>().UpdateForward("");
    }

    public void EnableCharacterMovement(object sender, EventArgs e)
    {   
        this.walkMode = true;
    }
    public void Move(RaycastHit hitInfo)
    {
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
            this.GetComponent<Animator>().SetBool("walking", true);
            isWalking = true;  // Assicurati che questo valore sia impostato correttamente
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
        Debug.Log("Personaggio fermato");
    }

  
    
    public void StopOrPlayCharAnimation()
    {
        var animator = this.GetComponent<Animator>();
        var agent = this.GetComponent<NavMeshAgent>();

        if (animator.speed != 0)
        {
            // Ferma l'animazione
            animator.speed = 0;

            // Ferma anche il NavMeshAgent se è in movimento
            if (agent != null && agent.hasPath)
            {
                agent.isStopped = true;  // Ferma il movimento del NavMeshAgent
            }

            // Disabilita qualsiasi parametro di animazione che potrebbe causare il movimento
            animator.SetBool("walking", false);
        }
        else
        {
            // Riavvia l'animazione
            animator.speed = 1;

            // Se desideri che l'agente riprenda a muoversi, puoi gestirlo qui
            if (agent != null && agent.isStopped)
            {
                agent.isStopped = false;  // Riavvia il movimento del NavMeshAgent
            }
        }
    }
  
    
    
    public void CheckDestination()
    {

        checkDestination = true;
    }
    

    
}
