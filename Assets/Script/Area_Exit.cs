using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Area_Exit : MonoBehaviour
{
    public string areaTranditonName;
    public int exitID = 0;

    public AreaEntrance theEndtrance;

    private void Start()
    {
        theEndtrance.TransitionName = areaTranditonName;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            SceneManager.LoadScene(exitID);
         //   Player.instance.areaTransitonName = areaTranditonName;
        }
    }
}
