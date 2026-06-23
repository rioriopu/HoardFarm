using System;
using System.Collections.Generic;
using Dalamud.Hooking;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Camera = FFXIVClientStructs.FFXIV.Client.Game.Camera;

namespace HoardFarm.Service;

/// <summary>
/// インタラクト距離を実質無制限にするフック群。
/// DailyRoutines の OptimizedInteraction モジュール（v2.1.0.1）で使用されている
/// 「距離 / 視界 / 高低 / カメラ位置」判定を無効化するシグネチャを移植したもの。
/// Dalamud の HookFromSignature は E8/E9（call/jmp 相対命令）を自動解決するため、
/// 元のシグネチャをそのまま利用できる。
///
/// シグネチャが解決できなかったフックは個別にスキップされ、他のフックには影響しない
/// （ゲーム更新でシグネチャがずれてもクラッシュしないようにするため）。
/// </summary>
public sealed unsafe class InteractDistanceService : IDisposable
{
    // 目標との距離を返す関数 → 常に 0（＝常に至近距離扱い）
    private const string CheckTargetDistanceSig =
        "E8 ?? ?? ?? ?? 0F 2F 05 ?? ?? ?? ?? 76 ?? 48 8B 03 48 8B CB FF 50 ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? EB";
    private delegate float CheckTargetDistanceDelegate(GameObject* localPlayer, GameObject* target);

    // 目標が視界範囲内かどうか → 常に true
    private const string IsObjectInViewRangeSig =
        "E8 ?? ?? ?? ?? 84 C0 75 ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B C8 48 8B 10 FF 52 ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? E9";
    private delegate bool IsObjectInViewRangeDelegate(TargetSystem* system, GameObject* gameObject);

    // 目標との高低差チェック → 常に true
    private const string CheckTargetPositionSig = "40 53 57 41 56 48 83 EC ?? 48 8B 02";
    private delegate bool CheckTargetPositionDelegate(
        EventFramework* framework, GameObject* source, GameObject* target, ushort interactType, bool sendError, byte a6);

    // 「現在の位置ではこの操作を実行できません」(カメラ位置) → 常に true
    private const string CheckCameraPositionSig = "E8 ?? ?? ?? ?? 84 C0 75 ?? B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? EB";
    private delegate bool CheckCameraPositionDelegate(TargetSystem* system, Camera* activeCamera, GameObject* obj);

    private readonly List<IDisposable> hooks = [];
    private readonly List<Action<bool>> toggles = [];
    private bool enabled;

    public InteractDistanceService()
    {
        Add<CheckTargetDistanceDelegate>(CheckTargetDistanceSig, (_, _) => 0f, "CheckTargetDistance");
        Add<IsObjectInViewRangeDelegate>(IsObjectInViewRangeSig, (_, _) => true, "IsObjectInViewRange");
        Add<CheckTargetPositionDelegate>(CheckTargetPositionSig, (_, _, _, _, _, _) => true, "CheckTargetPosition");
        Add<CheckCameraPositionDelegate>(CheckCameraPositionSig, (_, _, _) => true, "CheckCameraPosition");

        SetEnabled(Config.UnlimitedInteractDistance);
    }

    public bool Enabled => enabled;

    /// <summary>フックの有効/無効を切り替える。</summary>
    public void SetEnabled(bool value)
    {
        enabled = value;
        foreach (var toggle in toggles)
            toggle(value);
    }

    private void Add<T>(string signature, T detour, string name) where T : Delegate
    {
        try
        {
            var hook = Svc.Hook.HookFromSignature(signature, detour);
            hooks.Add(hook);
            toggles.Add(on =>
            {
                if (on) hook.Enable();
                else hook.Disable();
            });
        }
        catch (Exception e)
        {
            PluginLog.Warning(
                $"[HoardFarm] インタラクト距離フック '{name}' の初期化に失敗しました（ゲーム更新でシグネチャがずれた可能性）: {e.Message}");
        }
    }

    public void Dispose()
    {
        foreach (var hook in hooks)
            hook.Dispose();
        hooks.Clear();
        toggles.Clear();
    }
}
