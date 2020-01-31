using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.UI;


public class Exit_app_script : MonoBehaviour
{
    float ScToWRatio;
    GameObject work_cells, drop_cells, spawn_cells;   // Start is called before the first frame update

    public int ParticipantNumber;
    public string RA_initials;
    public int current_trial = 0;

    private const int MAX_PELLETS = 20;

    private List<int> conditions_ = new List<int>(6);
    private const int CONDITION_EXPERIMENT_OVER = 0;
    private const int CONDITION_CONSTANT_RATE = 1;
    private const int CONDITION_INCREASING_RATE = 2;
    private const int CONDITION_RANDOM_RATE = 3;

    private int current_cond = 0;
    private bool practice_mode = true;

    private List<float> spawn_rate = new List<float>(new float[MAX_PELLETS]);

    private List<GameObject> dropped_pellets = new List<GameObject>();

    private bool running_ = false;
    private float next_spawn = 0.0f;
    private int current_spawn = 0;

    public int current_frame = 0;

    public GameObject foodPrefab, beeFree, beeRestrict;
    public int pellets_dropped = 0;

    public Text ui_text;
    public RawImage ui_image;

    public struct SaveData
    {
        public List<Vector3> allPositions;
        public List<float> allTimeStamps;
    }

    public struct PelletData
    {
        public List<string> pelletEvent;
        public List<int> frameNum;
    }

    public List<PelletData> pellData = new List<PelletData>();
 //   public List<List<string>> pelletEvent;

  //  public List<SaveData2> saveData2 = new List<SaveData2>();

    public SaveData data_beeFree, data_beeRestrict;
    private bool quitting = false;

    private float timeIni = 0;

    void Start()
    {

        for (int ii = 0; ii < MAX_PELLETS; ii++)
        {
            PelletData pD;
            pD.pelletEvent = new List<string>();
            pD.frameNum = new List<int>();

            pellData.Add(pD);
        }

     //   for (int pelInd = 0; pelInd < 500000; pelInd++)
     //  {
     //       SaveData2 sd2 = new SaveData2();
     //       sd2.pelletEvent = new List<string>(20);
     //       saveData2.Add(sd2);
     //   }


        loadParams();

        checkCondition();

        ui_image.enabled = true;
        ui_text.text = "Ready to practice?";

        //allocate memory for Data to save
        data_beeFree.allPositions = new List<Vector3>(100000);
        data_beeFree.allTimeStamps = new List<float>(100000);

        data_beeRestrict.allPositions = new List<Vector3>(100000);
        data_beeRestrict.allTimeStamps = new List<float>(100000);

        //find all the environment gameObjects and assign them to variables
        work_cells = GameObject.FindGameObjectWithTag("work_row_middle");
        drop_cells = GameObject.FindGameObjectWithTag("drop_cells");
        spawn_cells = GameObject.FindGameObjectWithTag("spawn_cells");
        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestrict = GameObject.FindGameObjectWithTag("bee_restricted");

        ScToWRatio = 1080 / (2 * Camera.main.orthographicSize);

        for (int ii = 0; ii < work_cells.transform.childCount; ii++)
            work_cells.transform.GetChild(ii).position = new Vector3(0, +ii * 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio));// + 3 * 108 / (2 * ScToWRatio));

