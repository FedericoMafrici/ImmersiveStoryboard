using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class AutoResizeRawImage : MonoBehaviour
{
    private RawImage rawImage;

    void Awake()
    {
        // Recupera il componente RawImage
        rawImage = GetComponent<RawImage>();
    }

    /// <summary>
    /// Aggiorna la dimensione del RawImage in base alla texture caricata.
    /// </summary>
    public void UpdateImage(Texture newTexture)
    {
        if (newTexture == null)
        {
            Debug.LogError("La texture è null, impossibile aggiornare l'immagine.");
            return;
        }

        // Assegna la nuova texture
        rawImage.texture = newTexture;

        // Calcola la nuova dimensione mantenendo il rapporto
        RectTransform rectTransform = rawImage.rectTransform;
        float textureWidth = newTexture.width;
        float textureHeight = newTexture.height;

        // Mantiene le proporzioni in base al RectTransform corrente
        float currentWidth = rectTransform.rect.width;
        float scaleFactor = currentWidth / textureWidth;

        rectTransform.sizeDelta = new Vector2(textureWidth * scaleFactor, textureHeight * scaleFactor);
    }
}