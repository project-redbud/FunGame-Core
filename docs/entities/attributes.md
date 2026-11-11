# 核心属性与战斗属性加成

源码：`Entity/Character/Character.cs`。回归测试：`tests/FunGame.Core.Tests.Entities`。

`ExSTR`、`ExAGI`、`ExINT` 是可写点数加成；`ExSTRPercentage`、`ExAGIPercentage`、`ExINTPercentage` 是可写百分比加成。
`ExSTR2`、`ExAGI2`、`ExINT2` 为对应基础属性乘以百分比的只读派生值，随等级成长重新计算。

| 属性 | 计算方式 |
| --- | --- |
| `ExHP` | `(ExSTR + ExSTR2) × STRtoHPFactor` |
| `ExDEF` | `(ExSTR + ExSTR2) × STRtoDEFFactor` |
| `ExMP` | `(ExINT + ExINT2) × INTtoMPFactor` |
| `ExATK`、`ExPrimaryAttributeValue` | 当前主属性的点数加成与百分比派生值之和；`None` 按力量计算 |

`PrimaryAttributeValue = BasePrimaryAttributeValue + ExPrimaryAttributeValue`。
战斗属性自身的点数和百分比加成分别写入 `ExATK2` / `ExDEF2` / `ExHP2` / `ExMP2` 与对应 `Ex*Percentage`，独立叠加。
`MaxHP` 与 `MaxMP` 的下限为 1，提高上限不会自动补满当前 HP / MP。
角色 JSON 使用 `ExSTR` / `ExAGI` / `ExINT` 和对应百分比字段保存加成，派生值不写入存档。

```powershell
dotnet build FunGame.Core.sln -c Release
dotnet test FunGame.Core.sln -c Release --no-build
```
