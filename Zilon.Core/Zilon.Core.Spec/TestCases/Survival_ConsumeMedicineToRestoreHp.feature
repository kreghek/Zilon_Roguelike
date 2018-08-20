Feature: Survival_ConsumeMedicineToRestoreHp
    Чтобы ввести микроменеджмент ресурсов и состояния персонажей
    Как игроку
    Мне нужно, чтобы при употреблении еды повышалась сытость персонажа.

@survival @dev0
Scenario Outline: Употребление медикаметов для восстановления Hp.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр игрока имеет Hp: <startHp>
	And В инвентаре у актёра есть еда: <propSid> количество: <propCount>
	When Актёр использует предмет <propSid> на себя
	Then Значение Hp равно <expectedHpValue>
	And Предмет <propSid> отсутствует в инвентаре актёра

Examples: 
	| mapSize | personSid | actorNodeX | actorNodeY | startHp | propSid | propCount | expectedHpValue |
	| 2       | captain   | 0          | 0          | 100     | med-kit | 1         | 110             | 