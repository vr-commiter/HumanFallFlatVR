using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using System.Threading;
using UnityEngine;
using MyTrueGear;

namespace HumanFallFlatVR_TrueGear;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static TrueGearMod _TrueGear = null;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(typeof(Plugin));
        _TrueGear = new TrueGearMod();
        _TrueGear.Play("Fall");

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static bool isLeftHandGrab = false;
    private static bool isRightHandGrab = false;
    private static bool isJump = false;
    private static bool isLean = false;
    private static bool isUnconscious = false;
    private static bool isHumanOnGround = false;
    private static HumanState playerState = HumanState.Idle;

    //[HarmonyPostfix, HarmonyPatch(typeof(HumanVRController), "GetLeftGrab")]
    //private static void HumanVRController_GetLeftGrab_Postfix(HumanVRController __instance,bool __result)
    //{
    //    if (__instance.Human.IsLocalPlayer)
    //    {
    //        if (__result && !isLeftHandGrab)
    //        {
    //            Logger.LogInfo("--------------------------------------------");
    //            Logger.LogInfo("LeftHandGrab");
    //        }
    //        isLeftHandGrab = __result;
    //    }        
    //}

    //[HarmonyPostfix, HarmonyPatch(typeof(HumanVRController), "GetRightGrab")]
    //private static void HumanVRController_GetRightGrab_Postfix(HumanVRController __instance, bool __result)
    //{
    //    if (__instance.Human.IsLocalPlayer)
    //    {
    //        if (__result && !isRightHandGrab)
    //        {
    //            Logger.LogInfo("--------------------------------------------");
    //            Logger.LogInfo("RightHandGrab");
    //        }
    //        isRightHandGrab = __result;
    //    }
    //}

    //[HarmonyPostfix, HarmonyPatch(typeof(HandMuscles), "ProcessHand")]
    //private static void HandMuscles_ProcessHand_Postfix(HandMuscles __instance, bool grab, bool right)
    //{
    //    if (__instance.human.IsLocalPlayer)
    //    {
    //        if (right)
    //        {
    //            if (grab && !isRightHandGrab)
    //            {
    //                Logger.LogInfo("--------------------------------------------");
    //                Logger.LogInfo("RightHandGrab");
    //            }
    //            isRightHandGrab = grab;
    //        }
    //        else
    //        {
    //            if (grab && !isLeftHandGrab)
    //            {
    //                Logger.LogInfo("--------------------------------------------");
    //                Logger.LogInfo("leftHandGrab");
    //            }
    //            isLeftHandGrab = grab;
    //        }
    //    }
    //}


    [HarmonyPrefix, HarmonyPatch(typeof(HandMuscles), "PlaceHand")]
    private static void HandMuscles_PlaceHand_Prefix(HandMuscles __instance, HumanSegment hand, bool active, bool grabbed)
    {
        if (__instance.human.IsLocalPlayer)
        {
            if (!active)
            {
                return;
            }
            if (hand == __instance.ragdoll.partLeftHand)
            {
                if (grabbed && !isLeftHandGrab)
                {
                    Logger.LogInfo("--------------------------------------------");
                    Logger.LogInfo("LeftHandGrab");
                    _TrueGear.Play("LeftHandGrab");
                }
                isLeftHandGrab = grabbed;
            }
            if (hand == __instance.ragdoll.partRightHand)
            {
                if (grabbed && !isRightHandGrab)
                {
                    Logger.LogInfo("--------------------------------------------");
                    Logger.LogInfo("RightHandGrab");
                    _TrueGear.Play("RightHandGrab");
                }
                isRightHandGrab = grabbed;
            }
        }
    }

    //[HarmonyPostfix, HarmonyPatch(typeof(HumanVRController), "GetJump")]
    //private static void HumanVRController_GetJump_Postfix(HumanVRController __instance, bool __result)
    //{
    //    if (__instance.Human.IsLocalPlayer)
    //    {
    //        if (__result && !isJump && !isUnconscious)
    //        {
    //            Logger.LogInfo("--------------------------------------------");
    //            Logger.LogInfo("Jump");
    //            Logger.LogInfo(playerState);
    //        }
    //        isJump = __result;
    //    }
    //}

    [HarmonyPostfix, HarmonyPatch(typeof(HumanVRController), "GetLean")]
    private static void HumanVRController_GetLean_Postfix(HumanVRController __instance, bool __result)
    {
        if (__result && !isLean)
        {
            Logger.LogInfo("--------------------------------------------");
            Logger.LogInfo("Lean");
            _TrueGear.Play("Lean");
        }
        isLean = __result;
    }

    //[HarmonyPostfix, HarmonyPatch(typeof(HumanVRController), "GetLeanValue")]
    //private static void HumanVRController_GetLeanValue_Postfix(HumanVRController __instance, float __result)
    //{
    //    if (__result == 0f || __result == 1f)
    //    {
    //        return;
    //    }
    //    Logger.LogInfo("--------------------------------------------");
    //    Logger.LogInfo("GetLeanValue");
    //    Logger.LogInfo(__result);
    //}

    //[HarmonyPostfix, HarmonyPatch(typeof(TorsoMuscles), "IdleAnimation")]
    //private static void TorsoMuscles_IdleAnimation_Postfix(TorsoMuscles __instance)
    //{
    //    Logger.LogInfo("--------------------------------------------");
    //    Logger.LogInfo("IdleAnimation");
    //    Logger.LogInfo(__instance.human.IsLocalPlayer);
    //}

    //[HarmonyPostfix, HarmonyPatch(typeof(LegMuscles), "JumpAnimation")]
    //private static void LegMuscles_JumpAnimation_Postfix(LegMuscles __instance)
    //{
    //    Logger.LogInfo("--------------------------------------------");
    //    Logger.LogInfo("JumpAnimation");
    //    Logger.LogInfo(__instance.human.IsLocalPlayer);
    //}

    //[HarmonyPostfix, HarmonyPatch(typeof(TorsoMuscles), "FreeFallAnimation")]
    //private static void TorsoMuscles_FreeFallAnimation_Postfix(TorsoMuscles __instance)
    //{
    //    if (__instance.human.IsLocalPlayer)
    //    {
    //        Logger.LogInfo("--------------------------------------------");
    //        Logger.LogInfo("FreeFall");
    //    }
    //}

    [HarmonyPostfix, HarmonyPatch(typeof(TorsoMuscles), "FallAnimation")]
    private static void TorsoMuscles_FallAnimation_Postfix(TorsoMuscles __instance)
    {
        if (__instance.human.IsLocalPlayer)
        {
            Logger.LogInfo("--------------------------------------------");
            Logger.LogInfo("Fall");
            _TrueGear.Play("Fall");
        }
    }

    [HarmonyPostfix, HarmonyPatch(typeof(TorsoMuscles), "OnFixedUpdate")]
    private static void TorsoMuscles_OnFixedUpdate_Postfix(TorsoMuscles __instance)
    {
        if (__instance.human.IsLocalPlayer)
        {
            playerState = __instance.human.state;
            //Logger.LogInfo("--------------------------------------------");
            //Logger.LogInfo(playerState);
            if (__instance.human.state == HumanState.Unconscious)
            {
                if (!isUnconscious)
                {
                    isUnconscious = true;
                    Logger.LogInfo("--------------------------------------------");
                    Logger.LogInfo("Unconscious");
                    _TrueGear.Play("Unconscious");
                }
            }
            else
            {
                isUnconscious = false;
            }
            if (__instance.human.state == HumanState.Spawning)
            {
                Logger.LogInfo("--------------------------------------------");
                Logger.LogInfo("Spawning");
                _TrueGear.Play("Fall");
            }
            if (__instance.human.onGround && !isHumanOnGround)
            {
                Logger.LogInfo("--------------------------------------------");
                Logger.LogInfo("OnGround");
                _TrueGear.Play("OnGround");
                Logger.LogInfo(__instance.human.groundManager.onGround);
            }
            if (__instance.human.state != HumanState.Jump)
            {
                isJump = false;
            }
            isHumanOnGround = __instance.human.onGround;
        }
    }

    //[HarmonyPostfix, HarmonyPatch(typeof(TorsoMuscles), "FallAnimation")]
    //private static void TorsoMuscles_FallAnimation_Postfix(TorsoMuscles __instance)
    //{
    //    Logger.LogInfo("--------------------------------------------");
    //    Logger.LogInfo("TorsoMusclesFallAnimation");
    //    Logger.LogInfo(__instance.human.IsLocalPlayer);
    //}

    [HarmonyPostfix, HarmonyPatch(typeof(Human), "ClimbVibration")]
    private static void Human_ClimbVibration_Postfix(Human __instance)
    {
        if(!__instance.IsLocalPlayer)
        {
            return;
        }
        if (__instance.state != HumanState.Climb)
        {
            return;
        }
        float t = Mathf.Clamp01((__instance.controls.targetPitchAngle - 10f) / 60f);
        if ((double)Mathf.Lerp(0.2f, 1f, t) < 0.3 || !__instance.IsLocalPlayer)
        {
            return;
        }
        Logger.LogInfo("--------------------------------------------");
        Logger.LogInfo("Climb");
        _TrueGear.Play("Climb");
    }

    [HarmonyPostfix, HarmonyPatch(typeof(TorsoMuscles), "JumpAnimation")]
    private static void TorsoMuscles_JumpAnimation_Postfix(TorsoMuscles __instance)
    {
        if (__instance.human.IsLocalPlayer && !isJump)
        {
            isJump = true;
            Logger.LogInfo("--------------------------------------------");
            Logger.LogInfo("Jump");
            _TrueGear.Play("Jump");
        }
    }


}
