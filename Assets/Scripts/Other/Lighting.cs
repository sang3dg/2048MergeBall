using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    private TrailRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<TrailRenderer>();
    }
    IEnumerator FlashLight()
    {
        Material material = lineRenderer.material;
        float startOffset = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            startOffset += 0.25f;
            material.SetTextureOffset("_MainTex", new Vector2(0, startOffset));
        }
    }
    private void OnEnable()
    {
        StartCoroutine("FlashLight");
    }
    private void OnDisable()
    {
        StopCoroutine("FlashLight");
    }
}
