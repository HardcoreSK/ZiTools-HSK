using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace ZiTools
{
    [HarmonyPatch(typeof(Game), nameof(Game.CurrentMap), MethodType.Setter)]
    public static class Patch_CurrentMap
    {
        public static void Postfix()
        {
            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (Find.WindowStack.TryGetWindow<ObjectSeeker_Window>(out var w))
            {
                if (w.IsOpen)
                    w.ODB.Update();
            }
        }
    }

    [HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
    public static class Patch_DoPlaySettingsGlobalControls
    {
        static string tooltip = "ZiT_ObjectsSeekerLabel".Translate();

        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
                return;

            bool isSelected = Find.WindowStack.IsOpen<ObjectSeeker_Window>();
            row.ToggleableIcon(ref isSelected, Textures.Search, tooltip, SoundDefOf.Mouseover_ButtonToggle);
            bool isSelected2 = Find.WindowStack.IsOpen<ObjectSeeker_Window>();
            if (isSelected != isSelected2)
            {
                if (!isSelected2)
                    ObjectSeeker_Window.DrawWindow();
                else
                    Find.WindowStack.TryRemove(typeof(ObjectSeeker_Window), false);
            }
        }
    }

    [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld), MethodType.Normal)]
    public static class Patch_ClearAllMapsAndWorld
    {
        public static void Postfix()
        {
            ObjectsDatabase.ClearUpdateAction();
        }
    }
}
