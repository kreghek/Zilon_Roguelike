Feature: Survival_ConsumeProviantToDropSurvivalEffect
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы каждый ход значение сытости персонажа падало

@survival @dev0
Scenario Outline: Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод).
	Given Есть произвольная карта
	And Есть актёр игрока
	And Актёр значение <stat> равное <statValue>
	And Актёр имеет эффект <startEffect>
	When Актёр использует предмет <propSid> на себя
	Then Значение <stat> стало <expectedValue>
	And Актёр под эффектом <effect>

	Examples: 
	| stat    | statValue | startEffect   | propSid | expectedValue | effect        |
	| сытость | 0         | Слабый голод  | cheese  | 9             | нет           |
	| сытость | -25       | Голод         | cheese  | -16           | Голод         |
	| сытость | -50       | Голодание     | cheese  | -41           | Голодание     |
	| вода    | 0         | Слабая жажда  | water   | 0             | Слабая жажда  |
	| вода    | -25       | Жажда         | water   | -16           | Жажда         |
	| вода    | -50       | Обезвоживание | water   | -41           | Обезвоживание |

	