using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentScript2 : Agent
{
    // [SerializeField] private Transform targetTransform;
    public Transform hammerHead;
    public Transform body;
    public Transform target;

    public float maxRange = 2.0f;

    private float startTime;
    // public Transform hammerTransform;
    void Start() {
        Physics2D.IgnoreCollision(hammerHead.GetComponent<Collider2D>(),
                                  body.GetComponent<Collider2D>());
        // get current time
        startTime = 100f;
    }
    public override void OnEpisodeBegin(){
        Debug.Log("Begining new episode");
        body.position = new Vector3(7.66f,-0.02f,0);
        hammerHead.position = new Vector3(7.66f,-0.02f,0);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(body.position);
        sensor.AddObservation(hammerHead.position);
        // get distance between target and body
        float distance = Vector3.Distance(body.position, target.position);
        
        sensor.AddObservation(distance);
        // sensor.AddObservation(targetTransform.position);

    }
    public override void OnActionReceived(ActionBuffers actions){
        // Debug.Log(actions.ContinuousActions[0]);
        float moveX = actions.ContinuousActions[0] * 10;
        float moveY = actions.ContinuousActions[1] * 10;
     
        // float moveSpeed = 1f;
        // float depth = Mathf.Abs(Camera.main.transform.position.z);

        // transform.position += new Vector3(moveX,moveY,depth)*Time.deltaTime * moveSpeed;
        float depth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 center =
            new Vector3(Screen.width / 2, Screen.height / 2, depth);
        Vector3 mouse =
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth);

        // Transform to world space
        center = Camera.main.ScreenToWorldPoint(center);
        // mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse = new Vector3(moveX, moveY, 0);
        mouse *= 10;

        // Compute mouseVec for hammer control
        Vector3 mouseVec = Vector3.ClampMagnitude(mouse - center, maxRange);

        // hammerHead.GetComponent<Rigidbody2D>().MovePosition(body.position +
        // mouseVec); return;

        // Check if hammer head is collided with scene objects
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = LayerMask.GetMask("Default");
        Collider2D[] results = new Collider2D[5];
        if (hammerHead.GetComponent<Rigidbody2D>().OverlapCollider(
                contactFilter, results) > 0)  // If collided with scene objects
        {
            // Update body pos
            Vector3 targetBodyPos = hammerHead.position - mouseVec;

            Vector3 force = (targetBodyPos - body.position) * 80.0f;
            body.GetComponent<Rigidbody2D>().AddForce(force);

            body.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(
                body.GetComponent<Rigidbody2D>().velocity, 6);
        }

        // Compute new hammer pos
        Vector3 newHammerPos = body.position + mouseVec;
        Vector3 hammerMoveVec = newHammerPos - hammerHead.position;
        newHammerPos = hammerHead.position + hammerMoveVec * 0.2f;

        // Update hammer pos
        hammerHead.GetComponent<Rigidbody2D>().MovePosition(newHammerPos);

        // Update hammer rotation
        hammerHead.rotation = Quaternion.FromToRotation(
            Vector3.right, newHammerPos - body.position);
    }
    

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.mousePosition.x;
        continuousActions[1] = Input.mousePosition.y;
    }
    private void Update() {
        float currentY = body.position.y;
        float currentX = body.position.x;
        
        // if(currentY > prevY){
        //     AddReward(0.1f);
        // }
        // prevY = currentY;
        // SetReward(currentY);
        float reward = currentY;
        AddReward(reward);
        startTime -= Time.deltaTime;
        // Debug time based episode setting
        // Debug.Log("start time" + startTime);
        // Debug.Log(Time.time);
        // Debug.Log(GetCumulativeReward());
        if(startTime <= 0){
            Debug.Log("Episode Ended. \t Reward" + GetCumulativeReward());
            startTime = 400;
            EndEpisode();
        }
        if (body.position.y < -3) {
            AddReward(-500f);
            Debug.Log("Episode Ended. \t Reward" + GetCumulativeReward());
            EndEpisode();
        }
        // if body and hammer are too far away reset
        // Get distance between two vectors
        float distance1 = Vector3.Distance(body.position, hammerHead.position);
        // Debug.Log("Distance: " + distance1);
        if(distance1 > 3){
            Debug.Log("hammer dist");
            AddReward(-50f);
            EndEpisode();
        }
    }
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if(other.gameObject.CompareTag("Goal")){
    //         AddReward(100f);
    //         Debug.Log("Episode Ended. \t Reward" + GetCumulativeReward());
    //         EndEpisode();
    //     }
    // }

}
