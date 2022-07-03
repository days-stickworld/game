using System;
using System.Net.Http;
using kcp2k;
using Mirror;
using Newtonsoft.Json.Linq;
using Telepathy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerController : MonoBehaviour
{
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private string orchestrationEndpoint;
    
    void Start()
    {
        DoRequestServers();
    }

    void GenerateItem(string serverName, int onlinePlayers, int maxPlayers, string address, int port)
    {
        var item = Instantiate(itemPrefab, contentContainer, false);
        var btn = item.transform.Find("Button");
        btn.transform.Find("ServerName").gameObject.GetComponent<TMP_Text>().SetText(serverName);
        btn.transform.Find("PlayerCount").gameObject.GetComponent<TMP_Text>().text =
            onlinePlayers + " / " + maxPlayers + " PLAYERS ONLINE";
        
        btn.gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Game");
            NetworkManager.singleton.StartClient(new UriBuilder("kcp://" + address + ":" + port).Uri);
        });
    }

    public void DoUpdateList()
    {
        foreach (Transform child in contentContainer.transform) {
            Destroy(child.gameObject);
        }
        
        DoRequestServers();
    }

    async void DoRequestServers()
    {
        var client = new HttpClient();
        var response = await client.GetAsync(orchestrationEndpoint);
        var body = await response.Content.ReadAsStringAsync();
        var json = JArray.Parse(body);

        foreach (var item in json)
        {
            GenerateItem(item.Value<string>("identifier"), item.Value<int>("onlinePlayers"), item.Value<int>("maxPlayers"), item.Value<string>("host"), item.Value<int>("port"));
        }
    }
}