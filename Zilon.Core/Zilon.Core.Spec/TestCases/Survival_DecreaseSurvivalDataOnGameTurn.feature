Feature: Survival_DecreaseSurvivalDataOnGameTurn
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы каждый ход значение показателей сытости/воды/усталости персонажа падало

@survival @dev0
Scenario Outline: Падение показателей выживания каждый игровой ход
	Given Есть произвольная карта
	And Есть актёр игрока
	When Я перемещаю персонажа на <moveDistance> клетку
	Then Значение <stat> уменьшилось на <statRate> и стало <expectedStatValue>

Examples: 
| moveDistance | stat    | statRate | expectedStatValue |
| 1            | сытость | 1        | 49                |
| 1            | вода    | 1        | 49                |