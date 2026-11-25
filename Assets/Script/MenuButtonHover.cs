using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject setaObject;

    private void Start()
    {
        if (setaObject != null)
        {
            setaObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (setaObject != null)
        {
            setaObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (setaObject != null)
        {
            setaObject.SetActive(false);
        }
    }
}