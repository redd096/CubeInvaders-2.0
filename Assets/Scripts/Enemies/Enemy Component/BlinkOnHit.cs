using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy Component/Blink On Hit")]
public class BlinkOnHit : MonoBehaviour
{
    [Header("Blink")]
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float blinkTime = 0.1f;

    //for blink
    Material originalMat;
    Coroutine blink_Coroutine;

    void OnTriggerEnter(Collider other)
    {
        //blink if hit by shot
        if (other.GetComponentInParent<TurretShot>())
        {
            if (blink_Coroutine == null)
                blink_Coroutine = StartCoroutine(Blink_Coroutine());
        }
    }

    IEnumerator Blink_Coroutine()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();

        //change material
        if (originalMat == null)
        {
            originalMat = renderer.material;
            renderer.material = blinkMaterial;
        }

        //wait
        yield return new WaitForSeconds(blinkTime);

        //back to original material
        renderer.material = originalMat;
        originalMat = null;

        blink_Coroutine = null;
    }
}
