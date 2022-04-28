using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildGoalReach : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Goal"){
            Debug.Log("Reached Goal");
            player.GetComponent<AgentScript2>().AddReward(1000f);
            Debug.Log("Episode Ended. \t Reward" + player.GetComponent<AgentScript2>().GetCumulativeReward());
            player.GetComponent<AgentScript2>().EndEpisode();
        }
    }
}
