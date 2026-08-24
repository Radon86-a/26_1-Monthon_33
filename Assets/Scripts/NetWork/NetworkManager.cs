using UnityEngine;
using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    private WebSocket websocket;

    async void Start()
    {
        // GoサーバーのWebSocketエンドポイントへ接続
        websocket = new WebSocket("ws://localhost:8080/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("[Network] Connection open!");
            // 接続成功したらテストメッセージを送信
            SendTestMessage();
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("[Network] Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("[Network] Connection closed!");
        };

        // サーバーからデータを受信したときの処理
        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("[Network] Received from server: " + message);
        };

        // 接続処理を開始
        await websocket.Connect();
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            // 受信キューを毎フレーム処理する
            websocket?.DispatchMessageQueue();
        #endif
    }

    private async void SendTestMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("Hello from Unity!");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}
