Feature: Survival_HazardsDecreaseEvolutionStats_Become
	Чтобы избегать получение угроз выживания (голод/жажда)
	Как игроку
	Мне нужно, чтобы угрозы снижали характеристики характеристики модуля сражения.

@survival @dev0
Scenario Outline: Угрозы выживания (появляются в процессе) снижают характеристики модуля сражения у актёра игрока.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр игрока экипирован <equipmentSid>
	When Я перемещаю персонажа на <moveDistance> клетку
	Then Актёр под эффектом <expectedEffect>
	Then Актёр имеет характристику модуля сражения <combatStat> равную <combatStatValue>

Examples: 
| mapSize | personSid | actorNodeX | actorNodeY | equipmentSid | moveDistance | expectedEffect | combatStat | combatStatValue |
| 2       | captain   | 0          | 0          | short-sword  | 50           | Слабый голод   | melee      | 9               |
| 2       | captain   | 0          | 0          | short-sword  | 75           | Голод          | melee      | 7               |
| 2       | captain   | 0          | 0          | short-sword  | 100          | Голодание      | melee      | 5               |
| 2       | captain   | 0          | 0          | short-sword  | 50           | Слабая жажда   | melee      | 9               |
| 2       | captain   | 0          | 0          | short-sword  | 75           | Жажда          | melee      | 7               |
| 2       | captain   | 0          | 0          | short-sword  | 100          | Обезвоживание  | melee      | 5               |

	