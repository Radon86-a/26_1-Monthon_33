using System;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private WebSocket websocket;

    public string RoomId { get; private set; }
    public bool IsFirstPlayer { get; private set; }
    public bool IsConnected => websocket != null && websocket.State == WebSocketState.Open;

    // イベント通知（UI側で購読可能にする）
    public event Action OnConnected;
    public event Action<string> OnWaiting;
    public event Action<NetworkPayload> OnMatchFound;
    public PlayerData playerData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // サーバーへの接続を行うメソッド
    public async void ConnectToServer(string serverUrl = "ws://localhost:8080/ws")
    {
        if (websocket != null && websocket.State == WebSocketState.Open) return;

        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("[Network] Socket Opened!");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("[Network] Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("[Network] Closed: " + e);
        };

        websocket.OnMessage += (bytes) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("[Network] Received: " + json);
            
            // サーバーから "connected" メッセージが来たらイベント発火
            if (json.Contains("\"connected\""))
            {
                OnConnected?.Invoke();
            }
        };

        // 接続開始（awaitは内部でのみ実行）
        await websocket.Connect();
    }

    // マッチング待機列に参加するメソッド
    public async void StartMatching()
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[Network] サーバーに未接続です。先に ConnectToServer を呼び出してください。");
            return;
        }

        // サーバーへマッチング要求を送信
        string json = "{\"type\":\"join_match\"}";
        await websocket.SendText(json);
        Debug.Log("[Network] Sent: join_match request");
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
        #endif
    }

    private void HandleServerMessage(string json)
    {
        NetworkPayload data = JsonUtility.FromJson<NetworkPayload>(json);

        switch (data.type)
        {
            case "connected":
                playerData.player_id = data.player_id;
                Debug.Log($"<color=cyan>[Connected]</color> Player ID: {playerData.player_id}");
                OnConnected?.Invoke();
                break;

            case "waiting":
                Debug.Log($"<color=yellow>[Status]</color> {data.message}");
                OnWaiting?.Invoke(data.message);
                break;

            case "match_found":
                RoomId = data.room_id;
                IsFirstPlayer = data.is_first;
                Debug.Log($"<color=green>[Match Found]</color> Room: {RoomId} | First: {IsFirstPlayer}");
                OnMatchFound?.Invoke(data);
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

[Serializable]
public class NetworkPayload
{
    public string type;
    public string room_id;
    public string player_id;
    public bool is_first;
    public string opponent_id;
    public string message;
}