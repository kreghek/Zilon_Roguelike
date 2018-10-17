Feature: Survival_ConsumeMedicineToRestoreHp
    Чтобы ввести микроменеджмент ресурсов и состояния персонажей
    Как игроку
    Мне нужно, чтобы при употреблении еды повышалась сытость персонажа.

@survival @dev0
Scenario Outline: Употребление медикаметов для восстановления Hp.
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And Актёр игрока имеет Hp: <startHp>
	And В инвентаре у актёра есть еда: <propSid> количество: <propCount>
	When Актёр использует предмет <propSid> на себя
	Then Актёр игрока имеет запас hp <expectedHpValue>
	And Предмет <propSid> отсутствует в инвентаре актёра

Examples: 
	| startHp | propSid | propCount | expectedHpValue |
	| 1       | med-kit | 1         | 5               | 