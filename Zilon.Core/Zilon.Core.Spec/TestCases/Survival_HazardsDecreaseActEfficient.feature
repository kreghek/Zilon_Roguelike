Feature: Survival_HazardsDecreaseActEfficient
	Чтобы избегать получение угроз выживания (голод/жажда)
	Как игроку
	Мне нужно, чтобы угрозы снижали характеристики эффективность тактических действий.

@survival @dev0
Scenario Outline: Угрозы выживания снижают эффективность тактических действий у актёра игрока.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр имеет эффект <startEffect>
	And Актёр игрока экипирован <equipmentSid>
	Then Тактическое умение <tacticalActSid> имеет эффективность Min: <minEfficient> Max: <maxEfficient>

Examples: 
| mapSize | personSid | actorNodeX | actorNodeY | startEffect   | equipmentSid | tacticalActSid | minEfficient | maxEfficient |
| 2       | captain   | 0          | 0          | Слабый голод  | short-sword  | chop           | 9            | 14           |
| 2       | captain   | 0          | 0          | Голод         | short-sword  | chop           | 7            | 11           |
| 2       | captain   | 0          | 0          | Голодание     | short-sword  | chop           | 5            | 8            |
| 2       | captain   | 0          | 0          | Слабая жажда  | short-sword  | chop           | 9            | 14           |
| 2       | captain   | 0          | 0          | Жажда         | short-sword  | chop           | 7            | 11           |
| 2       | captain   | 0          | 0          | Обезвоживание | short-sword  | chop           | 5            | 8            |

	