Feature: Equipment_ChangeHandsToEquipment
	Чтобы была возможность улучшать навыки и характеристики персонажа при помощи предметов
	Как разработчику
	Мне нужно, чтобы была возможность экипировать предметы.

@equipment @dev0
Scenario Outline: Экипировка предмета в подходящий слот.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid>
	When Экипирую предмет <propSid> в слот Index: <slotIndex>
	Then В слоте Index: <testedSlotIndex> актёра игрока есть <propSid>

	Examples: 
	| personSid | propSid     | slotIndex | testedSlotIndex | paramType | paramValue |
	| captain   | short-sword | 2         | 2               | -         | 0          |