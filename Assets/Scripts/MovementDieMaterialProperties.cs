using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class MovementDieMaterialProperties : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer = null;

    [SerializeField]
    [ColorUsage(false, true)]
    public Color glowColor = Color.white;

    private MaterialPropertyBlock propertyBlock = null;

    private void Awake()
    {
        Assert.IsNotNull(targetRenderer);
    }

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        Assert.IsNotNull(propertyBlock);
        propertyBlock.SetColor("_GlowColor", glowColor);
        for (var materialIndex = 0; materialIndex < targetRenderer.sharedMaterials.Length; ++materialIndex)
        {
            targetRenderer.SetPropertyBlock(propertyBlock, materialIndex);
        }
    }
}
