using System.Collections.Generic;
using System.Reflection.Emit;
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

    [HarmonyPatch(typeof(PlaySettings), "DoMapControls")]
    [HarmonyDebug]
    public static class Patch_DoMapControls
    {
        // Remove old search icon from map controls
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var cm = new CodeMatcher(instructions);

            // Find a skip label where MapSearch button or OpenMapSearch key pressed if statement ends
            cm.End();
            cm.MatchEndBackwards(
                new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(KeyBindingDefOf), nameof(KeyBindingDefOf.OpenMapSearch))),
                new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(KeyBindingDef), nameof(KeyBindingDef.JustPressed))),
                new CodeMatch(OpCodes.Brfalse_S),
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Event), nameof(Event.current))),
                new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Event), nameof(Event.type))),
                new CodeMatch(OpCodes.Ldc_I4_4),
                new CodeMatch(OpCodes.Bne_Un_S) // we want this
            );
            if (!cm.IsValid)
            {
                Log.Error("Object seeker: failed to patch PlaySettings.DoMapControls (1)");
                return instructions;
            }

            var skipLabel = (Label) cm.Operand;

            // Find start of if statement
            cm.MatchStartBackwards(
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(KeyPrefs), nameof(KeyPrefs.KeyPrefsData))),
                new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(KeyBindingDefOf), nameof(KeyBindingDefOf.OpenMapSearch))),
                new CodeMatch(OpCodes.Ldc_I4_0),
                CodeMatch.Calls(() => default(KeyPrefsData).GetBoundKeyCode(default, default)),
                CodeMatch.Calls(() => GenText.ToStringReadable(default))
            );
            if (!cm.IsValid)
            {
                Log.Error("Object seeker: failed to patch PlaySettings.DoMapControls (2)");
                return instructions;
            }

            var labels = cm.Labels.ListFullCopy();
            cm.Labels.Clear();

            // Insert check call
            cm.Insert(
                CodeInstruction.Call(() => DisplayOldSearchIcon()),
                new CodeInstruction(OpCodes.Brfalse, skipLabel)
            );

            cm.Labels = labels;

            return cm.InstructionEnumeration();
        }

        public static bool DisplayOldSearchIcon()
        {
            return !ZiToolsMod.Settings.hideOldMapSearchIcon;
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
