//Dan Ivanovas
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerElementHover : MonoBehaviour, IPointerEnterHandler
{
    public TowerSounds sounds;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Play sound when hovering over an element
        sounds.PlayHover();
    }
}