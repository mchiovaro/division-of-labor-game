using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_app_script : MonoBehaviour
{
    float ScToWRatio;
    GameObject work_cells, drop_cells, spawn_cells;   // Start is called before the first frame update

    public float spawn_rate = 10.0f;

    private float next_spawn = 0.0f;

    public GameObject foodPrefab;
    void Start()
    {
        work_cells = GameObject.FindGameObjectWithTag("work_row_middle");
        drop_cells = GameObject.FindGameObjectWithTag("drop_cells");
        spawn_cells = GameObject.FindGameObjectWithTag("spawn_cells");


        ScToWRatio = 1080 / (2 * Camera.main.orthographicSize);

        Debug.Log(" scr worl ratio = " + ScToWRatio);

        for (int ii = 0; ii < work_cells.transform.childCount; ii++)
            work_cells.transform.GetChild(ii).position = new Vector3(0, ii * 108 / ScToWRatio - Camera.main.orthographicSize + 3 * 108 / (2 * ScToWRatio));

        for (int ii = 0; ii < drop_cells.transform.childCount; ii++)
            drop_cells.transform.GetChild(ii).position = new Vector3(-5.4f * (6.0f / 5), ii * 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio));

        for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
            spawn_cells.transform.GetChild(ii).position = new Vector3(5.4f * (6.0f / 5), ii * 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape")) Application.Quit();

        if (Time.time > next_spawn)
        {
            next_spawn = Time.time + spawn_rate;

            List<int> avail_spawn = new List<int>();

            for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
            {
                if (!spawn_cells.transform.GetChild(ii).GetComponent<SpawnCellScript>().contact_on)
                    avail_spawn.Add(ii);
            }
            if (avail_spawn.Count > 0)
            {
                int rand_ini = Random.Range(0, avail_spawn.Count);

                Instantiate(foodPrefab, spawn_cells.transform.GetChild(avail_spawn[rand_ini]).position, Quaternion.identity);
            }

        }


    }
}
