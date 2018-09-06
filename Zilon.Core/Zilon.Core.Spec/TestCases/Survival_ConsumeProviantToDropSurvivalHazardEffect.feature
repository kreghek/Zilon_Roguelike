Feature: Survival_ConsumeProviantToDropSurvivalHazardEffect
	Чтобы эмулировать восстановление сил персонажа при угрозах выживания
	Как разработчику
	Мне нужно, чтобы при употреблении провинта разного типа (еда/вода)
	сбрасывались соответствующие угрозы выживания при насыщении персонажа

Scenario Outline: Поглощение провианта, чтобы снимать выживальные состояния (жажда/голод).
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And Актёр значение <stat> равное <statValue>
	And Актёр имеет эффект <startEffect>
	And В инвентаре у актёра есть фейковый провиант <propSid> (<provisionStat> - <provisionEfficient>)
	When Актёр использует предмет <propSid> на себя
	Then Значение <stat> стало <expectedValue>
	And Актёр под эффектом <effect>

Examples: 
| stat    | statValue | startEffect   | propSid    | provisionStat | provisionEfficient | propCount | expectedValue | effect       |
| сытость | 0         | Слабый голод  | fake-food  | сытость       | 10                 | 1         | 9             | нет          |
| сытость | -25       | Голод         | fake-food  | сытость       | 10                 | 1         | -16           | Слабый голод |
| сытость | -50       | Голодание     | fake-food  | сытость       | 10                 | 1         | -41           | Голод        |
| вода    | 0         | Слабая жажда  | fake-water | вода          | 10                 | 1         | 9             | нет          |
| вода    | -25       | Жажда         | fake-water | вода          | 10                 | 1         | -16           | Слабая жажда |
| вода    | -50       | Обезвоживание | fake-water | вода          | 10                 | 1         | -41           | Жажда        |
