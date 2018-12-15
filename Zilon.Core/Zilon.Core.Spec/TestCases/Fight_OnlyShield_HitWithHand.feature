Feature: Fight_OnlyShield_HitWithHand
	Чтобы персонаж начинал "голым"
	Как разработчику
	Мне нужно, чтобы персонаж мог атаковать даже если у него нет никакой экипировки для атаки.

@fight @dev0
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
