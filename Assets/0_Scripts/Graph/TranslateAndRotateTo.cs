using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateAndRotateTo : MonoBehaviour
{
    [SerializeField] private Transform transformToReach;
    [SerializeField] private Transform transformToMove;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transformToMove == null && transformToReach == null) return;
        transformToMove.position = transformToReach.position;
        transformToMove.rotation = transformToReach.rotation;
        transformToMove.localScale = transformToReach.localScale;
    }
}
