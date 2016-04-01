using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    // state of the player
    public enum fightState { IDLE, LEFTPUNCH, LEFTPULL, RIGHTPUNCH, RIGHTPULL, BLOCKING, BLOCK, BLOCKPULL, DEAD}
    public fightState state;

    public bool AI = true; // determines player or AI

    // controls punching/blocking animations
    Rigidbody LeftArm;
    Rigidbody RightArm;

    Vector3 leftPunchDir;
    Vector3 rightPunchDir;

    public int PlayerHealth = 100; // default health
    int TotalHealth;
    GameObject HealthBar; // controls appearance of health bar

    public float reactionTime = 1.0f; // default reaction time for AI
    float actionTime;
    float idleTime;

    float restartTime = 4.0f; // respawn time
    public int level = 1;

    bool controlEnabled = true;
    int gamePause = 0;

    // Use this for initialization
    void Start () {
        // sets up parameters for fighting
        Vector3 enemyHead;

        if (!AI) // player
        {
            LeftArm = GameObject.Find("/Player/Left Arm").GetComponent<Rigidbody>();
            RightArm = GameObject.Find("/Player/Right Arm").GetComponent<Rigidbody>();

            GameObject HUDCanvas = GameObject.FindGameObjectWithTag("HUD");
            if (HUDCanvas != null)
            {
                Transform child = HUDCanvas.transform.Find("PlayerHealth/Health");
                HealthBar = child.gameObject;
            }

            enemyHead = GameObject.FindGameObjectWithTag("Enemy").transform.position;
            reactionTime = 0.0f;
        }
        else { // computer
            gameObject.name = "Player 2";
            LeftArm = GameObject.Find("/Player 2/Left Arm").GetComponent<Rigidbody>();
            RightArm = GameObject.Find("/Player 2/Right Arm").GetComponent<Rigidbody>();

            GameObject HUDCanvas = GameObject.FindGameObjectWithTag("HUD");
            if (HUDCanvas != null)
            {
                Transform child = HUDCanvas.transform.Find("EnemyHealth/Health");
                HealthBar = child.gameObject;
            }

            enemyHead = GameObject.FindGameObjectWithTag("Player").transform.position;
        }

        // sets up idle and health
        idleTime = 0.0f;
        state = fightState.IDLE;
        actionTime = 0.0f;
        TotalHealth = PlayerHealth;
        depleteHealth(0);

        // sets up punching for players
        leftPunchDir = (enemyHead - new Vector3(0.2f, 0, 0) - LeftArm.position).normalized;
        rightPunchDir = (enemyHead + new Vector3(0.2f, 0, 0) - RightArm.position).normalized;
        RightArm.transform.forward = rightPunchDir;
        LeftArm.transform.forward = leftPunchDir;
    }

    void stop() {
        RightArm.velocity *= 0;
        LeftArm.velocity *= 0;
    }
    public void setHealth(int num)  // sets health
    {
        TotalHealth = num;
        PlayerHealth = num;
    }

    public void depleteHealth(int num) // deplete health by num
    {
        PlayerHealth -= num;

        if (PlayerHealth < 0) // if the player's health is zero
            PlayerHealth = 0;

        if (PlayerHealth > TotalHealth) // maxed out health
            PlayerHealth = 0;

        float health = PlayerHealth;

        // depletes UI HealthBar
        HealthBar.transform.localScale = new Vector2(health / TotalHealth, 1.0f);
    }

    public void setEnabled(bool enabled) // set player's controls
    {
        controlEnabled = enabled;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.P))
            gamePause = (gamePause + 1) % 2;

        if (gamePause == 1)
            Time.timeScale = 0;
        else Time.timeScale = 1;

        float distance = 0.3f * 7.8f;
        float speed = 7.8f;
        float time = distance / speed;

        float block_distance = 0.5f;
        float block_speed = 5.0f;
        float block_time = block_distance / block_speed;

        if (PlayerHealth == 0) // checks if the player is alive
            state = fightState.DEAD;

        switch (state)
        {
            case fightState.IDLE:
                if (idleTime >= reactionTime)
                {
                    if (controlEnabled && gamePause == 0)
                    {
                        int action = -1;
                        if (AI) // Random move for AI
                            action = Random.Range(0, 2);

                        else {
                            if (Input.GetKeyDown(KeyCode.A)) // attack with left arm
                                action = 0;

                            else if (Input.GetKeyDown(KeyCode.D)) // attack with right arm
                                action = 1;

                            else if (Input.GetKeyDown(KeyCode.S)) // block
                                action = 2;
                        }
                        if (action == 0)
                        { // attack with left arm
                            state = fightState.LEFTPUNCH;
                        }
                        else if (action == 1)
                        { // attack with left arm
                            state = fightState.RIGHTPUNCH;
                        }
                        else if (action == 2)
                        { // block with both arms
                            state = fightState.BLOCKING;
                        }

                        if (action != -1) // action is done
                            idleTime = 0;
                    }
                }
                else idleTime += Time.deltaTime;
                break;
            case fightState.LEFTPUNCH: // player punches with left
                if (actionTime + Time.deltaTime < time)
                {
                    LeftArm.position += LeftArm.transform.forward * speed * Time.deltaTime;
                    actionTime += Time.deltaTime;
                }
                else {
                    LeftArm.position += LeftArm.transform.forward * speed * (0.3f - actionTime);
                    actionTime = 0.3f;
                    state = fightState.LEFTPULL;
                }
                break;

            case fightState.LEFTPULL: // player pulls back left punch
                if (actionTime - Time.deltaTime > 0.0f)
                {
                    LeftArm.position -= LeftArm.transform.forward * speed * Time.deltaTime;
                    actionTime -= Time.deltaTime;
                }
                else {
                    LeftArm.position -= LeftArm.transform.forward * speed * (actionTime);
                    state = fightState.IDLE;
                    actionTime = 0;
                    stop();
                }
                break;

            case fightState.RIGHTPUNCH: // player punches with right
                if (actionTime + Time.deltaTime < time)
                {
                    RightArm.position += RightArm.transform.forward * speed * Time.deltaTime;
                    actionTime += Time.deltaTime;
                }
                else {
                    RightArm.position += RightArm.transform.forward * speed * (0.3f - actionTime);
                    actionTime = 0.3f;
                    state = fightState.RIGHTPULL;
                }
                break;

            case fightState.RIGHTPULL: // player pulls back right punch
                if (actionTime - Time.deltaTime > 0.0f)
                {
                    RightArm.position -= RightArm.transform.forward * speed * Time.deltaTime;
                    actionTime -= Time.deltaTime;
                }
                else {
                    RightArm.position -= RightArm.transform.forward * speed * (actionTime);
                    state = fightState.IDLE;
                    actionTime = 0;
                    stop();
                }
                break;

            case fightState.BLOCKING: // blocking position
                if (actionTime + Time.deltaTime < block_time) 
                {
                    LeftArm.transform.position += transform.up * block_speed * Time.deltaTime;
                    LeftArm.transform.position += transform.right * block_speed * Time.deltaTime;

                    RightArm.transform.position += transform.up * block_speed * Time.deltaTime;
                    RightArm.transform.position -= transform.right * block_speed * Time.deltaTime;
                    actionTime += Time.deltaTime;
                }
                else state = fightState.BLOCK;
                break;

            case fightState.BLOCK: // block: reduce damage
                if (Input.GetKeyDown(KeyCode.S)) { // press down again to cancel block 
                    state = fightState.BLOCKPULL;
                }
                break;
            case fightState.BLOCKPULL: // back to idle position
                if (actionTime - Time.deltaTime > 0) 
                {
                    LeftArm.transform.position -= transform.up * block_speed * Time.deltaTime;
                    LeftArm.transform.position -= transform.right * block_speed * Time.deltaTime;

                    RightArm.transform.position -= transform.up * block_speed * Time.deltaTime;
                    RightArm.transform.position += transform.right * block_speed * Time.deltaTime;
                    actionTime -= Time.deltaTime;
                }
                else {
                    LeftArm.transform.position -= transform.up * block_speed * actionTime;
                    LeftArm.transform.position -= transform.right * block_speed * actionTime;

                    RightArm.transform.position -= transform.up * block_speed * actionTime;
                    RightArm.transform.position += transform.right * block_speed * actionTime;

                    actionTime = 0;
                    state = fightState.IDLE;
                }
                break;
            case fightState.DEAD:
                restartTime -= Time.deltaTime;

                if (restartTime <= 0) {
                    if (AI)
                    { // if it is an AI

                        // respawn another AI
                        gameObject.name = "DEAD";
                        GameObject player = Instantiate(Resources.Load("Player 2")) as GameObject;
                        PlayerScript ps = player.GetComponent<PlayerScript>();

                        ps.level = level + 1; // increase AI difficulty

                        if ((level % 3) == 0) // makes the AI tougher
                        {
                            ps.setHealth(TotalHealth + 20);
                            if (reactionTime > 0.6)
                                ps.reactionTime = reactionTime - 0.1f;
                        }

                        // delete defeated AI
                        Destroy(gameObject);

                        // turns on human player control
                        GameObject.Find("/Player").GetComponent<PlayerScript>().setEnabled(true); 
                    }
                    else // if it is a player
                        // reload the level
                        SceneManager.LoadScene(0);
                }
                break;

        }
    }
}
