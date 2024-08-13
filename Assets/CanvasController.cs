using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
      transform.LookAt(player.position);
    }
}
