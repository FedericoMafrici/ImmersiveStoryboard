using System.IO;
using UnityEngine;

public class CreateDirectoryExample : MonoBehaviour
{
    void Start()
    {
        CreateFolder();
    }

    public void CreateFolder()
    {
        // Specifica il nome della cartella da creare
        string folderName = "Federico";
        
        // Percorso completo della nuova cartella
        string path = Path.Combine(Application.persistentDataPath, folderName);

        // Verifica se la cartella esiste già
        if (!Directory.Exists(path))
        {
            // Crea la cartella
            Directory.CreateDirectory(path);
            Debug.Log("Cartella creata: " + path);
        }
        else
        {
            Debug.Log("La cartella esiste già: " + path);
        }
    }
}