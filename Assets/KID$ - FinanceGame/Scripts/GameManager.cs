﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private PlayerData player;

    //Awake is always called before any Start functions
    void Awake()
    {

        //Check if instance already exists
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);          //Sets this to not be destroyed when reloading scene
            instance = this;                        //if not, set instance to this
            InitGame();
        }

        //If instance already exists and it's not this:
        else if (instance != this)
        {
            Destroy(gameObject);                    //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
        }

    }

    private void Update()
    {
        evaluateEnergy();
    }

    private void InitGame()
    {
        loadGame();
    }

    private void loadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/KID$_playerinfo.dat"))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/KID$_playerinfo.dat", FileMode.Open);
                player = (PlayerData)bf.Deserialize(file);
                print(Application.persistentDataPath + "/KID$_playerinfo.dat: LOADED");
                print(player.toString());
            }
            catch (Exception ex)
            {
                createPlayer();
            }

        }
        else
        {
            print(Application.persistentDataPath + "/KID$_playerinfo.dat: FILE NOT FOUND");
            createPlayer();
        }

    }

    public void saveGame()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/KID$_playerinfo.dat");

            bf.Serialize(file, player);
            file.Close();

            print("Save success");
        }
        catch (Exception ex)
        {
            print("Save error: " + ex.ToString());
        }
        
    }

    private void evaluateEnergy()
    {

        TimeSpan timeBetween = DateTime.Now - player.getLastEnergyGain();

        for (int a = 0; a < timeBetween.Minutes; a++)       //Update every minute
        {
            player.addPlayerEnergy();
            player.setLastEnergyGain(DateTime.Now);
        }

    }

    private void createPlayer()
    {
        print("Creating player data");
        player = new PlayerData();
        saveGame();
        //TODO: Show tutorial?
    }

    public PlayerData getPlayerData()
    {
        return player;
    }

    public void addBankEntry(BankEntry entry)
    {
        if (player != null)
            player.addBankEntry(entry);
        else
            print("Player is null");
    }



}

