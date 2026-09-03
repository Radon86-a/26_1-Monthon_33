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
    public event Action<GameData> OnMatchFound;
    // キャンセル完了イベント
    public event Action OnMatchCancelled;
    public PlayerData playerData;
    public GameData gameData;

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
            
            HandleServerMessage(json);
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
        GameData data = JsonUtility.FromJson<GameData>(json);

        switch (data.type)
    {
        case "connected":
            playerData.player_id = data.my_data.player_id;
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

        case "game_state":
            GameData syncData = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"<color=orange>[Battle State]</color> Action: {syncData.action} | Turn: {syncData.current_turn_player_id}");
            OnBattleStateReceived?.Invoke(syncData);
            break;

        case "opponent_disconnected":
            Debug.Log($"<color=red>[Opponent Disconnected]</color>");
            break;

        case "match_cancelled":
                Debug.Log("<color=yellow>[Status]</color> Matchmaking cancelled.");
                OnMatchCancelled?.Invoke();
                break;

        default:
            Debug.LogWarning($"[Network] 未知のメッセージタイプを受信しました: {data.type}");
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

    // 戦闘データ受信用イベント
    public event Action<GameData> OnBattleStateReceived;

    // 戦闘データをサーバーへ送信するメソッド
    public async void SendBattleState(GameData data)
    {
        if (websocket != null && websocket.State == NativeWebSocket.WebSocketState.Open)
        {
            data.room_id = RoomId;
            data.my_data.player_id = playerData.player_id;
            string json = JsonUtility.ToJson(data);
            await websocket.SendText(json);
        }
    }

    // キャンセル要求を送信
    public async void CancelMatching()
    {
        if (!IsConnected) return;

        string json = "{\"type\":\"cancel_match\"}";
        await websocket.SendText(json);
        Debug.Log("[Network] Sent: cancel_match");
    }
}