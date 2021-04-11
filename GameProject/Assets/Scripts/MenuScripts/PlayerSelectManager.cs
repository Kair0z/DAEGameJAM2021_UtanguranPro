using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerSelectManager : MonoBehaviour
{
    public static bool[] playersJoiningGame = new bool[4];

    [SerializeField] private Transform[] pawnSpawns = new Transform[4];
    private List<GameObject> pawnsSpawned = new List<GameObject>();

    [SerializeField] private Vector2Int minMaxPlayers = new Vector2Int(2, 4);

    [Header("Countdown")]
    [SerializeField] private float countdownTime = 5.0f;
    [SerializeField] private TextMeshProUGUI countdownTextMesh;
    private Timer _countDown = new Timer();

    public UnityEvent OnGameStart;

    private void Start()
    {
        _countDown.Set(countdownTime);
        FlushJoiningPlayers();
    }
    private void OnPlayerJoined(PlayerInput newPlayer)
    {
        pawnsSpawned.Add(newPlayer.gameObject);
        pawnsSpawned[pawnsSpawned.Count - 1].transform.position = pawnSpawns[pawnsSpawned.Count - 1].position;
    }
    private void OnPlayerLeft(PlayerInput leftPlayer)
    {
        pawnsSpawned.Remove(leftPlayer.gameObject);
    }


    public void SignalPlayerLock(PlayerInput lockedPlayer)
    {
        if (lockedPlayer.playerIndex >= 4) return; // MAX 4 Players

        playersJoiningGame[lockedPlayer.playerIndex] = true;
        _countDown.Set(countdownTime); // Reset timer
    }

    static public void FlushJoiningPlayers()
    {
        playersJoiningGame = new bool[4];
    }

    private void Update()
    {
        int playersJoining = 0;
        for (int i = 0; i < playersJoiningGame.Length; ++i)
            if (playersJoiningGame[i]) ++playersJoining;

        if (playersJoining >= minMaxPlayers.x)
        {
            _countDown.OnPing(Time.deltaTime, StartGame, false);
        }

        if (countdownTextMesh)
            countdownTextMesh.text = Mathf.Clamp(_countDown.TimeLeft, 0.0f, countdownTime).ToString();
    }
    private void StartGame()
    {
        OnGameStart.Invoke();
    }
}
