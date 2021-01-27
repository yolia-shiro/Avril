using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMeshRendererLayer : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public string sortingName;
    public int sortingId;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = sortingName;
        meshRenderer.sortingOrder = sortingId;
    }
}
