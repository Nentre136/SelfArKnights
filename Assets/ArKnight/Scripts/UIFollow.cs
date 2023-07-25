using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    private GameObject roleGameObject;//½ÇÉ«×é¼þ
    [SerializeField] RectTransform healthBar;
    
    void Start()
    {
        roleGameObject = transform.parent.gameObject;
        
    }

    void Update()
    {
        LayoutFollow();
    }

    private void LayoutFollow()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(roleGameObject.transform.position);
        healthBar.position = screenPosition + new Vector3(0,-10f,0);
    }
}
