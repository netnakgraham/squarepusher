using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTyle : MonoBehaviour {
 
	
	private Renderer renderer;
	private Color originalColor;
 	private bool isOccupied = false;
 	private bool isMouseOver = false;

	void Start() {

		renderer = GetComponent<Renderer>();
		originalColor = renderer.material.color;
	}


    void OnMouseOver()
    {
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }

	public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool value)
    {
        isOccupied = value;
    }


    public void HighlightTile(Color highlightColor)
    {
        GetComponent<Renderer>().material.color = isMouseOver ? highlightColor : originalColor;
    }
}