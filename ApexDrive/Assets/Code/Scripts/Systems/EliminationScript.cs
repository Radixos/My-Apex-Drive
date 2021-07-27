using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationScript : MonoBehaviour
{
    private float grace = 0.0f;
    public GameObject deathPlane;

    private Coroutine[] m_GraceRoutines = new Coroutine[4];
    private List<Player> m_ActivePlayers = new List<Player>();
    private List<Player> m_OffscreenPlayers = new List<Player>();

    private void OnEnable()
    {
        deathPlane = GameObject.FindGameObjectWithTag("Offroad");
        RaceManager.OnRoundStart += OnRoundStart;
        RaceManager.OnRoundEnd += OnRoundEnd;
    }

    private void OnDisable()
    {
        RaceManager.OnRoundStart -= OnRoundStart;
        RaceManager.OnRoundEnd -= OnRoundEnd;
    }

    private void OnRoundStart()
    {
        m_ActivePlayers.Clear();
        m_OffscreenPlayers.Clear();
        foreach(Player player in GameManager.Instance.ConnectedPlayers) m_ActivePlayers.Add(player);
        StartCoroutine(Co_CheckEliminations());
    }

    private void OnRoundEnd()
    {
        m_ActivePlayers.Clear();
        m_OffscreenPlayers.Clear();
    }

    private IEnumerator Co_CheckEliminations()
    {
        while(RaceManager.State == RaceManager.RaceState.Racing)
        {
            List<Player> noLongerVisiblePlayers = new List<Player>();

            foreach(Player player in m_ActivePlayers)
            {
                bool visiblePlayer = IsPointInsideCameraFrustum(player.Car.Position);
                if (!visiblePlayer && !m_OffscreenPlayers.Contains(player)) noLongerVisiblePlayers.Add(player);
                //else if (player.Car.GetComponent<SphereCollider>().bounds.Intersects(deathPlane.GetComponent<BoxCollider>().bounds)
                    //&& !m_OffscreenPlayers.Contains(player)) noLongerVisiblePlayers.Add(player); Debug.Log("Death");
                //else for (int i = 0; i < deathPlane.Length; i++) if (player.Car.GetComponent<SphereCollider>().bounds.Intersects
                //(deathPlane[i].GetComponent<BoxCollider>().bounds)
                //&& !m_OffscreenPlayers.Contains(player)) noLongerVisiblePlayers.Add(player); Debug.Log("Death");
            }
            foreach (Player player in noLongerVisiblePlayers) StartCoroutine(Co_Eliminate(player.Car));
            if(m_ActivePlayers.Count == 1) m_ActivePlayers[0].WinRound();
            yield return null;
        }
        
    }

    private bool IsPointInsideCameraFrustum(Vector3 point)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(point);
        if ((viewportPosition.x > 1.0f || viewportPosition.x < 0.0f) || (viewportPosition.y > 1.0f || viewportPosition.y < 0.0f)) return false;
        return true;
    }

    private IEnumerator Co_Eliminate(CoreCarModule car)
    {
        float elapsed = 0.0f;

        while(elapsed < grace)
        {
            if(IsPointInsideCameraFrustum(car.Position))
            {
                // save player
            }
            yield return null;
        }

        car.gameObject.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/TukTuk/Elimination");
        m_ActivePlayers.Remove(car.Player);
    }
}