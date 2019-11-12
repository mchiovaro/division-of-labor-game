using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_app_script : MonoBehaviour
{
    float ScToWRatio;
    GameObject work_row;   // Start is called before the first frame update
    void Start()
    {
        work_row = GameObject.FindGameObjectWithTag("work_row_middle");

        ScToWRatio = 1080/(2*Camera.main.orthographicSize);

        Debug.Log(" scr worl ratio = " + ScToWRatio);

        for (int ii = 0; ii < work_row.transform.childCount; ii++)
        {
            work_row.transform.GetChild(ii).position = new Vector3(0, ii*108/ScToWRatio - Camera.main.orthographicSize + 3*108/(2*ScToWRatio));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape")) Application.Quit();

    }
}
