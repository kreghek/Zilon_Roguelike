Feature: Survival_ConsumeProviantToRestoreSurvivalData
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы при употреблении еды повышалась сытость персонажа

@survival @dev0
Scenario Outline: Употребление провианта для восстановления показателей выживания
	Given Есть произвольная карта
	And Есть актёр игрока
	And В инвентаре у актёра есть еда: <propSid> количество: <propCount>
	When Актёр использует предмет <propSid> на себя
	Then Значение <stat> повысилось на <propValue> единиц и уменьшилось на <statRate> за игровой цикл и стало <expectedStatValue>
	And Предмет <propSid> отсутствует в инвентаре актёра


Examples: 
| propSid | propCount | stat    | propValue | statRate | expectedStatValue |
| cheese  | 1         | сытость | 10        | 1        | 59                |
| water   | 1         | вода    | 10        | 1        | 59                |