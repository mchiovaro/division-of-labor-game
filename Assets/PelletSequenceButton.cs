/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;


/// <exclude />
public class PelletSequenceButton : MonoBehaviour
{

    private const int BUTTON_TRIANGLE = 1;
    private const int BUTTON_SQUARE = 2;
    private const int BUTTON_CIRCLE = 3;

    private int myButtonColor = 0;

    private void Start()
    {

    }

    private void Awake()
    {
        if (transform.tag.Equals("button_triangle"))
            myButtonColor = BUTTON_TRIANGLE;
        if (transform.tag.Equals("button_square"))
            myButtonColor = BUTTON_SQUARE;
        if (transform.tag.Equals("button_circle"))
            myButtonColor = BUTTON_CIRCLE;


    }

    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += tappedHandler2;
    }

    private void OnDisable()
    {
        GetComponent<TapGesture>().Tapped -= tappedHandler2;
    }


    //this happens when the pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {

        Debug.Log(transform.parent.parent.GetComponent<PelletScript>().player_coll.gameObject.tag);

            if(transform.parent.parent.GetComponent<PelletScript>().player_coll.gameObject.tag.Equals ("bee_free"))
                transform.parent.parent.GetComponent<PelletScript>().addToSequence(myButtonColor, GetComponent<SpriteRenderer>());

    }
}
