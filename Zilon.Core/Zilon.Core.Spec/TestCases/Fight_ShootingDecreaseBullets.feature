Feature: Fight_ShootingDecreaseBullets
	Чтобы ограничить использование стрелкового оружия
	Как игроку
	Мне нужно, чтобы при стрельбе расходовались патроны, для каждого оружия свои.

@fight @dev0
Scenario Outline: Расход патронов при стрельбе.
	Given Есть произвольная карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр игрока экипирован <equipmentSid>
	And Актёр игрока имеет в инвентаре <resourceSid> количеством <resourceCount>
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	When Актёр игрока стреляет в монстра Id:<monsterId>
	Then Актёр игрока имеет в инвентаре <resourceSid> количеством <expectedResourceCount>

Examples: 
| mapSize | personSid | actorNodeX | actorNodeY | equipmentSid | resourceSid | resourceCount | monsterSid | monsterId | monsterNodeX | monsterNodeY | expectedResourceCount |
| 3       | captain   | 0          | 0          | pistol       | bullet-45   | 10            | rat        | 1000      | 0            | 2            | 9                     |