        for (int ii = 0; ii < drop_cells.transform.childCount; ii++)
            drop_cells.transform.GetChild(ii).position = new Vector3(-5.4f * (6.0f / 5) - 5.4f / 10, ii * 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio));

        for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
            spawn_cells.transform.GetChild(ii).position = new Vector3(5.4f * (6.0f / 5), ii * 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio));

    }

    // Update is called once per frame
    void Update()
    {

        float timestamp = Time.time - timeIni;

        if (running_)
        {
            current_frame++;

            if (Input.GetKey("escape")) Application.Quit();

            if (Time.time > next_spawn && current_spawn<spawn_rate.Count)
            {
               
                List<int> avail_spawn = new List<int>();

                for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
                {
                    if (!spawn_cells.transform.GetChild(ii).GetComponent<SpawnCellScript>().contact_on)
                        avail_spawn.Add(ii);
                }

                if (avail_spawn.Count > 0)
                {
                    next_spawn = Time.time + spawn_rate[current_spawn];
                    current_spawn++;

                    int rand_ini = Random.Range(0, avail_spawn.Count);

                    GameObject pel_ = Instantiate(foodPrefab, spawn_cells.transform.GetChild(avail_spawn[rand_ini]).position, Quaternion.identity);
                    pel_.GetComponent<PelletScript>().pelletID = current_spawn - 1;

                    if (!practice_mode){                  
                        pel_.GetComponent<PelletScript>().saveToBuffer("SPAWN");
                    }

                }
            }

            data_beeFree.allTimeStamps.Add(timestamp);
            data_beeRestrict.allTimeStamps.Add(timestamp);

            data_beeFree.allPositions.Add(beeFree.transform.position);
            data_beeRestrict.allPositions.Add(beeRestrict.transform.position);

           
        }
        else if (!(current_cond == conditions_.Count - 1))
        {
            if (Input.GetKey("space"))
            {
                ui_image.enabled = false;
                ui_text.text = "";

                beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
                beeRestrict.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;

                running_ = true;

                timeIni = Time.time;
            }

        }
        else
        {
            if (Input.GetKey("escape")) Application.Quit();
        }

    }

    public void addPellet(GameObject pell_)
    {

        dropped_pellets.Add(pell_);

        //   pellets_dropped++;
        Debug.Log("pellet count = " + dropped_pellets.Count);

        if (dropped_pellets.Count == MAX_PELLETS)
        {
            running_ = false;

            // 
            if (!practice_mode){
                saveToDisk();
            }

            //RESET EVERYTHING
            dropped_pellets.Clear();

            GameObject[] allPellets = GameObject.FindGameObjectsWithTag("pellet");
            foreach (GameObject ggoo in allPellets) Destroy(ggoo);

            beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
            beeRestrict.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;

            beeFree.transform.position = new Vector3(-3.25f, 0, 0);
            beeRestrict.transform.position = new Vector3(3.25f, 0, 0);

            current_spawn = 0;
            next_spawn = 0;
            current_frame = 0;

            for (int ii = 0; ii < drop_cells.transform.childCount; ii++)
            {
                drop_cells.transform.GetChild(ii).GetComponent<DropCellScript>().dropTapCounter = 0;
                drop_cells.transform.GetChild(ii).GetComponent<DropCellScript>().pelletCounter = 0;
            }

            for (int ii = 0; ii < work_cells.transform.childCount; ii++)
            {
                work_cells.transform.GetChild(ii).GetComponent<workCellScript>().resetParameters();
            }

            for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
            {
                spawn_cells.transform.GetChild(ii).GetComponent<SpawnCellScript>().contact_on = false;
            }

            // practice round runs only once
            // if more attempts are needed, for now they just restart game
            if (practice_mode){
                practice_mode = false;
            }
            else{
                current_cond++;
                current_trial++;
            }            

            checkCondition();

            for (int ii = 0; ii < MAX_PELLETS; ii++)
            {
                pellData[ii].pelletEvent.Clear();
                pellData[ii].frameNum.Clear();
            }

        }

    }

    void checkCondition()
    {
        switch (conditions_[current_cond])
        {
            case CONDITION_CONSTANT_RATE:

                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Ready for next round?";

                for (int ii = 0; ii < MAX_PELLETS; ii++)
                    spawn_rate[ii] = 5.0f;//.Add(5.0f);

                break;

            case CONDITION_INCREASING_RATE:
                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Ready for next round?";

                for (int ii = 0; ii < MAX_PELLETS; ii++)
                {
                    spawn_rate[ii] = 7.0f - ii * 0.25f;

                    if (spawn_rate[ii] <= 0.25f) spawn_rate[ii] = 0.25f;
                }

                break;

            case CONDITION_RANDOM_RATE:
                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Ready for next round?";

                for (int ii = 0; ii < MAX_PELLETS; ii++)
                    spawn_rate[ii] = Random.Range(0.5f, 6.0f);
                break;

            case CONDITION_EXPERIMENT_OVER:

                ui_image.enabled = true;
                ui_text.text = "Game Over. \n Thanks for participating!";

                break;

            default:
                break;
        }

    }

    void saveToDisk()
    {
        string filetitle = RA_initials + "-" + ParticipantNumber;
        string path = "Assets/Resources/Data/"+ filetitle + ".csv";
        
        StreamWriter writer = new StreamWriter(path, true);

        if(current_cond ==0 && practice_mode)
        {
            string header_string = "Date,Participant,RA,Trial,Condition,Timestamp,BeeOldX,BeeOldY,BeeYoungX,BeeYoungY";
            writer.WriteLine(header_string);
        }

        if(current_cond ==0 && !practice_mode)
        {
            string header_string = "Date,Participant,RA,Trial,Condition,Timestamp,BeeOldX,BeeOldY,BeeYoungX,BeeYoungY";

            for (int jj = 0; jj < MAX_PELLETS; jj++)
                header_string = header_string + ",Pellet" + jj+"Event";

            writer.WriteLine(header_string);
        }

        List<List<string>> all_pell_ev = new List<List<string>>();

        for(int ii = 0; ii< MAX_PELLETS; ii++)
        {
            List<string> _pell = new List<string>(new string[data_beeFree.allPositions.Count]);
            all_pell_ev.Add(_pell);
        }

        for (int pelInd = 0; pelInd < MAX_PELLETS; pelInd++)
        {
            for (int frameInd = 0; frameInd < pellData[pelInd].frameNum.Count; frameInd++)
            {
                all_pell_ev[pelInd][pellData[pelInd].frameNum[frameInd]] = pellData[pelInd].pelletEvent[frameInd];
            }

        }


        List<string> pell_ = new List<string>(new string [data_beeFree.allPositions.Count]);


        for(int ii = 0; ii< pellData[0].frameNum.Count; ii++)
            pell_[pellData[0].frameNum[ii]] = pellData[0].pelletEvent[ii];
            

        for (int ii = 0; ii < data_beeFree.allPositions.Count; ii++)
        {
            string next_line = System.DateTime.Now + "," + ParticipantNumber + "," + RA_initials + "," + current_trial + "," + conditions_[current_cond]
                + "," + data_beeFree.allTimeStamps[ii] + "," + data_beeFree.allPositions[ii].x + ","
                + data_beeFree.allPositions[ii].y + "," + data_beeRestrict.allPositions[ii].x + "," + data_beeRestrict.allPositions[ii].y;
             // + "," + all_pell_ev[0][ii];// pell_[ii];

            for(int jj =0; jj< all_pell_ev.Count; jj++)
                next_line = next_line + "," + all_pell_ev[jj][ii];

            writer.WriteLine(next_line);

        }
        writer.Close();

    }

    void saveAndQuit()
    {
        quitting = true;
        string path = "Assets/Resources/Data/RATest.csv";
        StreamWriter writer = new StreamWriter(path, true);

        for (int ii = 0; ii < data_beeFree.allPositions.Count; ii++)
        {
            string next_line = data_beeFree.allTimeStamps[ii] + "," + data_beeFree.allPositions[ii].x + "," + data_beeFree.allPositions[ii].y + "," + data_beeRestrict.allPositions[ii].x + "," + data_beeRestrict.allPositions[ii].y;

            writer.WriteLine(next_line);

        }
        writer.Close();

        Application.Quit();

    }

    void loadParams()
    {

        int partOffSet = 1 + (ParticipantNumber - 1) * 6;

        TextAsset iniFile = Resources.Load<TextAsset>("loadParams/participant_sheet2");

        string[] row = iniFile.text.Split(new char[] { '\n' });


        for (int ii = 0; ii < 6; ii++)
        {
            string[] col = row[partOffSet + ii].Split(new char[] { ',' });

            int temptemp;
            int.TryParse(col[1], out temptemp);
            conditions_.Add(temptemp);
        }

        foreach (int iii in conditions_) Debug.Log(iii);

        //add the stopping condition at the end.
        conditions_.Add(CONDITION_EXPERIMENT_OVER);

    }


    void saveToBuffer(float timeNow, int evendCode, int conditionNow, int targ)
    {
        /* //save to buffer
         dataSave_.time_stamp.Add(timeNow);
         dataSave_.event_code.Add(evendCode);
         dataSave_.current_cond.Add(conditionNow);
         dataSave_.target_optotype.Add(targ);
         dataSave_.target_x.Add(0);
         dataSave_.target_y.Add(0);*/
    }



}
