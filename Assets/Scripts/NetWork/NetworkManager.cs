using System;
using UnityEngine;
using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    private WebSocket websocket;

    public string MyPlayerId { get; private set; }
    public string RoomId { get; private set; }
    public bool IsFirstPlayer { get; private set; }

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8080/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("[Network] Connected to matching server!");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("[Network] Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("[Network] Connection closed");
        };

        websocket.OnMessage += (bytes) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("[Network] Raw JSON: " + json);
            HandleServerMessage(json);
        };

        await websocket.Connect();
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
        #endif
    }

    private void HandleServerMessage(string json)
    {
        MatchData data = JsonUtility.FromJson<MatchData>(json);

        switch (data.type)
        {
            case "waiting":
                Debug.Log($"<color=yellow>[Status]</color> {data.message}");
                break;

            case "match_found":
                MyPlayerId = data.player_id;
                RoomId = data.room_id;
                IsFirstPlayer = data.is_first;

                string turnStr = IsFirstPlayer ? "先行 (First)" : "後攻 (Second)";
                Debug.Log($"<color=green>[Match Found!]</color> Room: {RoomId} | You: {MyPlayerId} | Opponent: {data.opponent_id} | Turn: {turnStr}");
                break;

            case "opponent_disconnected":
                Debug.LogWarning($"<color=red>[Warning]</color> {data.message}");
                break;
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

// サーバーから受信するメッセージ用クラス
[Serializable]
public class MatchData
{
    public string type;
    public string room_id;
    public string player_id;
    public bool is_first;
    public string opponent_id;
    public string message;
}