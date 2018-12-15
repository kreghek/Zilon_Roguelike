Feature: Fight_Dual_FailureRoll
	Чтобы ввести разнообразные сбособы экипировки и развития
	Как разработчику
	Мне нужно, чтобы персонаж мог экипироваться и атаковать двумя оружиями.

@fight @dev0
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
