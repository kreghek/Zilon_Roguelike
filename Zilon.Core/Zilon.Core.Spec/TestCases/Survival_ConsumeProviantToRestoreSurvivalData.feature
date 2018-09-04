Feature: Survival_ConsumeProviantToRestoreSurvivalData
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы при употреблении еды повышалась сытость персонажа

@survival @dev0
Scenario Outline: Употребление провианта для восстановления показателей выживания
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And В инвентаре у актёра есть еда: <propSid> количество: <propCount>
	And Запущен игровой цикл
	When Актёр использует предмет <propSid> на себя
	Then Значение <stat> повысилось на <propValue> и уменьшилось на <statRate> за игровой цикл и стало <expectedStatValue>
	And Предмет <propSid> отсутствует в инвентаре актёра


Examples: 
	| propSid | propCount | stat    | propValue | statRate | expectedStatValue |
	| cheese  | 1         | сытость | 50        | 1        | 99                |
	| water   | 1         | вода    | 50        | 1        | 99                |