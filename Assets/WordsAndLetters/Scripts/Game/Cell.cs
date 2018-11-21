using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct Letter
{
    public Sprite icon;
    public string letter;
}

[Serializable]
public class Cell : MonoBehaviour
{

#region Variables
    public List<Cell> cells;
    public Letter value;
    
    public GameObject back;
    public List<Letter> Alphabet;
    public Vector2 index;
    private SpriteRenderer _sr;
#endregion

#region Unity functions
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        SetValue(" ");
    }

    ///<summary>
    /// Show cell links 
    ///</summary>
    void OnDrawGizmosSelected()
    {
        for(var i=0;i<cells.Count;i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, cells[i].transform.position);
        }
    }
#endregion

#region Cell functions 
    ///<summary>
    /// Set value and update cell UI
    ///</summary>
    public void SetValue(string val)
    {
        if (Alphabet.FindAll(pred => pred.letter.ToLower() == val.ToLower()).Count != 0)
        {
            value = Alphabet.Find(pred => pred.letter.ToLower() == val.ToLower());
            _sr.sprite = value.icon;
        }
        else
        {
            value = Alphabet.Find(pred => pred.letter==" ");
            _sr.sprite = value.icon;
        }
    }

    ///<summary>
    /// Set cell background
    ///</summary>
    public void SetBackgroundColor(Color color)
    {
        back.SetActive(true);
        back.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.75f);
    }

    ///<summary>
    /// Hide cell background
    ///</summary>
    public void HideBackground()
    {
        back.SetActive(false);
    }
#endregion

}