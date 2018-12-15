Feature: Equipment_DualShields_Failure
	Чтобы ввести разнообразные сбособы экипировки и развития
	Как разработчику
	Мне нужно, чтобы персонаж мог экипироваться и атаковать двумя оружиями.

@equipment @dev0
Scenario Outline: Экипировка двух щитов не разрешена.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid1>
	And В инвентаре у актёра игрока есть предмет: <propSid2>
	When Экипирую предмет <propSid1> в слот Index: <slotIndex1>
	Then В слоте Index: <slotIndex1> актёра игрока есть <propSid1>
	And Невозможна экипировка предмета <propSid2> в слот Index: <slotIndex2>

	Examples: 
	| personSid    | propSid1      | slotIndex1 | propSid2      | slotIndex2 |
	| human-person | wooden-shield | 2          | wooden-shield | 3          |
	