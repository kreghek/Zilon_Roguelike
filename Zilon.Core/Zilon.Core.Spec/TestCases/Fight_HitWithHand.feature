Feature: Fight_HitWithHand
	Чтобы персонаж начинал "голым"
	Как разработчику
	Мне нужно, чтобы персонаж мог атаковать даже если у него нет никакой экипировки.

@fight @dev0
Scenario Outline: Удары "руками".
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Монстр Id:<monsterId> имеет Hp <monsterHp>
	When Актёр игрока атакует монстра Id:<monsterId>
	# При нанесении урона будет взято среднее значение между максимальной и
	# минимальной эффективностью действия
	Then Монстр Id:<monsterId> имеет Hp <expectedMonsterHp>

Examples: 
| personSid | monsterSid | monsterId | monsterNodeX | monsterNodeY | monsterHp | expectedMonsterHp |
| captain   | rat        | 1000      | 0            | 2            | 100       | 92                |
