Feature: Fight_ShootingDecreaseBullets
	Чтобы ограничить использование стрелкового оружия
	Как игроку
	Мне нужно, чтобы при стрельбе расходовались патроны, для каждого оружия свои.

@fight @dev0
Scenario Outline: Расход патронов при стрельбе.
	Given Есть карта размером <mapSize>
	And Есть актёр игрока класса <personSid> в ячейке (<actorNodeX>, <actorNodeY>)
	And Актёр игрока экипирован предметом <equipmentSid> в слот Index: 2
	And В инвентаре у актёра есть ресурс: <resourceSid> количество: <resourceCount>
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	When Актёр игрока атакует монстра Id:<monsterId>
	Then В инвентаре у актёра есть ресурс: <resourceSid> количество: <expectedResourceCount>

Examples: 
| mapSize | personSid    | actorNodeX | actorNodeY | equipmentSid | resourceSid | resourceCount | monsterSid | monsterId | monsterNodeX | monsterNodeY | expectedResourceCount |
| 3       | human-person | 0          | 0          | pistol       | bullet-45   | 10            | rat        | 1000      | 0            | 2            | 9                     |
