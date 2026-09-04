using System;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private WebSocket websocket;

    public string RoomId { get; private set; }
    public string OpponentId { get; private set; } // ★対戦相手IDを保持
    public bool IsFirstPlayer { get; private set; }
    public bool IsConnected => websocket != null && websocket.State == WebSocketState.Open;

    // イベント通知
    public event Action OnConnected;
    public event Action<string> OnWaiting;
    public event Action<GameData> OnMatchFound;
    public event Action OnMatchCancelled;
    public event Action<GameData> OnBattleStateReceived;

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

        await websocket.Connect();
    }

    public async void StartMatching()
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[Network] サーバーに未接続です。先に ConnectToServer を呼び出してください。");
            return;
        }

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
                // ★修正: サーバーからルート直下で届く player_id を代入
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
                OpponentId = data.opponent_id; // ★相手のIDを保持
                IsFirstPlayer = data.is_first;
                Debug.Log($"<color=green>[Match Found]</color> Room: {RoomId} | First: {IsFirstPlayer} | Opponent: {OpponentId}");
                OnMatchFound?.Invoke(data);
                break;

            case "game_state":
                Debug.Log($"<color=orange>[Battle State]</color> Action: {data.action} | Turn: {data.current_turn_player_id}");
                // ★既にパース済みの data をそのまま渡す
                OnBattleStateReceived?.Invoke(data);
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

    // 戦闘データをサーバーへ送信するメソッド
    public async void SendBattleState(GameData data)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            data.type = "game_state"; // ★typeの確実なセット
            data.room_id = RoomId;
            data.player_id = playerData.player_id; // ★ルートの送信元IDもセット

            // 内部の my_data にも自身のIDを注入
            var myStatus = data.my_data;
            myStatus.player_id = playerData.player_id;
            data.my_data = myStatus;

            string json = JsonUtility.ToJson(data);
            await websocket.SendText(json);
        }
        else
        {
            Debug.LogWarning("[Network] 送信失敗: WebSocketが切断されています。");
        }
    }

    public async void CancelMatching()
    {
        if (!IsConnected) return;

        string json = "{\"type\":\"cancel_match\"}";
        await websocket.SendText(json);
        Debug.Log("[Network] Sent: cancel_match");
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}