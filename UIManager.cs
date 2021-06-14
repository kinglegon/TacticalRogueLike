using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite attackSprite;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseAttackSprite()
    {
        image.sprite = attackSprite;
    }

    public void UseDefaultSprite()
    {
        image.sprite = defaultSprite;
    }
}
