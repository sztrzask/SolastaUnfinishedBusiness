﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GuiPanelPatcher
{
    [HarmonyPatch(typeof(GuiPanel), "Show")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Show_Patch
    {
        internal static void Postfix(GuiPanel __instance)
        {
            //PATCH: Keeps last level up hero selected
            if (__instance is not MainMenuScreen mainMenuScreen || Global.LastLevelUpHeroName == null)
            {
                return;
            }

            mainMenuScreen.charactersPanel.Show();
        }
    }
}
