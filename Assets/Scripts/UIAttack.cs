using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAttack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public int index;

    public Attack attack { get; private set; }

    [Header("UI")]
    public Button button;
    public Image img;
    public Color defaultColor = Color.white;
    
	public void SetAttack(Attack atk)
    {
        attack = atk;
        if (atk != null)
        {
            button.interactable = true;
            //img.sprite = atk.type.icon;
            img.color = atk.type.color;

        } else
        {
            img.color = defaultColor;
            button.interactable = false;
        }
    }

    public void OnClick()
    {
        attack.Activate();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (attack != null)
        {
            attack.highlight.Show();
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (attack != null)
        {
            attack.highlight.Hide();
        }
    }
}
