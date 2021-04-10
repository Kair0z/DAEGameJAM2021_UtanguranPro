using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerSelectManager : MonoBehaviour
{
    public static List<PlayerInput> playersJoiningGame = new List<PlayerInput>();

    [SerializeField] private Transform[] pawnSpawns = new Transform[4];
    private List<GameObject> pawnsSpawned = new List<GameObject>();

    [SerializeField] private Vector2Int minMaxPlayers = new Vector2Int(2, 4);

    [Header("Countdown")]
    [SerializeField] private float countdownTime = 5.0f;
    [SerializeField] private TextMeshProUGUI countdownTextMesh;
    private Timer _countDown = new Timer();

    public UnityEvent OnGameStart;

    private void Awake()
    {
        _countDown.Set(countdownTime);
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
        playersJoiningGame.Add(lockedPlayer);
        _countDown.Set(countdownTime); // Reset timer
    }
    public void SignalPlayerUnlock(PlayerInput freedPlayer)
    {
        playersJoiningGame.Remove(freedPlayer);
    }
    static public void FlushJoiningPlayers()
    {
        playersJoiningGame.Clear();
    }

    private void Update()
    {
        if (playersJoiningGame.Count >= minMaxPlayers.x)
        {
            _countDown.OnPing(Time.deltaTime, StartGame, false);
        }

        if (countdownTextMesh)
            countdownTextMesh.text = Mathf.Clamp(_countDown.TimeLeft, 0.0f, countdownTime).ToString();
    }
    private void StartGame()
    {
        Debug.Log("GAME START");
        OnGameStart.Invoke();
    }
}
