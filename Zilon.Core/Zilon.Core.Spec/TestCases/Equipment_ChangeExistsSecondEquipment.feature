Feature: Equipment_ChangeExistsSecondEquipment
	Чтобы была возможность улучшать навыки и характеристики персонажа при помощи предметов
	Как разработчику
	Мне нужно, чтобы была возможность экипировать предметы.

@equipment @dev0
Scenario Outline: Замена одного примета другим.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Актёр игрока экипирован предметом <propSid1> в слот Index: <slotIndex1>
	And Актёр игрока экипирован предметом <propSid2> в слот Index: <slotIndex2>
	And В инвентаре у актёра игрока есть предмет: <propSid3>
	When Экипирую предмет <propSid3> в слот Index: <slotIndex3>
	Then В слоте Index: <slotIndex3> актёра игрока есть <propSid3>

	Examples: 
	| personSid    | propSid1    | slotIndex1 | propSid2    | slotIndex2 | propSid3      | slotIndex3 |
	| human-person | short-sword | 2          | short-sword | 3          | wooden-shield | 3          |
	| human-person | short-sword | 2          | short-sword | 3          | wooden-shield | 2          |