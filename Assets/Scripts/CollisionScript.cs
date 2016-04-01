using UnityEngine;
using System.Collections;

public class CollisionScript : MonoBehaviour
{

    GameObject HUDCanvas;
    ScoreScript ss;

    // Use this for initialization
    void Start()
    {
        HUDCanvas = GameObject.FindGameObjectWithTag("HUD");
        if (HUDCanvas != null) {
            Transform child = transform.Find("/HUDCanvas/Score");
            ss = child.gameObject.GetComponent<ScoreScript>();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        string name = col.gameObject.name;
        if (name.Contains("Body"))
        {

            // depletes and checks health
            Transform parent = col.gameObject.transform.parent;
            PlayerScript player = parent.gameObject.GetComponent<PlayerScript>();

            if (player.PlayerHealth != 0)
            {
                if (player.state == PlayerScript.fightState.BLOCK) // reduce damage if blocking
                    player.depleteHealth(5);
                else player.depleteHealth(20); // lose more health

                if (player.PlayerHealth <= 0) // if the player is knocked out
                {
                    foreach (Transform child in parent)
                    {
                        // sends all object flying
                        Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
                        rb.isKinematic = false;
                        rb.useGravity = true;

                        // random fall animation
                        Vector3 dir = -rb.transform.forward * (400f + Random.value * 250.0f);
                        dir += new Vector3(0, 50 + Random.value * 50, 0);
                        rb.AddForce(dir);
                    }

                    if (player.AI == true)
                    {
                        ss.AddScore(); // adds to the player's score

                        // recover player's health
                        parent = gameObject.transform.parent;
                        player = parent.gameObject.GetComponent<PlayerScript>();
                        player.depleteHealth(-25);
                        player.setEnabled(false);
                    }
                }
            }

            //        else col.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
