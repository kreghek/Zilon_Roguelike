Feature: Fight

@fight @dev1
Scenario Outline: Провальный удар двумя оружиями.
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Актёр игрока экипирован предметом <propSid1> в слот Index: <slotIndex1>
	And Актёр игрока экипирован предметом <propSid2> в слот Index: <slotIndex2>
	And Задаём броски для использования действий
	When Актёр игрока атакует монстра Id:<monsterId>
	# При нанесении урона будет взято среднее значение между максимальной и
	# минимальной эффективностью действия. Для 1D6 это будет 3 (int от 3.5)
	Then Монстр Id:<monsterId> имеет Hp <expectedMonsterHp>

Examples: 
| personSid    | monsterSid | monsterId | monsterNodeX | monsterNodeY | propSid1    | slotIndex1 | propSid2    | slotIndex2 | expectedMonsterHp |
| human-person | skorolon   | 1000      | 0            | 1            | short-sword | 2          | short-sword | 3          | 7                 |

@fight @dev1
Scenario Outline: Успешный удар двумя оружиями.
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Актёр игрока экипирован предметом <propSid1> в слот Index: <slotIndex1>
	And Актёр игрока экипирован предметом <propSid2> в слот Index: <slotIndex2>
	And Задаём броски для использования действий
	When Актёр игрока атакует монстра Id:<monsterId>
	# При нанесении урона будет взято среднее значение между максимальной и
	# минимальной эффективностью действия. Для 1D6 это будет 3 (int от 3.5)
	Then Монстр Id:<monsterId> имеет Hp <expectedMonsterHp>

Examples: 
| personSid    | monsterSid | monsterId | monsterNodeX | monsterNodeY | propSid1    | slotIndex1 | propSid2    | slotIndex2 | expectedMonsterHp |
| human-person | skorolon   | 1000      | 0            | 1            | short-sword | 2          | short-sword | 3          | 4                 |

@fight @dev1
Scenario Outline: Удары "руками".
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Монстр Id:<monsterId> имеет Hp <monsterHp>
	When Актёр игрока атакует монстра Id:<monsterId>
	# При нанесении урона будет взято среднее значение между максимальной и
	# минимальной эффективностью действия. Для 1D3 это будет 1 (int от 1.5)
	Then Монстр Id:<monsterId> имеет Hp <expectedMonsterHp>

Examples: 
| personSid | monsterSid | monsterId | monsterNodeX | monsterNodeY | monsterHp | expectedMonsterHp |
| captain   | rat        | 1000      | 0            | 1            | 2         | 1                 |

@fight @dev1
Scenario Outline: При наличии только щита разрешены удары "руками".
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Актёр игрока экипирован предметом <propSid> в слот Index: <slotIndex>
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Монстр Id:<monsterId> имеет Hp <monsterHp>
	When Актёр игрока атакует монстра Id:<monsterId>
	# При нанесении урона будет взято среднее значение между максимальной и
	# минимальной эффективностью действия. Для 1D3 это будет 1 (int от 1.5)
	Then Монстр Id:<monsterId> имеет Hp <expectedMonsterHp>

Examples: 
| personSid    | propSid       | slotIndex | monsterSid | monsterId | monsterNodeX | monsterNodeY | monsterHp | expectedMonsterHp |
| human-person | wooden-shield | 3         | rat        | 1000      | 0            | 1            | 2         | 1                 |
| human-person | wooden-shield | 2         | rat        | 1000      | 0            | 1            | 2         | 1                 |

@fight @dev1
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

@fight @dev1
Scenario Outline: Промахи действий с определённым типом нападения, если у противника высокий уровень обороны проив этого действия.
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Актёр игрока экипирован предметом <equipmentSid> в слот Index: 2
	And В инвентаре у актёра есть ресурс: mana количество: 10
	When Актёр игрока атакует монстра Id:<monsterId>
	Then Монстр Id:<monsterId> успешно обороняется

Examples: 
| personSid | equipmentSid | monsterSid | monsterId | monsterNodeX | monsterNodeY |
| captain   | short-sword  | pretorian  | 1000      | 0            | 1            |
| captain   | battle-axe   | pretorian  | 1000      | 0            | 1            |
| captain   | shadow-staff | hound      | 1000      | 0            | 1            |
| captain   | stilleto     | hound      | 1000      | 0            | 1            |
| captain   | sniper-rifle | jocker     | 1000      | 0            | 1            |
| captain   | minigun      | jocker     | 1000      | 0            | 1            |