using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessControler : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcess;
    public void Activate_deactivateDepthOfField(bool isAtive)
    {
        postProcess.gameObject.SetActive(isAtive);
    }
}
