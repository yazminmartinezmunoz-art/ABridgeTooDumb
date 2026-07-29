using UnityEngine;
using TMPro;
using Photon.Pun;

public class Chronometer : MonoBehaviourPun
{
    [SerializeField] private float remainingTime;
    [SerializeField] private TextMeshProUGUI timerText;
    private bool hasEnded = false;

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!hasEnded)
            {
                remainingTime -= Time.deltaTime;

                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    hasEnded = true;

                    Debug.Log("Se acabo el tiempo");
                    GameManager.instance.Lose();
                }

                // Enviamos el tiempo actualizado a todos los clientes
                photonView.RPC("RPC_UpdateTimeUI", RpcTarget.All, remainingTime);
            }
        }
    }

    [PunRPC]
    private void RPC_UpdateTimeUI(float time)
    {
        UpdateTimeUI(time);
    }

    private void UpdateTimeUI(float time)
    {
        if (time <= 0)
        {
            time = 0;
        }

        //timerText.text = time.ToString("f2");

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }
}
