using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class BlackBackPlaneMaterialProperties : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer = null;

    [SerializeField]
    private float coverageAmount = 0f;

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
        propertyBlock.SetFloat("_ShowProportion", coverageAmount);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}
