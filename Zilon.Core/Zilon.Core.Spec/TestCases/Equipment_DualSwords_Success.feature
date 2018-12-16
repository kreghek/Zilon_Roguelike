Feature: Equipment_DualSwords_Success
	Чтобы ввести разнообразные сбособы экипировки и развития
	Как разработчику
	Мне нужно, чтобы персонаж мог экипироваться и атаковать двумя оружиями.

@equipment @dev0
Scenario Outline: Экипировка двух оружий.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid1>
	And В инвентаре у актёра игрока есть предмет: <propSid2>
	When Экипирую предмет <propSid1> в слот Index: <slotIndex1>
	And Экипирую предмет <propSid2> в слот Index: <slotIndex2>
	Then В слоте Index: <slotIndex1> актёра игрока есть <propSid1>
	And В слоте Index: <slotIndex2> актёра игрока есть <propSid2>

	Examples: 
	| personSid    | propSid1    | slotIndex1 | propSid2    | slotIndex2 |
	| human-person | short-sword | 2          | short-sword | 3          |