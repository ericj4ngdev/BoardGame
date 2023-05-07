using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    public GameObject NextImage;

    public void ActiveImage()
    {
        NextImage.SetActive(true);
        gameObject.SetActive(false);
    }

}
