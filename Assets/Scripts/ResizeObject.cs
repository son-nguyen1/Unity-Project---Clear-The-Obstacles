using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float resizeSpeed = 3f;

    private RectTransform rectTransform;

    private Vector3 startScale;
    private Vector3 hoverEnterScale;
    private Vector3 hoverClickScale;

    private float sizeOffset = 0.15f;

    private bool isHovering = false;
    private bool isClicked = false;

    /// <summary>
    /// On default, the objects stay unchanged. They will enlarge or shrink, when hovered over or clicked, as visual cues.
    /// </summary>
    private void Start()
    {
        startScale = Vector3.one;
        hoverEnterScale = startScale + new Vector3(sizeOffset, sizeOffset, 0f);
        hoverClickScale = startScale + new Vector3(-sizeOffset, -sizeOffset, 0f);

        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        HandleResizeEffect();        
    }

    /// <summary>
    /// The objects have colliders, which handle interactions with the pointer, and determine a target size.
    /// At 1st, the target size is default, so the objects are unchanged.
    /// Once hovered over, they enlarge a little. Once clicked, they shrink a little, then return to their default size.
    /// </summary>
    private void HandleResizeEffect()
    {
        Vector3 targetScale = startScale;

        if (isClicked)
        {
            targetScale = hoverClickScale;

            if (Vector3.Distance(transform.localScale, hoverClickScale) < 0.01f)
            {
                isClicked = false;
            }
        }
        else if (isHovering)
        {
            targetScale = hoverEnterScale;
        }

        transform.localScale = new Vector3(
            Mathf.MoveTowards(transform.localScale.x, targetScale.x, resizeSpeed * Time.deltaTime),
            Mathf.MoveTowards(transform.localScale.y, targetScale.y, resizeSpeed * Time.deltaTime),
            transform.localScale.z
            );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.activeInHierarchy)
        {
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.activeInHierarchy)
        {
            isHovering = false;
        }       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.activeInHierarchy)
        {
            isClicked = true;
        }
    }
}