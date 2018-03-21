using System.Collections.Generic;
using System.Net;
using UnityEngine;


public class SocketMgr : Singleton<SocketMgr> {
    private Dictionary<ServerType, XYSocketClient> socketNodeDic;

    private SocketMgr() {
        socketNodeDic = new Dictionary<ServerType, XYSocketClient>();
    }

    public bool Setup() {
        Debug.Log("SocketMgr Init Suceess...");
        return true;
    }

    public void CreateNewSocket(ServerType type, IPEndPoint lep, int timeout) {
        XYSocketClient sc = new XYSocketClient(type, lep, timeout);
        Add(type, sc);
    }

    // todo 检测 
    // 可能要改成List，可连接多个同一类型的服务器
    private void Add(ServerType type, XYSocketClient socket) {
        socketNodeDic.Add(type, socket);
    }

    // 发送数据接口
    public void SendByServerType(ServerType type, byte[] data) {
        XYSocketClient sc;
        if(!socketNodeDic.TryGetValue(type, out sc)) {
            SDebug.LogError("Server Type Not Found");
            return;
        }

        sc.PushSendRequest(data);
    }

    public void Update() {
        foreach(var item in socketNodeDic) {
            item.Value.Update();
        }
    }

    public void OnDestroy() {
        foreach(var item in socketNodeDic) {
            item.Value.Close();
        }
    }
}
