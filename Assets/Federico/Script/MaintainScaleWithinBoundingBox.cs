using UnityEngine;

public class MaintainScaleWithinBoundingBox : MonoBehaviour
{
    public GameObject plane; // Il piano all'interno della bounding box
    public GameObject boundingBox; // La bounding box

    private Vector3 initialBoundingBoxScale; // Scala iniziale della bounding box
    private Vector3 initialPlaneScale; // Scala iniziale del piano

    void Start()
    {
        // Salva la scala iniziale della bounding box e del piano
        initialBoundingBoxScale = boundingBox.transform.localScale;
        initialPlaneScale = plane.transform.localScale;
    }

    void Update()
    {
        // Aggiorna la scala del piano in base alla scala attuale della bounding box
        MaintainProportionalScale();
    }

    private void MaintainProportionalScale()
    {
        // Calcola il rapporto di scala rispetto alla scala originale della bounding box
        Vector3 scaleFactor = new Vector3(
            boundingBox.transform.localScale.x / initialBoundingBoxScale.x,
            boundingBox.transform.localScale.y / initialBoundingBoxScale.y,
            boundingBox.transform.localScale.z / initialBoundingBoxScale.z
        );

        // Applica il fattore di scala al piano, mantenendo il rapporto proporzionale
        plane.transform.localScale = new Vector3(
            initialPlaneScale.x * scaleFactor.x,
            initialPlaneScale.y * scaleFactor.y,
            initialPlaneScale.z * scaleFactor.z
        );
    }
}