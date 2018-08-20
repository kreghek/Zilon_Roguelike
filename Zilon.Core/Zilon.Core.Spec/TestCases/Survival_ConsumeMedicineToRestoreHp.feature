Feature: Survival_ConsumeMedicineToRestoreHp
    Чтобы ввести микроменеджмент ресурсов и состояния персонажей
    Как игроку
    Мне нужно, чтобы при употреблении еды повышалась сытость персонажа.

@survival @dev0
Scenario Outline: Употребление медикаметов для восстановления Hp.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр имеет Hp: 100
	And В инвентаре у актёра есть еда: <propSid> количество: <propCount>
	When Актёр использует предмет <propSid> на себя
	Then Значение Hp повысилось равно <expectedHpValue>
	And Предмет <propSid> отсутствует в инвентаре актёра

Examples: 
	| mapSize | personSid | actorNodeX | actorNodeY | equipmentSid | moveDistance | expectedEffect | combatStat | combatStatValue |
	| 2       | captain   | 0          | 0          | short-sword  | 50           | Слабый голод   | melee      | 8               |
	| 2       | captain   | 0          | 0          | short-sword  | 75           | Голод          | melee      | 4               |
	| 2       | captain   | 0          | 0          | short-sword  | 100          | Голодание      | melee      | 1               |