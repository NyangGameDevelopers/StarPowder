using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Status))]
public class CasterCore : MonoBehaviour
{
    public Status status { get; private set; }
    void Awake()
    {
        status = GetComponent<Status>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Cast(SkillData data){}
}
