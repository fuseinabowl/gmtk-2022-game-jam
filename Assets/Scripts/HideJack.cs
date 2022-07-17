using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideJack : MonoBehaviour
{
    [SerializeField]
    private Transform jackTransform;

    public void Hide()
    {
        jackTransform.gameObject.SetActive(false);
    }
}
