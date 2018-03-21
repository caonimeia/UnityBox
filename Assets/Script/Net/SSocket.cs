using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum ServerType {
    gateway,
    logicServer,
}

public enum SocketState {
    idle,
    connectFaild,
    connecting,
    connected,
    reConnecting,
}

public enum SocketErrorCode {
    WSAETIMEDOUT = 10060, // 超时异常
}


public class XYSocketClient {
    //private const int tinyPackSize = 0x00007FFF;
    //private readonly int packSendMaxSize = 4 * 1024;
    private readonly int _sendTimeout = 4000;           // 发送数据超时时间
    private readonly int _recvTimeout = 4000;           // 接受数据超时时间
    private readonly int _sendBufferSize = 8 * 1024;    // 发送数据缓冲区大小 
    private readonly int _recvBufferSize = 8 * 1024;    // 接受数据缓冲区大小
    private byte[] _recvBuffer;                         // 缓存从Tcp接受的数据 用于分包
    private Queue<byte[]> _sendDataQueue;               // 发送数据队列

    // todo 与socket没什么关系，应该移动到协议相关的定义文件去
    private readonly int _headerSize = 10; // 协议头大小


    private Socket _socket;
    private IPEndPoint _lep;
    private ServerType _type;
    private SocketState _curState;

    private int _timeout;
    private float _lastConnectTime = 0f;
    private int _curRetryCount = 0;
    private int _maxConnectCount = 5;

    public XYSocketClient(ServerType type, IPEndPoint lep, int timeout) {
        _type = type;
        _lep = lep;
        _timeout = timeout;
        _recvBuffer = new byte[_recvBufferSize];
        _sendDataQueue = new Queue<byte[]>();
        _curState = SocketState.idle;
    }

    private void CloseSocket() {
        if(_socket != null) {
            try {
                if(_socket.Connected) {
                    _socket.Shutdown(SocketShutdown.Both);
                }

                _socket.Close();
            } catch(SocketException e) {
                SDebug.Log(e.ToString());
                SDebug.Log("ErrorCode: ", e.ErrorCode);
            } catch(Exception e) {
                SDebug.Log(e.ToString());
            }

            _socket = null;
        }
    }

    private void Setup() {
        CloseSocket();
        CreateSocket();
        Connect();
    }

    public void Close() {
        CloseSocket();
    }

    private void CreateSocket() {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.SendTimeout = _sendTimeout;
        _socket.ReceiveTimeout = _recvTimeout;
        _socket.SendBufferSize = _sendBufferSize;
        _socket.ReceiveBufferSize = _recvBufferSize;
    }

    private void Connect() {
        /* 不使用同步Connect的原因
         * 同步的Connect会Block当前线程
         * 可能会出现在尝试Connect的过程中（无网络），长时间Block当前线程
         * 且TCP类型的Connect无法设置为非Block
         */
        try {
            _socket.BeginConnect(_lep, OnConnectCallBack, _socket);
            _curState = SocketState.connecting;
            _lastConnectTime = Time.realtimeSinceStartup;
        } catch(SocketException e) {
            SDebug.Log(e.ToString());
            SDebug.Log("ErrorCode: ", e.ErrorCode);
            _curState = SocketState.connectFaild;
        } catch(Exception e) {
            SDebug.Log(e.ToString());
            _curState = SocketState.connectFaild;
        }
    }

    private void OnConnectCallBack(IAsyncResult ar) {
        SDebug.Log(string.Format("Connect {0}({1}) {2}", _type, _lep, _socket.Connected ? "Success" : "Faild"));
        if(!_socket.Connected) {
            _curState = SocketState.connectFaild;
            return;
        }

        _socket.EndConnect(ar);
        _curState = SocketState.connected;
    }

    private void TryConnect() {
        if(NeedReConnect()) {
            Connect();
            _lastConnectTime = Time.realtimeSinceStartup;
            _curRetryCount++;
            _curState = SocketState.connecting;
        }
    }

    private bool NeedReConnect() {
        if(Time.realtimeSinceStartup - _lastConnectTime > _timeout && _curRetryCount < _maxConnectCount) {
            return true;
        }

        return false;
    }

    private bool NowConnected() {
        if(_socket != null && _socket.Connected)
            return true;

        return false;
    }

    public void PushSendRequest(byte[] data) {
        _sendDataQueue.Enqueue(data);
    }

    private void Send() {
        if(NowConnected()) {
            try {
                while(_sendDataQueue.Count > 0) {
                    // 发送数据大小检测
                    byte[] data = _sendDataQueue.Dequeue();
                    int ret = _socket.Send(data);
                }
            } catch(SocketException e) {
                SDebug.LogError(e.ToString());
                SDebug.LogError("ErrorCode: ", e.ErrorCode);
            } catch(ObjectDisposedException e) {
                SDebug.LogError(e.ToString());
            }
        }
    }

    private void Recv() {
        if(NowConnected() && _socket.Available > 0) {
            try {
                int recvSize = Math.Min(_socket.Available, _recvBufferSize - _recvBuffer.Length);
                int ret = _socket.Receive(_recvBuffer, _recvBuffer.Length, recvSize, SocketFlags.None);
                /* 检测数据接收缓冲区的大小是否大于数据头的大小
                 * 解析数据头，获取后续数据包的大小或者直接返回
                 * 判断数据接收缓冲区剩下的大小（减去数据头的大小）是否大于后续数据包的大小
                 * 解析数据包或者直接返回
                 */
               
                if(_recvBuffer.Length < _headerSize) {
                    return;
                }

                

            } catch(SocketException e) {
                SDebug.LogError(e.ToString());
                SDebug.LogError("ErrorCode: ", e.ErrorCode);
            } catch(ObjectDisposedException e) {
                SDebug.LogError(e.ToString());
            }
        }
    }

    public void Update() {
        switch(_curState) {
            case SocketState.idle:
                Setup();
                break;
            case SocketState.connectFaild:
                TryConnect();
                break;
            case SocketState.connecting:
                break;
            case SocketState.connected:
                if(!NowConnected()) {
                    TryConnect();
                    return;
                }

                Send();
                Recv();

                break;
            default:
                SDebug.LogError("Socket State Error");
                break;
        }
    }
}