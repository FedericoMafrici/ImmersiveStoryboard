using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class State : MonoBehaviour
{
    public List<string> state; //Lista di stati correnti, per implementare eventualmente il multi-stato
    public  List<string> defaultState;
    public List<string> possibleStates;
    public ActionsDataBase DataBase;
    [SerializeField] private SimulationManager _simulationManager;
    public Animator animator;
    //TODO INSERT THE OTHER SCRIPT WHEN THE SECOND PART IS DONE 
    public PhraseGenerator phraseGenerator;
    //[SerializeField] private SimulationManager simulationManager;

    private string initialState;
    public string surname;

    public List<string> nearObjs; //Oggetti vicini, validi solo per i Players

    //Questo script � assegnato ad ogni oggetto al momento della sua creazione in creation mode, nel BuildingManager (metodo SelectObject)

    public void OnEnable()
    {
        SimulationManager.pauseStoryboarding += ResetState;
    }

    public void OnDisable()
    {
        SimulationManager.pauseStoryboarding -= ResetState;
    }
    
    void Start()
    {
        state = new List<string>();
        defaultState = state;
        nearObjs = new List<string>();

        //surname = gameObject.name;
        DataBase = GameObject.Find("SimulationManager").GetComponent<ActionsDataBase>();
        phraseGenerator = GameObject.Find("SimulationManager").GetComponent<PhraseGenerator>();
        _simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        //  phraseGenerator = GameObject.Find("SimulationManager").GetComponent<PhraseGenerator>(); TODO 

      //  possibleStates = DataBase.GetPossibleStates(gameObject.GetComponent<CharacterManager>().type);
        if (GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();

        //Se l'oggetto pu� avere stati, il suo stato viene inizializzato al primo presente nella lista degli stati possibili
        if (possibleStates != null && possibleStates.Count!=0 )
        {
            state.Add(possibleStates[0]);
        }
        if (possibleStates != null && possibleStates.Count!=0)
        {
            initialState = state[0];
        }

     

    }

    public void Update()
    {
        // aggiunto altrimenti nel testo generato non stampava sempre i nomi rinomati 
       // surname = gameObject.name;
    }

public void ChangeState(string action) {
        string[] transition;
        if (DataBase.GetStateTransition(action, gameObject.GetComponent<CharacterManager>().type) != null)
        {
            transition = DataBase.GetStateTransition(action, gameObject.GetComponent<CharacterManager>().type);
            if (state[0] == transition[0])
            {
                ChangeStateAnimation(state[0], transition[2]);
                state[0] = transition[2];
            }

            //Multistati binari


            if (state.Count > 1)
            {
                for (int i = 1; i < state.Count; i++)
                {
                    if (state[i] == transition[0])
                    {
                        
                        // Memorizza il nome dell'elemento da rimuovere
                        string stateToRemove = state[i];

                        // Rimuovi tutti gli stati con lo stesso nome
                        for (int j = i; j < state.Count; j++)
                        {
                            if (state[j] == stateToRemove)
                            {
                                state.RemoveAt(j);
                                j--; // Decrementa j per continuare a controllare lo stesso indice
                            }
                        }

                      
               

                   }
               }
           }


                    }
                }

    public void SetState(string s, string action)
    {
     
        if (action == "play" || action == "sit")
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        ChangeStateAnimation(state[0], s);

        state[0] = s;
        phraseGenerator.GenerateStatusPhrase(gameObject.GetComponent<CharacterManager>().type, state[0]);
        
    }

    //Animazioni passive
    public void PlayAnimation(string action) 
    {
        if (animator != null)
        {
            //Se � un oggetto passivo, effettua l'animazione corrispondente all'azione
            if (CompareTag("Object"))
            {
                foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
                {
                    if (ac.name == action)
                    {
                        animator.Play(ac.name);
                        return;
                    }
                }
            }
            //Se � un personaggio passivo, effettua l'animazione passiva corrispondente all'azione, cercando quelle con prefisso "get"
            if (CompareTag("Player"))
            {
                foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
                {
                    if (ac.name == "get " + action)
                    {
                        animator.Play(ac.name);
                        return;
                    }
                }
            }
        }


        /* //Suono  eventuale
        if (GetComponent<AudioSource>() != null)
        {
            if (GetComponent<AudioSource>().clip.name == action)
                GetComponent<AudioSource>().Play();
        }*/

    }
    
    public void ChangeStateAnimation(string Oldstate, string newState)
    {
        
        if (animator != null)
        {
            if (CompareTag("Player"))
            {
                if (Oldstate != initialState)
                {
                    animator.SetBool(Oldstate, false);
                }
                animator.SetBool(newState, true);
                _simulationManager.activeCharacter.GetComponent<Animator>().speed = 0.5f;

            }
        }
        
    }

    public void ResetState(object s,EventArgs e)
    {
        state = defaultState;
    }
    public string GetCurrentState()
    {
    //    return state[0];
    return ""; //  TODO cancella quando avrai inserito tutto il codice 
    }

}