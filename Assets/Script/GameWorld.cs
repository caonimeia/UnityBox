using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Assertions;

public class GameWorld : MonoBehaviour {
    private void Awake() {

        //TODO 换个合适点全局初始化的地方
        Assert.raiseExceptions = true;

        {
            IPAddress ipAddr;
            if(IPAddress.TryParse("127.0.0.1", out ipAddr)) {
                SocketMgr.instance.CreateNewSocket(ServerType.gateway, new IPEndPoint(ipAddr, 8001), 1);
            }
        }

        {
            IPAddress ipAddr;
            if(IPAddress.TryParse("127.0.0.1", out ipAddr)) {
                SocketMgr.instance.CreateNewSocket(ServerType.logicServer, new IPEndPoint(ipAddr, 5622), 1);
            }
        }

    }

    private void Start () {
		
	}
	
	void Update () {
        SocketMgr.instance.Update();
	}

    private void OnDestroy() {
        SocketMgr.instance.OnDestroy();
    }
}
