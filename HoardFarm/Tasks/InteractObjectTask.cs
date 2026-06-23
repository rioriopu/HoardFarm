using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using HoardFarm.Tasks.Base;

namespace HoardFarm.Tasks;

public class InteractObjectTask(uint dataId) : BaseTask()
{
    public override unsafe bool? Run()
    {
        if (ObjectTable.TryGetFirst(e => e.BaseId == dataId, out var obj))
        {
            if (TargetSystem.Instance()->Target == (GameObject*)obj.Address)
            {
                // 距離無制限が有効なときは視線判定（checkLineOfSight）も無効化して
                // 遠隔からでもインタラクトできるようにする。
                TargetSystem.Instance()->InteractWithObject((GameObject*)obj.Address,
                                                            !Config.UnlimitedInteractDistance);
                return true;
            }

            if (EzThrottler.Throttle("Interact" + dataId))
            {
                TargetSystem.Instance()->Target = (GameObject*)obj.Address;
                return false;
            }
        }

        return false;
    }
}
