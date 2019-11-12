using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;
using TouchScript.Gestures;

public class BasicTouch : MonoBehaviour
{
    public Transform Position;

    private PressGesture press;

    private void OnEnable()
    {
        press = GetComponent<PressGesture>();
        press.Pressed += pressHandler;
    }

    private void OnDisable()
    {
        press.Pressed -= pressHandler;
    }

    private void pressHandler(object sender, EventArgs eventArgs)
    {
        Debug.Log(Position.position.x);

        press.Cancel(true, true);
        //   var target = Instantiate(Prefab, Position.parent);
        /*  target.position = Position.position;

          LayerManager.Instance.SetExclusive(target);
          press.Cancel(true, true);
          LayerManager.Instance.ClearExclusive();*/
    }
}
