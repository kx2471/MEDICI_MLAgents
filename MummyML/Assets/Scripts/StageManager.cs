using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public GameObject goodItem;
    public GameObject guardItem;
    public GameObject meteor;

    float currentTime;

   
    [Range (5, 50)]
    public int guardItemCount = 5;
    public int goodItemCount = 15;


    public List<GameObject> goodItemList = new List<GameObject>();
    public List<GameObject> guardItemList = new List<GameObject> ();

    public void InitStage()
    {
        foreach (var obj in goodItemList)
        {
            Destroy(obj);
        }
        goodItemList.Clear();

        foreach (var obj in guardItemList)
        {
            Destroy(obj);
        }
        guardItemList.Clear ();

      

        for (int i = 0; i < guardItemCount; i++)
        {
            Vector3 pos = new Vector3
                (Random.Range(-20.0f, 20.0f), 1.8f, Random.Range(-20.0f, 20.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));

            guardItemList.Add(Instantiate(guardItem, transform.position + pos, rot, transform));
        }

        for (int i = 0; i < goodItemCount; i++)
        {
            Vector3 pos = new Vector3
                (Random.Range(-20.0f, 20.0f), 0.8f, Random.Range(-20.0f, 20.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));

            goodItemList.Add(Instantiate(goodItem, transform.position + pos, rot, transform));
        }



    }


    // Start is called before the first frame update
    void Start()
    {
        InitStage();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > 1.5)
        {
            for (int i = 0; i < 15; i++)
            {
                Instantiate(meteor, new Vector3(Random.Range(-20.0f, 20.0f), 30f, Random.Range(-20.0f, 20.0f)) + transform.position, Quaternion.identity);
            }
            currentTime = 0;
        }
    }

            
}
