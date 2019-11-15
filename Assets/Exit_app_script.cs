using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Exit_app_script : MonoBehaviour
{
    float ScToWRatio;
    GameObject work_cells, drop_cells, spawn_cells;   // Start is called before the first frame update

    public float spawn_rate = 10.0f;

    private float next_spawn = 0.0f;

    public GameObject foodPrefab, beeFree, beeRestrict;

    public struct SaveData
    {
        public List<Vector3> allPositions;
        public List<float> allTimeStamps;
    }

    public SaveData data_beeFree, data_beeRestrict;
    private bool quitting = false;

    void Start()
    {

        data_beeFree.allPositions = new List<Vector3>(100000);
        data_beeFree.allTimeStamps = new List<float>(100000);

        data_beeRestrict.allPositions = new List<Vector3>(100000);
        data_beeRestrict.allTimeStamps = new List<float>(100000);


        work_cells = GameObject.FindGameObjectWithTag("work_row_middle");
        drop_cells = GameObject.FindGameObjectWithTag("drop_cells");
        spawn_cells = GameObject.FindGameObjectWithTag("spawn_cells");
        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestrict = GameObject.FindGameObjectWithTag("bee_restricted");

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

        float timestamp = Time.time;

        if (Input.GetKey("escape") && !quitting) saveAndQuit();

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

        data_beeFree.allTimeStamps.Add(timestamp);
        data_beeRestrict.allTimeStamps.Add(timestamp);

        data_beeFree.allPositions.Add(beeFree.transform.position);
        data_beeRestrict.allPositions.Add(beeRestrict.transform.position);

    }

    void saveAndQuit()
    {
        quitting = true;
        string path = "Assets/Resources/Data/dataTest.csv";
        StreamWriter writer = new StreamWriter(path, true);

        for (int ii = 0; ii < data_beeFree.allPositions.Count; ii++)
        {
            string next_line = data_beeFree.allTimeStamps[ii] + "," + data_beeFree.allPositions[ii].x + "," + data_beeFree.allPositions[ii].y + "," + data_beeRestrict.allPositions[ii].x + "," + data_beeRestrict.allPositions[ii].y;

            writer.WriteLine(next_line);

        }
        writer.Close();

        Application.Quit();

    }



}
