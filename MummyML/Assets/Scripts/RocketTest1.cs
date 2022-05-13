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
    public GameObject forwarddir;
    
    
    public override void Initialize()
    {
        MaxStep = 15000;

        stageManager = transform.root.GetComponent<StageManager>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        

    }

    public override void OnEpisodeBegin()
    {
        stageManager.InitStage();

        //�������� �ʱ�ȭ
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
            case 1: rb.AddForce(forwarddir.transform.up * 0.5f, ForceMode.Impulse); break; //����
            
        }
        switch (action[1])
        {
            case 1: rot = -tr.up; break; //��ȸ��
            case 2: rot = tr.up; break; //��ȸ��
            case 3: rot = -tr.right; break;
            case 4: rot = tr.right; break;

        }
        tr.Rotate(rot, Time.fixedDeltaTime * 200);
        rb.AddForce(dir * 0.5f, ForceMode.VelocityChange);

        //�������� �������� �����ϱ� ���� ���̳ʽ� ���Ƽ
        AddReward(-1 / (float)MaxStep); // -1/5000, -0.005

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        /*
         * 
         *  Branch 0 = 0, 1, 2 => 3��
         *  Branch 1 = 0, 1, 2 => 3��
         * 
         */



        var action = actionsOut.DiscreteActions;

        action.Clear();

        //����, ���� �̵�ó�� - Branch 0 = (0:����, 1:����, 2:����);
        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1; //����
        }
       
        

        //�¿� ȸ�� �̵�ó�� - Branch 1 = (0:��ȸ��, 1:��ȸ��, 2:��ȸ��);
        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1; //��ȸ��
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2; //��ȸ��
        }
        if (Input.GetKey(KeyCode.U))
        {
            action[1] = 3; //��ȸ��
        }
        if (Input.GetKey(KeyCode.J))
        {
            action[1] = 4; //��ȸ��
        }
    }


    void OnCollisionEnter(Collision collision)
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
