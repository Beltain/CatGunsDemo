using UnityEngine;

public class MeshSharer : MonoBehaviour
{
    public Material[] sharedMaterials;
    float scaleFactor = 2f;

    void Start()
    {
        GetComponent<Renderer>().sharedMaterial = sharedMaterials[0];
    }
}
