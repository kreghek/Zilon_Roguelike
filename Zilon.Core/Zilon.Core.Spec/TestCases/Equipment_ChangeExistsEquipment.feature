Feature: Equipment_ChangeExistsEquipment
	Чтобы была возможность улучшать навыки и характеристики персонажа при помощи предметов
	Как разработчику
	Мне нужно, чтобы была возможность экипировать предметы.

@equipment @dev0
Scenario Outline: Замена одного примета другим.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid>
	When Экипирую предмет <propSid1> в слот Index: <propSid1>
	And Экипирую предмет <propSid2> в слот Index: <slotIndex>
	Then В слоте Index: <slotIndex> актёра игрока есть <propSid2>

	Examples: 
	| personSid    | propSid1    | propSid2    | slotIndex |
	| human-person | short-sword | short-sword | 2         |