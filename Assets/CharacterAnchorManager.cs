using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CharacterAnchorManager : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;
    private ARAnchor _currentAnchor;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anchorManager = FindObjectOfType<ARAnchorManager>();
        if (anchorManager == null)
        {
            Debug.LogError("Character Anchor manager di :" + gameObject.name + " non trovato");
        }
        CreateAnchor();
    }

    public void DestroyAnchor()
    {
        if (_currentAnchor == null)
        {
            return;
        }
        Destroy(_currentAnchor.gameObject);
        _currentAnchor = null;
        transform.parent = null; // Rimuove la parentela per la manipolazione libera
    }
    
    public async void CreateAnchor()
    {
        // Controlla se esiste già un'ancora e la rimuove se presente
        if (_currentAnchor != null)
        {
            Destroy(_currentAnchor);
            _currentAnchor = null;
        }

        // Verifica l'esistenza di ARAnchorManager
        if (anchorManager == null)
        {
            Debug.LogError("ARAnchorManager non trovato. Assicurati che sia presente nella scena.");
            return;
        }

        try
        {
            var result = await anchorManager.TryAddAnchorAsync(new Pose(transform.position, transform.rotation));
            if (result.status.IsSuccess())
            {
                _currentAnchor = result.value;
                transform.parent = _currentAnchor.transform; // Rende l'oggetto figlio dell'ancora
                Debug.Log("Ancora creata e assegnata con successo.");
            }
            else
            {
                Debug.LogWarning("Impossibile creare un'ancora alla posizione specificata.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore durante la creazione dell'ancora: " + ex.Message);
        }
    }
    
}
