using System.Collections;
using HoardFarm.Tasks.Base;

namespace HoardFarm.Tasks.TaskGroups;

public class MoveToHoHTask : IBaseTaskGroup
{
    public ArrayList GetTaskList()
    {
        // インタラクト距離無制限が有効なら、キュウセイへ歩いて近づかず
        // オノコロへテレポートするだけ（マウント・経路探索を省略）。
        // テレポート後はキュウセイがスポーン範囲内（約15〜30ヤルム）に居るため、
        // そのまま遠隔でインタラクトできる。
        if (Config.UnlimitedInteractDistance)
            return [new TeleportTask(OnokoroAetherytId, RubySeaMapId)];

        return [
            new TeleportTask(OnokoroAetherytId, RubySeaMapId),
            new MountTask(),
            new PathfindTask(KyuseiLocation)
        ];
    }
}
