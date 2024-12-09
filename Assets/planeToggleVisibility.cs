using System;
using UnityEngine;

public class planeToggleVisibility : MonoBehaviour
{
    [SerializeField] private Material featheredPlane;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScreenshotManager.OnHidePanels += HidePlaneMaterial;
        ScreenshotManager.screenShotTaken += ShowPlaneMaterial;
    }

    private void OnDestroy()
    {
        ScreenshotManager.OnHidePanels -= HidePlaneMaterial;
        ScreenshotManager.screenShotTaken -= ShowPlaneMaterial;
    }

    
      private void HidePlaneMaterial(object sender, EventArgs e)
    {
        if (featheredPlane != null)
        {
            // Assicurati che il nome della proprietà sia corretto
            string propertyName = "_TexTintColor"; // Modifica il nome della proprietà se necessario

            if (featheredPlane.HasProperty(propertyName))
            {
                // Ottieni il colore attuale
                Color currentColor = featheredPlane.GetColor(propertyName);

                // Modifica l'alpha del colore
                currentColor.a = 0;

                // Assegna il colore aggiornato al materiale
                featheredPlane.SetColor(propertyName, currentColor);

                Debug.Log($"La trasparenza del materiale è stata impostata a 0 per la proprietà {propertyName}.");
            }
            else
            {
                Debug.LogError($"La proprietà {propertyName} non è presente nel materiale.");
            }
        }
        else
        {
            Debug.LogError("Il materiale featheredPlane non è assegnato.");
        }
    }
    private void ShowPlaneMaterial(object sender, EventArgs e)
    {
        if (featheredPlane != null)
        {
            // Assicurati che il nome della proprietà sia corretto
            string propertyName = "_TexTintColor"; // Modifica il nome della proprietà se necessario

            if (featheredPlane.HasProperty(propertyName))
            {
                // Ottieni il colore attuale
                Color currentColor = featheredPlane.GetColor(propertyName);

                // Modifica l'alpha del colore
                currentColor.a = 1.0f;

                // Assegna il colore aggiornato al materiale
                featheredPlane.SetColor(propertyName, currentColor);

                Debug.Log($"La trasparenza del materiale è stata impostata a 0 per la proprietà {propertyName}.");
            }
            else
            {
                Debug.LogError($"La proprietà {propertyName} non è presente nel materiale.");
            }
        }
        else
        {
            Debug.LogError("Il materiale featheredPlane non è assegnato.");
        }
    }
}
