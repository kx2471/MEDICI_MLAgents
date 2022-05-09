using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    
    public GameObject badItem;

   
    [Range (10, 50)]
    public int badItemCount = 40;


    
    public List<GameObject> badItemList = new List<GameObject> ();

    public void InitStage()
    {
     
        foreach(var obj in badItemList)
        {
            Destroy(obj);
        }
        badItemList.Clear ();

      

        for (int i = 0; i < badItemCount; i++)
        {
            Vector3 pos = new Vector3
                (Random.Range(-23.0f, 23.0f), Random.Range(0.05f, 35), Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));

            badItemList.Add(Instantiate(badItem, transform.position + pos, rot, transform));
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
        
    }
}
