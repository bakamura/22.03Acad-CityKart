using UnityEngine;

public class PostProcessControler : MonoBehaviour
{
    public void Activate_deactivateDepthOfField(bool isAtive)
    {
        this.gameObject.SetActive(isAtive);
    }
}
