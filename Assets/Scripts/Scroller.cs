using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    int numLayers;
    Transform[] layers;
    public float[] layerFac;
    public Camera attachedCamera;
    Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        numLayers = layerFac.Length;
        layers = new Transform[numLayers];
        initialPosition = attachedCamera.transform.position;
        // find the layers, these are assumed to be the first numLayers children
        // layers objects are assumed to have zero local transform to the parent
        for (int i = 0; i < numLayers; i++)
        {
            layers[i] = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = attachedCamera.transform.position - initialPosition;
        // scale by the scale factors for each layer,
        // and set the local position of each child
        for (int i = 0; i < numLayers; i++)
        {
            Vector3 scaledDiff = diff * layerFac[i];
            layers[i].transform.localPosition = new Vector3(scaledDiff.x,
           scaledDiff.y, 0.0f);
        }
    }
}
