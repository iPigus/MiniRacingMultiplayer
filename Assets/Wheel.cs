using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool isOnTrack { get; private set; } = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Road")) isOnTrack = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Road")) isOnTrack = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Road")) isOnTrack = true;
    }
}
