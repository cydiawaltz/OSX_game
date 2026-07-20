using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ControlStiripUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage Exp;
    public float duration;
    public Ease ease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Exp.color = new Color(1,1,1,0);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Exp.DOColor(new Color(1,1,1,1),duration).SetEase(ease);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Exp.DOColor(new Color(1,1,1,0),duration).SetEase(ease);
    } 
    void OnDisable()
    {
        Exp.DOColor(new Color(1,1,1,0),duration).SetEase(ease);
    }   
}
