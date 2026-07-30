using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;

public class MeteorSpawner : MonoBehaviourPunCallbacks
{
    [Header("Prefab")]
    public string nombrePrefabMeteorito = "Meteoro";

    [Header("Evento")]
    public int probabilidadBase = 10;
    public int probabilidadMax = 20;

    public int cantidadMeteoritos = 10;
    public float tiempoEntreMeteoritos = 0.5f;

    [Header("Timer")]
    public float tiempoPorMinuto = 60f;

    [Header("Spawn")]
    public float alturaSpawn = 30f;
    public float radioTormenta = 10f;
    public float areaMapaX = 50f;
    public float areaMapaZ = 50f;

    [Header("UI")]
    public GameObject panelEvento;
    public TMP_Text textoEvento;

    [Header("FX Audio")]
    public AudioSource audioAlarma;

    private int minutosPasados = 0;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        StartCoroutine(TimerGlobal());
    }

    IEnumerator TimerGlobal()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoPorMinuto);

            minutosPasados++;

            int probabilidadActual = Mathf.Min(
                probabilidadBase + (minutosPasados * 2),
                probabilidadMax
            );

            int numero = Random.Range(0, 100);

            Debug.Log($"Minuto {minutosPasados} | Prob: {probabilidadActual}%");

            if (numero < probabilidadActual)
            {
                photonView.RPC("RPC_IniciarEvento", RpcTarget.All);
                StartCoroutine(EventoMeteoritos());
            }
        }
    }

    IEnumerator EventoMeteoritos()
    {
        Vector3 centroEvento = new Vector3(
            Random.Range(-areaMapaX, areaMapaX),
            0f,
            Random.Range(-areaMapaZ, areaMapaZ)
        );

        for (int i = 0; i < cantidadMeteoritos; i++)
        {
            Vector3 pos = centroEvento + new Vector3(
                Random.Range(-radioTormenta, radioTormenta),
                alturaSpawn,
                Random.Range(-radioTormenta, radioTormenta)
            );

            PhotonNetwork.Instantiate(
                nombrePrefabMeteorito,
                pos,
                Random.rotation
            );

            yield return new WaitForSeconds(tiempoEntreMeteoritos);
        }

        photonView.RPC("RPC_FinEvento", RpcTarget.All);
    }

    // =========================
    // INICIO EVENTO
    // =========================
    [PunRPC]
    void RPC_IniciarEvento()
    {
        if (panelEvento != null)
            panelEvento.SetActive(true);

        if (textoEvento != null)
            textoEvento.text = "☄️ LLUVIA DE METEORITOS";

        if (audioAlarma != null)
            audioAlarma.Play();

        CamaraShake cam = FindFirstObjectByType<CamaraShake>();
        if (cam != null)
            StartCoroutine(cam.Shake(0.5f, 0.3f));
    }

    // =========================
    // FIN EVENTO
    // =========================
    [PunRPC]
    void RPC_FinEvento()
    {
        if (panelEvento != null)
            panelEvento.SetActive(false);

        if (audioAlarma != null)
            audioAlarma.Stop();
    }
}