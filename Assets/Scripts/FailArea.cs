using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailArea : MonoBehaviour
{
    [NonSerialized]
    public int ballNum = 0;
    public List<GameObject> collisions = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (collision.GetComponent<Ball>().hasSpawNew)
            {
                if (!collisions.Contains(collision.gameObject))
                {
                    ballNum++;
                    collisions.Add(collision.gameObject);
                    MainController.Instance.CheckFail();
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (collision.GetComponent<Ball>().hasSpawNew)
            {
                if (collisions.Contains(collision.gameObject))
                {
                    ballNum--;
                    collisions.Remove(collision.gameObject);
                    MainController.Instance.CheckFail();
                }
            }
        }
    }
}
