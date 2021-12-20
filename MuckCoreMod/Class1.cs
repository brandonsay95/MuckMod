using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MuckCoreMod
{
    public class ModInstance
    {
        public static string Version = "0.01-B-DMRC";
        public static MenuUI MenuUI { get; private set; }

        public void OnModLoad(MenuUI menuUI, Transform menuTransform)
        {
            MenuUI = menuUI;
            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "muckCoreMod.txt"));

            LogToConsole("Loaded Mod Successfully");
            var mainMod = new GameObject("MainMod");
            mainMod.AddComponent<MainModBehavior>();
        }
        public static void LogToConsole(string message)
        {
            System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "muckCoreMod.txt"), message + Environment.NewLine);
        }
    }
   
    public class MainModBehavior : MonoBehaviour
    {
        string printHirarchy(Transform t , string spacer)
        {
            string data = "";
            foreach(Transform item in t)
            {
                if(item.childCount != 0)
                {
                    data += item.name + Environment.NewLine;
                    data += printHirarchy(item, spacer + "|    ") + Environment.NewLine;
                }
                else
                {
                    data += item.name + Environment.NewLine;
                }
            }
            return data;
        }
        public bool cheatMode = false;
        public string cheatText = "";
        public string versionText = "";
        public bool showVersion = true;
        public bool showOutput = false;
        public string outputText = "";
        public string GetChildren(Transform t , string spacer)
        {
            string retData = "";
            foreach(Transform child in t)
            {
                retData += spacer + child.name+Environment.NewLine;
                if (child.childCount != 0)
                    retData +=  GetChildren(child,spacer + "\t") +Environment.NewLine;
            }
            return retData;
        }
        void OnGUI()
        {
            if (cheatMode)
            {
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                {
                    this.cheatMode = false;
                    var command = this.cheatText;
                    this.cheatText = "";
                    if (command == "ListGameObjects")
                    {
                        outputText = "Game Object List with no parents" + Environment.NewLine;
                        foreach (var item in Transform.FindObjectsOfType<GameObject>().Where(o => o.transform.parent == null))
                        {
                            outputText += "\t" + item.name + Environment.NewLine;
                        }
                        showOutput = true;
                    }
                    if(command == "ReviveAll")
                    {
                        for(int i = 0; i < Server.clients.Count; i++)
                        {
                            if (GameManager.players[i].dead)
                                ClientSend.RevivePlayer(i, -1, true);
                        }
                    }
                    if (command == "ListGameObjectsAll")
                    {
                        outputText = "Game Object List with no parents" + Environment.NewLine;
                        foreach (var item in Transform.FindObjectsOfType<GameObject>().Where(o => o.transform.parent == null))
                        {
                            outputText += "\t" + item.name + Environment.NewLine;
                            if (item.transform.childCount != 0) 
                            outputText += GetChildren(item.transform, "\t") + Environment.NewLine;

                        }
                        showOutput = true;
                    }
                }
                GUI.SetNextControlName("CheatMenu");
                cheatText = GUI.TextField(new Rect(0, 0, Screen.width, 40), cheatText);
                GUI.Label(new Rect(0, 40, Screen.width, 40),
                    Event.current.type.ToString() + ":" +  Event.current.keyCode.ToString()

                    );
                GUI.FocusControl("CheatMenu");


            }
            else if (showOutput)
            {
                GUI.depth = 100;
                GUI.Label(new Rect(10, 10, Screen.width, Screen.height), outputText);
                GUI.depth = 0;
            }
            else
            {
                 GUI.Label(new Rect(10, 10, Screen.width, 20),versionText );
            }

        }
        void Update()
        {
            if(showOutput && Input.GetKeyDown(KeyCode.Escape))
            {
                showOutput = false;
                outputText = "";
            }
            
            if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.BackQuote))
            {
                cheatMode = !cheatMode;
            }
        }
        private void Awake()
        {
            ModGameManager.OnNewDay += ModGameManager_OnNewDay;
            FurnaceSync.ProcessingMultiplier = () => 3;
          //  ShrineRespawn.GetNameFunc = () => "Warp To Homie";
            RespawnTotemUI.GetRevivePriceFunc = () => 10;
            //GameLoop.OnGameLoopStart = () => {
                
            //};
            HitableResource.HitResourceMultiplier  = () => 4;
            //RespawnTotemUI.RequestReviveFunc = (int playerId) => {
            //    Debug.LogError("trying");
            //    if (InventoryUI.Instance.GetMoney() < 10)
            //    {
            //        return true;
            //    }
            //    if (RespawnTotemUI.RequestReviveFunc != null && RespawnTotemUI.RequestReviveFunc(playerId))
            //    {
            //        return true;
            //    }
            //    PlayerManager playerManager = GameManager.players[playerId];
            //    if (playerManager == null || playerManager.disconnected || !playerManager.dead)
            //    {
            //        return true;
            //    }
            //    Debug.LogError("sendinging revie");
            //    GameManager.players[playerManager.id].transform.position = GameManager.players[playerId].transform.position;
            //    ClientSend.PlayerPosition(GameManager.players[playerId].transform.position + (Vector3.up* 4));
            //    return true;
            //};
            //GameLoop loop = Transform.FindObjectOfType<GameLoop>();
            //ModGameLoop modGameLoop = ModGameLoop.AttachToGameLoop(loop);
            //GameRegistry.SetType<GameLoop>(modGameLoop);
            //var modLoop =  loop.transform.gameObject.AddComponent<ModGameLoop>();
            //GameLoop.Instance = modLoop;
            //modLoop.bosses = loop.bosses;
            //modLoop.currentDay = loop.currentDay;
            //modLoop.name = loop.name;

            //ModInstance.LogToConsole("Behavior Attached");

            DontDestroyOnLoad(this);
        }

        private void ModGameManager_OnNewDay(object sender, EventArgs e)
        {

            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (GameManager.players[i].dead)
                    ClientSend.RevivePlayer(i, -1, true);
            }
        }
    }
 
}
