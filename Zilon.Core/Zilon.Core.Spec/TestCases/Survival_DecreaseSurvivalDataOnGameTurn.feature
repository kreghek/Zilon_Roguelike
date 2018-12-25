Feature: Survival_DecreaseSurvivalDataOnGameTurn
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы каждый ход значение показателей сытости/воды/усталости персонажа падало

@survival @dev0
Scenario Outline: Падение показателей выживания каждый игровой ход
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	When Я перемещаю персонажа на <moveDistance> клетку
	Then Значение <stat> уменьшилось на <statRate> и стало <expectedStatValue>

Examples: 
| moveDistance | stat    | statRate | expectedStatValue |
| 1            | сытость | 1        | 149               |
| 1            | вода    | 1        | 149               |