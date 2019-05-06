using UnityEngine;

public class DestroyOnBecomeInvisible : MonoBehaviour
{
    public void OnBecameInvisible()
    {
       // Destroy object because it's no longer rendering
        Destroy(gameObject);
    }
}
