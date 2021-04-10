using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInputManager))]
public class InGameManager : MonoBehaviour
{
    enum GameState
    {
        Intro,
        Game,
        GameLoss,
        GameWin
    }
    GameState _currentState;

    

    [Header("Spawn Players")]
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private Transform[] spawns = new Transform[4];
    [SerializeField] Cinemachine.CinemachineTargetGroup playerTargetGroup;

    [Header("Overlays")]
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject endscreenOverlay;

    [Header("Player ID")]
    public Color[] IdToColorMap = new Color[4];

    // PAUSE:
    bool _gamePaused = false;
    public bool GamePaused { get => _gamePaused; }

    private void Start()
    {
        SpawnPlayers();
        SetupOverlays();
    }
    private void SpawnPlayers()
    {
        if (!playerPrefab) return;

        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        inputManager.playerPrefab = playerPrefab;

        _currentState = GameState.Intro;

        for (int i = 0; i < PlayerSelectManager.playersJoiningGame.Length; ++i)
        {
            bool playerJoin = PlayerSelectManager.playersJoiningGame[i];
            if (playerJoin)
            {
                PlayerInput newPlayer = inputManager.JoinPlayer(i);
                newPlayer.GetComponent<PlayerBehaviour>().ID = i;
                newPlayer.transform.position = spawns[i].position;
                if (playerTargetGroup) playerTargetGroup.AddMember(newPlayer.transform, 1.0f, 1.0f);

                List<AudioClip> clips = new List<AudioClip>();
                clips.Add(Resources.Load($"Audio/{i}Bark0") as AudioClip);
                clips.Add(Resources.Load($"Audio/{i}Bark1") as AudioClip);
                newPlayer.GetComponent<PlayerBehaviour>().SetAudioClips(clips);
            }
        }
    }
    private void SetupOverlays()
    {
        if (pauseOverlay) pauseOverlay.SetActive(false);
        if (endscreenOverlay) endscreenOverlay.SetActive(false);
    }


    public void PauseGame(bool pause)
    {
        if (pauseOverlay) pauseOverlay.SetActive(pause);
        Time.timeScale = pause ? 0.05f : 1.0f;
        _gamePaused = pause;
    }
}
