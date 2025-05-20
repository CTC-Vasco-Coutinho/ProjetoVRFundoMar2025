using System.Collections.Generic;
using UnityEngine;

public class Walk_Away : MonoBehaviour
{
    public List<GameObject> locais;
    public int indice;
    public float distance;
    public bool move;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indice = 0;
        move = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, locais[indice].transform.position, 0.01f);
            distance = Vector3.Distance(transform.position, locais[indice].transform.position);
            if (distance < 2)
            {
                indice++;
                if (indice == 4)
                {
                    move = !move;
                }
            }
        }
    }
}