using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class GoalIndicatorMaterialProperties : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer = null;

    [SerializeField]
    [ColorUsage(true, false)]
    private Color albedoColor = Color.white;

    [SerializeField]
    [ColorUsage(false, true)]
    private Color emissionColor = Color.white;

    private MaterialPropertyBlock propertyBlock = null;

    private void Awake()
    {
        Assert.IsNotNull(targetRenderer);
    }

    private void Start()
    {
        CreatePropertyBlock();
    }

    private void CreatePropertyBlock()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (propertyBlock == null)
        {
            CreatePropertyBlock();
        }
#endif
        propertyBlock.SetColor("_BaseColor", albedoColor);
        propertyBlock.SetColor("_EmissionColor", emissionColor);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}
