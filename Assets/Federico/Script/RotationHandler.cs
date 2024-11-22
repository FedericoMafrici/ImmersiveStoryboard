using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

public class RotationHandler : MonoBehaviour
{
    private Camera xrCamera; // Riferimento alla camera dell'XR Origin
    [SerializeField] private TransformSync _transformSyncPersonaggioAttivo;
    [SerializeField] private Transform  posizioneCorrispondente;
    void Awake()
    {
        // Trova l'XR Origin in scena e ottieni la camera
        var xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            xrCamera = xrOrigin.Camera;
        }
        else
        {
            Debug.LogError("XR Origin non trovato in scena! Assicurati che esista un oggetto XR Origin.");
        }
    }

    void OnEnable()
    {
        if (xrCamera != null)
        {
            OrientTowardsUser();
        }
        else
        {
            Debug.LogError(
                "Camera dell'XR Origin non trovata! Verifica che l'XR Origin sia configurato correttamente.");
        }

        if (this.CompareTag("Player"))
        {
            CharacterManager.OnDestinationReached += AdjustPosition;
        }
    }

    private void OnDisable()
    {
        if (this.CompareTag("Player"))
        {
            CharacterManager.OnDestinationReached -= AdjustPosition;
        }
    }

    private void AdjustPosition(object obj,EventArgs e)
    {
        _transformSyncPersonaggioAttivo.transform.position = posizioneCorrispondente.transform.position;
    }
    
    private void OrientTowardsUser()
    {
        // Salva la posizione corrente dell'oggetto
        Vector3 originalPosition = transform.position;

        // Calcola la direzione verso l'utente
        Vector3 directionToUser = xrCamera.transform.position - transform.position;

        // Mantieni solo la direzione sull'asse X-Z (orizzontale), azzerando la Y
        directionToUser.y = 0;

        
        // Verifica che la direzione non sia nulla
        if (directionToUser.sqrMagnitude > 0.001f)
        {
            // Calcola la rotazione per guardare l'utente, solo sull'asse Y
            float targetYRotation = Mathf.Atan2(directionToUser.x, directionToUser.z) * Mathf.Rad2Deg;

            // Applica la rotazione solo sull'asse Y
            _transformSyncPersonaggioAttivo.transform.rotation = Quaternion.Euler(0, targetYRotation, 0);

            // Reimposta la posizione originale per evitare spostamenti
            _transformSyncPersonaggioAttivo.transform.position = originalPosition;

            Debug.Log($"Oggetto {gameObject.name} orientato verso l'utente. Rotazione Y: {targetYRotation}");
        }
        else
        {
            Debug.LogWarning("La direzione verso l'utente è nulla. Assicurati che l'oggetto e la camera non siano nella stessa posizione.");
        }
    }
}