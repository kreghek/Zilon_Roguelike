Feature: Fight_Dual_SuccessRoll
	Чтобы ввести разнообразные сбособы экипировки и развития
	Как разработчику
	Мне нужно, чтобы персонаж мог экипироваться и атаковать двумя оружиями.

@fight @dev0
Scenario Outline: Успешный удар двумя оружиями.
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Монстр Id:<monsterId> имеет Hp <monsterHp>
	And Актёр игрока экипирован предметом <propSid1> в слот Index: <slotIndex1>
	And Актёр игрока экипирован предметом <propSid2> в слот Index: <slotIndex2>
	When Актёр игрока атакует монстра Id:<monsterId>
	# При нанесении урона будет взято среднее значение между максимальной и
	# минимальной эффективностью действия. Для 1D3 это будет 1 (int от 1.5)
	Then Монстр Id:<monsterId> имеет Hp <expectedMonsterHp>

Examples: 
| personSid    | monsterSid | monsterId | monsterNodeX | monsterNodeY | monsterHp | propSid1    | slotIndex1 | propSid2    | slotIndex2 | expectedMonsterHp |
| human-person | rat        | 1000      | 0            | 1            | 10        | short-sword | 2          | short-sword | 3          | 4                 |
