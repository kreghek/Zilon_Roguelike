Feature: Survival_HazardsDecreaseEvolutionStats
	Чтобы избегать получение угроз выживания (голод/жажда)
	Как игроку
	Мне нужно, чтобы угрозы снижали характеристики актёра, пока актёр от них не избавиться.

@survival @dev0
Scenario Outline: Угрозы выживания снижают характеристики модуля сражения у актёра игрока.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр имеет эффект <startEffect>
	Then Актёр имеет характристику модуля сражения <combatStat> равную <combatStatValue>

Examples: 
| mapSize | personSid | actorNodeX | actorNodeY | startEffect   | combatStat | combatStatValue |
| 2       | captain   | 0          | 0          | Слабый голод  | melee      | 9               |
| 2       | captain   | 0          | 0          | Голод         | melee      | 7               |
| 2       | captain   | 0          | 0          | Голодание     | melee      | 5               |
| 2       | captain   | 0          | 0          | Слабая жажда  | melee      | 9               |
| 2       | captain   | 0          | 0          | Жажда         | melee      | 7               |
| 2       | captain   | 0          | 0          | Обезвоживание | melee      | 5               |

	