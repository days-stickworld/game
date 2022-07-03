using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StartServerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.singleton.StartServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
