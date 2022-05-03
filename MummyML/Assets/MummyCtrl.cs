using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/*
    �ֺ� ȯ���� ����(Observations)
    ��å�� ���� �ൿ(Actions)
    ����(Reward)
*/

public class MummyCtrl : Agent
{
    private Transform tr;
    private Rigidbody rb;

    [System.NonSerialized]
    public Transform targetTr;

    public Material goodMT, madMT;
    private Material originMT;
    private Renderer floor;

    // �ʱ�ȭ �۾�
    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        targetTr = tr.parent.Find("Target").GetComponent<Transform>();
        floor = tr.root.Find("Floor").GetComponent<MeshRenderer>();
        originMT = floor.material;
    }

    // �н�(���Ǽҵ�)�� ���۵� �� ���� ȣ��Ǵ� �ݹ�
    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ��ġ �ʱ�ȭ
        tr.localPosition = new Vector3(Random.Range(-4.0f, +4.0f),
                                        0.05f,
                                        Random.Range(-4.0f, 4.0f));

        targetTr.localPosition = new Vector3(Random.Range(-4.0f, +4.0f),
                                        0.55f,
                                        Random.Range(-4.0f, 4.0f));

    }

    // �ֺ�ȯ���� ���� �� ���������� �극�� ����
    public override void CollectObservations(VectorSensor sensor)
    {
        /*
            ��ġ���� (Vector Observation)
            - ���� ��ġ (Continues) : -1.0f ~ 0.0f ~ +1.0f
            - �̻� ��ġ (Discrete)  : -1.0f, 0.0f, +1.0f 
        */

        // Ÿ���� ��ġ ����
        sensor.AddObservation(targetTr.localPosition);      // 3

        // �ڽ��� ��ġ�� ����
        sensor.AddObservation(tr.localPosition);            // 3

        // �ӵ� ����
        sensor.AddObservation(rb.velocity.x);               // 1
        sensor.AddObservation(rb.velocity.z);               // 1
    }

    // �극������ ���� ���� ���� ���
    public override void OnActionReceived(ActionBuffers actions)
    {
        // ����/����
        // ����/������

        // ���޹��� ��ɴ�� �ൿ(Action)
        var action = actions.ContinuousActions;

        //Debug.Log($"[0]={action[0]}, [1]={action[1]}");

        Vector3 dir = (Vector3.forward * action[0]) + (Vector3.right * action[1]);
        rb.AddForce(dir.normalized * 30.0f);

        // �������� �������� �����ϱ� ���� ���̳ʽ� ���Ƽ
        SetReward(-0.001f);
    }

    // ������ �׽�Ʈ�� ���� ���
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;
        // ����/����
        action[0] = Input.GetAxis("Vertical"); // Up/Down, W/S  -1.0f ~ 0.0f ~ +1.0f
        // ��/��
        action[1] = Input.GetAxis("Horizontal"); // Left/Right , A/D
    }


     void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DEAD_ZONE"))
        {
            StartCoroutine(this.ChangeColor(madMT));
            SetReward(-1.0f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("TARGET"))
        {
            StartCoroutine(this.ChangeColor(goodMT));
            SetReward(+1.0f);
            EndEpisode();
        }
    }

    IEnumerator ChangeColor(Material changeMT)
    {
        floor.material = changeMT;

        yield return new WaitForSeconds(0.2f);

        floor.material = originMT;
    }
}