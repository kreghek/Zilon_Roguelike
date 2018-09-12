Feature: Survival_HazardsDecreaseActEfficient
	Чтобы избегать получение угроз выживания (голод/жажда)
	Как игроку
	Мне нужно, чтобы угрозы снижали характеристики эффективность тактических действий.

@survival @dev0
Scenario Outline: Угрозы выживания (имеются изначально) снижают эффективность тактических действий у актёра игрока.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And В инвентаре у актёра игрока есть предмет: <equipmentSid>
	And Актёр имеет эффект <startEffect>
	When Экипирую предмет <equipmentSid> в слот Index: <slotIndex>
	Then Тактическое умение <tacticalActSid> имеет эффективность Min: <minEfficient> Max: <maxEfficient>

Examples: 
| mapSize | personSid | actorNodeX | actorNodeY | startEffect   | equipmentSid | slotIndex | tacticalActSid | minEfficient | maxEfficient |
| 2       | captain   | 0          | 0          | Слабый голод  | short-sword  | 2         | chop           | 9            | 14           |
| 2       | captain   | 0          | 0          | Голод         | short-sword  | 2         | chop           | 7            | 11           |
| 2       | captain   | 0          | 0          | Голодание     | short-sword  | 2         | chop           | 5            | 8            |
| 2       | captain   | 0          | 0          | Слабая жажда  | short-sword  | 2         | chop           | 9            | 14           |
| 2       | captain   | 0          | 0          | Жажда         | short-sword  | 2         | chop           | 7            | 11           |
| 2       | captain   | 0          | 0          | Обезвоживание | short-sword  | 2         | chop           | 5            | 8            |

	