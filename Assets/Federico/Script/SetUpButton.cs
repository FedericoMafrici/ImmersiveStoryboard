using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUpButton : MonoBehaviour
{

    private SimulationManager _simulationManager;

    // Start is called before the first frame update
    void Start()
    {
        _simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        if (_simulationManager == null)
        {
            Debug.LogError("simulation manager non trovato per il bottone " + this.name);
        }


    }

    public void moveButton()
    {
        _simulationManager.SetupMovement();
    }

    public void StartOrStopButton()
    {
            _simulationManager.StartOrStopAnimation();
    }

    public void getControl()
    {
        _simulationManager.getControlOfsecondCharacter();
    }
    
    // Update is called once per frame
    void Update()
    {

    }

}