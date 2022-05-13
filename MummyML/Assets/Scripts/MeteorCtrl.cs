using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MeteorCtrl : Agent
{
    private StageManager stageManager;
    private Transform tr;
    private Rigidbody rb;
    float currentTime;

    public override void Initialize()
    {
        MaxStep = 6000;

        stageManager = transform.root.GetComponent<StageManager>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();


    }

    public override void OnEpisodeBegin()
    {
        stageManager.InitStage();

        //�������� �ʱ�ȭ
        rb.velocity = rb.angularVelocity = Vector3.zero;
        //������Ʈ�� ��ġ����
        tr.localPosition = new Vector3(Random.Range(-20f, 20.0f), 0.05f, Random.Range(-20, 20));
        tr.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        
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
            case 1: dir = tr.forward; break; //����
            case 2: dir = -tr.forward; break; //����
        }
        switch (action[1])
        {
            case 1: rot = -tr.up; break; //��ȸ��
            case 2: rot = tr.up; break; //��ȸ��
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
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2; //����
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
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GOOD_ITEM"))
        {
            AddReward(+1.0f);
            rb.velocity = rb.angularVelocity = Vector3.zero;
            Destroy(collision.gameObject);
        }

       

        if (collision.collider.CompareTag("METEOR"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        if (collision.collider.CompareTag("DEAD_ZONE"))
        {
            AddReward(-0.5f);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BUNKER"))
        {
            currentTime += Time.deltaTime;
            AddReward(+0.001f);
            if (currentTime > 3)
            {
                AddReward(-1f);
                currentTime = 0;
            }

        }
    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BUNKER"))
        {
            currentTime = 0;
        }
    }
}
