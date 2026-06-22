using System;
using UnityEngine;

public class Atom : MonoBehaviour
{
    //event that is called when a fusion happens successfully, notifies levelmanager to update how many reactions left
    public static event Action atomFused;

    [SerializeField] private float forceRequiredToFuse = 2.0f;

    void Start()
    {
        //self register to the game manager
        LevelManager.m_instance.atomRegister();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float velocityImpact = collision.relativeVelocity.magnitude;
        if (velocityImpact > forceRequiredToFuse)
            Fuse(collision.gameObject);
    }

    private void Fuse(GameObject atomToFuseWith)
    {
        //for now just turning game object off
        atomFused?.Invoke();
        atomToFuseWith.SetActive(false);
    }
}
