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
	And В инвентаре у актёра есть еда: <propSid> количество: <propCount>
	When Актёр использует предмет <propSid> на себя
	Then Значение <stat> стало <expectedValue>
	And Актёр под эффектом <effect>

Examples: 
| stat    | statValue | startEffect   | propSid | propCount | expectedValue | effect       |
| сытость | 0         | Слабый голод  | cheese  | 1         | 9             | нет          |
| сытость | -25       | Голод         | cheese  | 1         | -16           | Слабый голод |
| сытость | -50       | Голодание     | cheese  | 1         | -41           | Голод        |
| вода    | 0         | Слабая жажда  | water   | 1         | 9             | нет          |
| вода    | -25       | Жажда         | water   | 1         | -16           | Слабая жажда |
| вода    | -50       | Обезвоживание | water   | 1         | -41           | Жажда        |