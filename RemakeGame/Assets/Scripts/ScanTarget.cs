using UnityEngine;
using System.Collections;

public class ScannableObject : MonoBehaviour
{
    [Header("Material")]
    public Material highlightOverlayMat; 

    [Header("Time")]
    public float duration = 10f; 

    private Renderer _renderer;
    private Material _baseMat; 
    private Coroutine _revertRoutine;

    void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _baseMat = _renderer.sharedMaterial;

            _renderer.materials = new Material[] { _baseMat };
        }
    }

    public void OnScanned()
    {
        if (_renderer == null || highlightOverlayMat == null) return;

        _renderer.materials = new Material[] { _baseMat, highlightOverlayMat };

        if (_revertRoutine != null) StopCoroutine(_revertRoutine);
        _revertRoutine = StartCoroutine(WaitAndRemove());
    }

    IEnumerator WaitAndRemove()
    {
        yield return new WaitForSeconds(duration);

        if (_renderer != null)
        {
            _renderer.materials = new Material[] { _baseMat };
        }
        _revertRoutine = null;
    }
}