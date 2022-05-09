using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RocketTest1 : Agent
{
    private StageManager stageManager;
    private Transform tr;
    private Rigidbody rb;

    public override void Initialize()
    {
        MaxStep = 5000;

        stageManager = transform.root.GetComponent<StageManager>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();


    }

    public override void OnEpisodeBegin()
    {
        stageManager.InitStage();

        //물리엔진 초기화
        rb.velocity = rb.angularVelocity = Vector3.zero;
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions; //Discreate (0,1,2,3, ...)
        //Debug.Log($"[0] = {action[0]}, [1] = {action[1]}");

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        

        //Branch 0 ==> action[0]
        //Branch 1 ==> action[1]
        switch (action[0])
        {
            case 1: rb.AddForce(new Vector3(0, 0, 1) * 0.5f, ForceMode.Impulse); break; //전진
            case 2: rb.AddForce(new Vector3(1,0,0) * 0.5f, ForceMode.Impulse); break;//후진
            case 3: rb.AddForce(new Vector3(0, 1, 0) * 0.5f, ForceMode.Impulse); break;
        }
        switch (action[1])
        {
            case 1: rot = -tr.up; break; //좌회전
            case 2: rot = tr.up; break; //우회전
        }
        tr.Rotate(rot, Time.fixedDeltaTime * 200);
        rb.AddForce(dir * 0.5f, ForceMode.VelocityChange);

        //지속적인 움직임을 유도하기 위한 마이너스 페널티
        AddReward(-1 / (float)MaxStep); // -1/5000, -0.005

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        /*
         * 
         *  Branch 0 = 0, 1, 2 => 3개
         *  Branch 1 = 0, 1, 2 => 3개
         * 
         */



        var action = actionsOut.DiscreteActions;

        action.Clear();

        //전진, 후진 이동처리 - Branch 0 = (0:정지, 1:전진, 2:후진);
        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1; //전진
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2; //후진
        }
        if (Input.GetKey(KeyCode.U))
        {
            action[0] = 3;
        }
        

        //좌우 회전 이동처리 - Branch 1 = (0:무회전, 1:좌회전, 2:우회전);
        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1; //좌회전
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2; //우회전
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("BAD_ITEM"))
        {
            AddReward(+1.0f);
            Destroy(collision.gameObject);
        }
        if (collision.collider.CompareTag("DEAD_ZONE"))
        {
            AddReward(-0.01f);
        }

    }
}
