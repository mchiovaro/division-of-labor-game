using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Exit_app_script : MonoBehaviour
{
    float ScToWRatio; // some camera ratio?
    GameObject work_cells, drop_cells, spawn_cells;
    public GameObject foodPrefab, beeFree, beeRestrict;

    // input parameters
    public int ParticipantNumber; // pulls from the excel sheet with conditions
    public string RA_initials;

    // trial counter
    public int current_trial = 0;

    // max number of pellets based on number of cells available
    private const int MAX_PELLETS = 20;

    // initialize conditions list
    public List<int> td_condition = new List<int>(7);
    public List<int> ie_condition = new List<int>(1);
    public List<int> com_condition = new List<int>(1);
    public List<int> size_condition = new List<int>(1);

    // ending experiment condition
    private const int CONDITION_EXPERIMENT_OVER = 0;

    // conditions for task demand
    public const int FREE_HARD = 1;
    public const int RESTRICT_HARD = 2;
    public const int BOTH_HARD = 3;

    // condition for individual effectivities
    public const int FREE_PASS = 1;
    public const int RESTRICT_PASS = 2;

    // condition for communication
    public const int TALK_SILENCE = 1;
    public const int TALK_BUTTONS = 2;
    public const int TALK_FREE = 3;

    // rates of incoming food pellets
    private const int CONDITION_CONSTANT_RATE = 1;
    //private const int CONDITION_INCREASING_RATE = 2;
    //private const int CONDITION_RANDOM_RATE = 3;

    // initial conditions
    // current_cond is really just another trial counter, but then pulls from the list of conditions
    private int current_cond = 0;
    private bool practice_mode = false; // true to include the practice trial // FIX

    // allows for the rate of spawn to be changed in condition checkCondition()
    private List<float> spawn_rate = new List<float>(new float[MAX_PELLETS]);

    // counter for number of dropped pellets
    private List<GameObject> dropped_pellets = new List<GameObject>();

    // trial running, spawn rate, and frame variables
    private bool running_ = false;
    private float next_spawn = 0.0f;
    private int current_spawn = 0;
    public int current_frame = 0;

    // pellet counter
    public int pellets_dropped = 0;

    // set up for inbetween trials
    public Text ui_text;
    public RawImage ui_image;

    // data structures for position and time
    public struct SaveData
    {
        public List<Vector3> allPositions;
        public List<float> allTimeStamps;
    }

    // data structures for pellet events and frames
    public struct PelletData
    {
        public List<string> pelletEvent;
        public List<int> frameNum;
    }

    public List<PelletData> pellData = new List<PelletData>();

    // variables for data structures
    public SaveData data_beeFree, data_beeRestrict;
    private bool quitting = false;

    // old time variable to reset time after each trial
    //private float timeIni = 0;

    // Start is called before the first frame update
    void Start()
    {
        // create list/variable for saving pellet data
        for (int ii = 0; ii < MAX_PELLETS; ii++)
        {
            //
            PelletData pD;
            pD.pelletEvent = new List<string>();
            pD.frameNum = new List<int>();

            pellData.Add(pD); //
        }

        // set spawn rate to every 2 seconds
        for (int ii = 0; ii < MAX_PELLETS; ii++)
            spawn_rate[ii] = 2.0f;

        // set up parameters and conditions
        loadParams();
        //checkCondition();

        // set up practice screen
        ui_image.enabled = true;
        ui_text.text = "Ready to practice?";

        //allocate memory for Data to save
        data_beeFree.allPositions = new List<Vector3>(100000); // old bee
        data_beeFree.allTimeStamps = new List<float>(100000); // old bee
        data_beeRestrict.allPositions = new List<Vector3>(100000); // young bee
        data_beeRestrict.allTimeStamps = new List<float>(100000); // young bee

        //find all the environment gameObjects and assign them to variables
        work_cells = GameObject.FindGameObjectWithTag("work_row_middle");
        drop_cells = GameObject.FindGameObjectWithTag("drop_cells");
        spawn_cells = GameObject.FindGameObjectWithTag("spawn_cells");
        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestrict = GameObject.FindGameObjectWithTag("bee_restricted");

        // set camera ratio
        ScToWRatio = 1080 / (2 * Camera.main.orthographicSize);

        // setting up camera ratio of the field
        float ratio = 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio);

    }

    // Update() is called once per frame
    void Update()
    {
        //float timestamp = Time.time - timeIni; // brings timestamp back to zero before each trial
        float timestamp = Time.time; // keeping the time relative to the start of the game, not reseting at each trial

        // if we are currently in a trial
        if (running_)
        {
            current_frame++;

            // allow for escape quitting
            if (Input.GetKey("escape")) Application.Quit();

            //
            if (Time.time > next_spawn && current_spawn < spawn_rate.Count)
            {

                List<int> avail_spawn = new List<int>();

                for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
                {
                    if (!spawn_cells.transform.GetChild(ii).GetComponent<SpawnCellScript>().contact_on)
                        avail_spawn.Add(ii);
                }

                // if there are more pellets, keep spawning
                if (avail_spawn.Count > 0)
                {
                    next_spawn = Time.time + spawn_rate[current_spawn];
                    current_spawn++;

                    int rand_ini = Random.Range(0, avail_spawn.Count);

                    // spawn a pellet prefab
                    GameObject pel_ = Instantiate(foodPrefab, spawn_cells.transform.GetChild(avail_spawn[rand_ini]).position, Quaternion.identity);
                    pel_.GetComponent<PelletScript>().pelletID = current_spawn - 1;
                    //Debug.Log("pellet spawned");

                    if (!practice_mode) {
                        pel_.GetComponent<PelletScript>().saveToBuffer("SPAWN");
                    }
                }
            }

            // grab timstamp for each player and add to allTimeStamps (x, y coord)
            data_beeFree.allTimeStamps.Add(timestamp);
            data_beeRestrict.allTimeStamps.Add(timestamp);

            // grab position for each player and add to allTimeStamps (x, y coord)
            data_beeFree.allPositions.Add(beeFree.transform.position);
            data_beeRestrict.allPositions.Add(beeRestrict.transform.position);

        }

        // what's this doing?
        else if (!(current_cond == td_condition.Count - 1))
        {
            // if space bar is pressed between trials
            if (Input.GetKey("space"))
            {
                // remove the waiting screen and text
                ui_image.enabled = false;
                ui_text.text = "";

                // allow for touch behaviors
                beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
                beeRestrict.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;

                // run the game
                running_ = true;

                // start of game time
                //timeIni = Time.time; // time in seconds since game was launched
            }

        }

        else //allow for quitting
        {
            // allow for quitting with escape key
            if (Input.GetKey("escape")) Application.Quit();
        }

    }

    // fixed rate for saving position data (50 samples per second)
    private void FixedUpdate()
    {
        // save data only for real trials and only while running
        if (!(practice_mode) && running_)
        {
            // set up saving location
            string filetitle = RA_initials + "-" + ParticipantNumber;
            string path = "Assets/Resources/Data/" + filetitle + "-movement_data.csv";

            // create writer
            StreamWriter writer = new StreamWriter(path, true);

            // create the string with the participant position data
            string next_line = System.DateTime.Now + "," + ParticipantNumber + ","
                    + RA_initials + "," + current_trial + ","
                    + td_condition[current_cond] + "," + Time.time + ","
                    + beeFree.transform.position.x + ","
                    + beeFree.transform.position.y + ","
                    + beeRestrict.transform.position.x + ","
                    + beeRestrict.transform.position.y;

            // write the line
            writer.WriteLine(next_line);

            // close the writer
            writer.Close();
        }
    }

    // adding pellet data
    public void addPellet(GameObject pell_)
    {

        dropped_pellets.Add(pell_);

        //   pellets_dropped++;
        Debug.Log("pellet count = " + dropped_pellets.Count);

        if (dropped_pellets.Count == MAX_PELLETS)
        {
            running_ = false;

            // save pellet data if it's not from practice
            if (!practice_mode){
                saveToDisk();
            }

            //RESETS EVERYTHING
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
            else {
                current_cond++;
                current_trial++;
            }

            checkCondition();

            // clear pellet data
            for (int ii = 0; ii < MAX_PELLETS; ii++)
            {
                pellData[ii].pelletEvent.Clear();
                pellData[ii].frameNum.Clear();
            }

        }

    }

    void checkCondition()
    {
        switch (td_condition[current_cond])
        {


            // constant rate condition
            case CONDITION_CONSTANT_RATE:

                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Ready for next round?";

                // keeps spawn rate at 5.0f
                //for (int ii = 0; ii < MAX_PELLETS; ii++)
                //    spawn_rate[ii] = 5.0f;

                break;

            // end screen
            case CONDITION_EXPERIMENT_OVER:

                ui_image.enabled = true;
                ui_text.text = "Game Over. \n Thanks for participating!";

                break;

            default:
                break;
        }

    }

    // save pellet data here at Update() rate, not FixedUpdate()
    void saveToDisk()
    {

        string filetitle = RA_initials + "-" + ParticipantNumber;
        string path = "Assets/Resources/Data/"+ filetitle + "-pellet_data.csv";

        StreamWriter writer = new StreamWriter(path, true);


        /* if(current_cond ==0 && practice_mode)
        {
            string header_string = "Date,Participant,RA,Trial,Condition,Timestamp,BeeOldX,BeeOldY,BeeYoungX,BeeYoungY";
            writer.WriteLine(header_string);
        } */

        // if this is the first real round
        if(current_cond ==0 && !practice_mode)
        {
            string header_string = "Date,Participant,RA,Trial,Condition,Timestamp";

            // create column for max number of pellets
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

        // new string for pellet data?
        List<string> pell_ = new List<string>(new string [data_beeFree.allPositions.Count]);

        for(int ii = 0; ii< pellData[0].frameNum.Count; ii++)
            pell_[pellData[0].frameNum[ii]] = pellData[0].pelletEvent[ii];

        // save data
        for (int ii = 0; ii < data_beeFree.allPositions.Count; ii++)
        {
            string next_line = System.DateTime.Now + "," + ParticipantNumber + "," + RA_initials + "," + current_trial + "," + td_condition[current_cond]
                + "," + data_beeFree.allTimeStamps[ii];

            /* This loop is the reason for the duplicate time stamps (I think).
            * It takes each pellet and creates a line for any pellet event.
            * If two pellet events happen in the same frame, it will create
            * two separate lines. We will just need to collapse across duplicate
            * timestamps to see all pellet events at the moment. */
            for (int jj =0; jj< all_pell_ev.Count; jj++)
                next_line = next_line + "," + all_pell_ev[jj][ii];

            writer.WriteLine(next_line);

        }

        writer.Close();

    }

    // grabbing randomization of condition types for trials
    void loadParams()
    {

        // read in condition orders
        TextAsset iniFile = Resources.Load<TextAsset>("loadParams/conditions");

        // create string to split out rows of conditions
        string[] row = iniFile.text.Split(new char[] { '\n' });

        // create string to split columns of conditions for the row
        string[] col = row[ParticipantNumber].Split(new char[] { ',' });

    // td_condition

        // increase by 1 until we hit six
        for (int ii = 2; ii < 8; ii++)
        {

            // create temp variable to hold the condition
            int temp_td;

            // parse apart conditions from file going down column 1
            int.TryParse(col[ii], out temp_td);
            Debug.Log("td_condition = " + temp_td);

            // add the condition to a string
            td_condition.Add(temp_td);
        }

        //add the stopping condition at the end
        td_condition.Add(CONDITION_EXPERIMENT_OVER);

        // made sure td_condition is being generated correctly (with 0 at the end for closing the game)
        //Debug.Log("td_condition = " + td_condition[0] + td_condition[1] + td_condition[2] + td_condition[3] + td_condition[4] + td_condition[5] + td_condition[6]);

    // ie_condition

        // create temp variable to hold the condition
        int temp_ie;

        // parse apart conditions from file going down column 1
        int.TryParse(col[1], out temp_ie);
        Debug.Log("ie_condition = " + temp_ie);

        // add the condition to a string
        ie_condition.Add(temp_ie);

    // com_condition

        // create temp variable to hold the condition
        int temp_com;

        // parse apart conditions from file going down column 1
        int.TryParse(col[8], out temp_com);
        Debug.Log("com_condition = " + temp_com);

        // add the condition to a string
        com_condition.Add(temp_com);

    // size_condition: number of participants

        // create temp variable to hold the condition
        int temp_size;

        // parse apart conditions from file going down column 1
        int.TryParse(col[9], out temp_size);
        Debug.Log("size_condition = " + temp_size);

        // add the condition to a string
        com_condition.Add(temp_size);

    }

}
