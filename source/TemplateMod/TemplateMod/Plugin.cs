﻿using BepInEx;
using HarmonyLib;
using TemplateMod.Assets;
using UnityEngine;

namespace TemplateMod;

[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    private const string Guid = "yourname.modname";
    private const string Name = "Template";
    private const string Version = "1.0.0";

    private void Awake()
    {
        new Harmony(Guid).PatchAll();
        AssetManager.LoadCatalog();
        Debug.Log($"{Name} v{Version} has been loaded.");
    }
}