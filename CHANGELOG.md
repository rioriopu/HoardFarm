# Changelog

I try to keep this changelog up to date with the latest changes in the project.

## [1.6.0.9]
- 「インタラクト距離を無制限にする」が有効なとき、NPC「キュウセイ」へ歩かず遠隔でインタラクトするように改善
  - `MoveToHoHTask`: 距離無制限時はオノコロへテレポートするだけ（マウント・経路探索を省略）。テレポート後はキュウセイがスポーン範囲内に居るため遠隔ターゲット・インタラクト可能
  - `KyuseiInteractable()`: 距離無制限時はキュウセイがオブジェクトテーブルに存在すれば true（距離判定をスキップ）
  - navmesh（vnavmesh）待ちの解消: navmesh は経路探索にのみ必要なため、距離無制限かつ HoH 外（ルビーシー側）では構築完了を待たずに進行するよう変更（HoH 内のホード探索では引き続き navmesh を必須）

## [1.6.0.8]
- ログイン前・ゾーン遷移中・ロード中などに UI 描画でクラッシュする不具合を修正
  - `Player.Territory.Value.RowId` が、territory が無効（0 など）のとき Lumina の
    `RowRef.Value` で `InvalidOperationException` をスローしていた
  - 行を解決しない `Player.Territory.RowId` に変更（InHoH / InRubySea / テレポート判定など全箇所）

## [1.6.0.7]
- Dalamud 15.0.2.1 / FFXIVClientStructs 7.51 追従の再ビルド（挙動変更なし）

## [1.6.0.6]
- 「インタラクト距離を無制限にする」機能を追加（設定画面のチェックボックスで切り替え）
  - DailyRoutines の OptimizedInteraction モジュールの距離/視界/高低/カメラ位置判定の無効化フックを移植
  - 距離・視界範囲・高低差・カメラ位置の各判定を無効化し、どの距離からでもオブジェクトにインタラクト可能にする
  - シグネチャは現行ゲームバイナリで一意に一致することを検証済み。解決に失敗したフックは個別にスキップされクラッシュしない
  - 設定画面の「Want to help with localization?」ボタンをこのチェックボックスに置き換え

## [1.6.0.5]
- 秘宝を発見してもしなくても即座に退出してしまう不具合を修正
  - LogMessage（7272/7273/7274）にはフロア名などのプレースホルダー（マクロ）が含まれており、`GetText()` で結合した全文が実際のチャットメッセージと完全一致しないため、「秘宝の気配」検知が常に失敗して `hoardAvailable` が false のままになっていた
  - 完全一致ではなく、各メッセージの最長テキストセグメントによる部分一致判定に変更（プレースホルダーを含まないため言語非依存・英/日/独/仏で区別性を検証済み）

## [1.6.0.4]
- UI を全面的に日本語化（`Strings.ja.resx` を全 80 キー翻訳）
- 支援リンクを Ko-fi から Patreon に変更（https://www.patreon.com/cw/SuppotToEstell）
- 支援ボタンのテキストを「ご支援」に変更

## [1.6.0.3]
- Dalamud API 15 / Dalamud 15.x（net10.0）対応の再ビルド（rioriopu フォーク）
- AutoRetainerAPI を最新版に追従（ECommons 3.2.0.11）
- API 変更への追従:
  - `IClientState.TerritoryChanged` の引数が `ushort` から `uint` に変更
  - `IChatGui.ChatMessage` が `IHandleableChatMessage` ベースの新デリゲートに変更
  - `FFXIVClientStructs` の `ValueType` を `AtkValueType` に更新

## [1.5.2.6]
- Use default Dalamud title bar buttons by @carvelli
- Retainer home world check by @decorwdyun

## [1.5.2.5]
- just version push

## [1.5.2.4]
- changed repo url

## [1.5.2.3]
- tamed AutoRetainer
- camera now aligns to the player while running

## [1.5.2.2]
- some performance things and code cleanup

## [1.5.2.1]
- just a message fix

## [1.5.2.0]
- better AutoRetainer handling

## [1.5.1.0]
- timer bugfix

## [1.5.0.0]
- safety and magicite usage if no concealment is available
- fixed some bugs with the new retainers
- improved search logic
- timeout leave

## [1.4.0.0]
- run retainers between runs
- speed up some things to save 1-2 seconds per run
- some refactorings (but i still dislike it)
- overall statistics now can show days

## [1.3.1.0]
- initial setup check bugfix

## [1.3.0.0]
- small overlay button on deep dungeon menu to open the farm window
- checks for minimal setup/floor before starting the farm
- some UI additions like support button and error message
- implemented paranoid mode
- safety checks for pomanders
- allow enter with deep dungeon menu open

## [1.2.1.0]
- disabling command now finishes the run first

## [1.2.0.0]
- command arguments control
- implemented farm modes Efficiency and Time

## [1.1.1.1]
- fixed stupid search logic bug (no really i fucked up xD)

## [1.1.1.0]
- search logic bugfixes
- added achievement tracker to statistics


## [1.1.0.0]
- Various bugfixes
- more statistics
- first draft of hoard searching

## [1.0.1.0]

- Various bugfixes preventing getting some hoards

## [1.0.0.0]

- Initial release
