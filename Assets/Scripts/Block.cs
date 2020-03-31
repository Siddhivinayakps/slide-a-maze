using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Block : MonoBehaviour, IPointerDownHandler
{
    public int Row, Column, InitRow, InitColumn;
    public event System.Action<Block> OnBlockPressed;
    public Image BlockImage;
    public int number;

    public int Number
    {
        get
        {
            return number;
        }
        set
        {
            number = value;
            if(value == 0)
            {
                SetEmpty();
            } else
            {
                SetActive();
            }
        }
    }

    private void Awake()
    {
        BlockImage = this.gameObject.GetComponent<Image>();

    }

    // Start is called before the first frame update
    void Start()
    {
        InitRow = Row;
        InitColumn = Column;
    }

    // Update is called once per frame
        void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnBlockPressed != null)
        {
            OnBlockPressed(this);
        }
    }

    private void SetEmpty()
    {

        BlockImage.enabled = false;
    }
    
    private void SetActive()
    {
        BlockImage.enabled = true;
    }


    public void SetImage(Sprite image)
    {
        BlockImage.sprite = image;
    }

    public bool OnCorrectPosition()
    {
        if(InitColumn == Column && InitRow == Row)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
