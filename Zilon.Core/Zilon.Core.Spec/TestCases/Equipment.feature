Feature: Equipment

@dev1 @equipment
Scenario Outline: Снятие экипированного предмета из слота.
	Given Есть карта размером 2
	And Есть актёр игрока класса human-person в ячейке (0, 0)
	And Актёр игрока экипирован предметом steel-helmet в слот Index: 0
	And Актёр игрока экипирован предметом steel-armor в слот Index: 1
	And Актёр игрока экипирован предметом battle-axe в слот Index: 2
	And Актёр игрока экипирован предметом short-sword в слот Index: 3
	When Снимаю экипировку из слота <slotIndex>
	Then В слоте Index: <slotIndex> актёра игрока ничего нет
	And У актёра в инвентаре есть <propSid>

	Examples: 
	| slotIndex | propSid      |
	| 0         | steel-helmet |
	| 1         | steel-armor  |
	| 2         | battle-axe   |
	| 3         | short-sword  |

@equipment @dev1
Scenario Outline: Замена одного примета другим.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid1>
	And В инвентаре у актёра игрока есть предмет: <propSid2>
	When Экипирую предмет <propSid1> в слот Index: <slotIndex>
	And Экипирую предмет <propSid2> в слот Index: <slotIndex>
	Then В слоте Index: <slotIndex> актёра игрока есть <propSid2>

	Examples: 
	| personSid    | propSid1    | propSid2    | slotIndex |
	| human-person | short-sword | short-sword | 2         |
	| human-person | pistol      | short-sword | 2         |
	| human-person | short-sword | pistol      | 2         |

@equipment @dev1
Scenario Outline: Замена одного примета другим. Работа с щитами.
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

@equipment @dev1
Scenario Outline: Экипировка предмета в подходящий слот.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid>
	When Экипирую предмет <propSid> в слот Index: <slotIndex>
	Then В слоте Index: <testedSlotIndex> актёра игрока есть <propSid>
	And Параметр <paramType> равен <paramValue>

	Examples: 
	| personSid | propSid     | slotIndex | testedSlotIndex | paramType | paramValue |
	| captain   | short-sword | 2         | 2               | -         | 0          |

@equipment @dev1
Scenario Outline: Экипировка двух пистолетов не разрешена.
	Given Есть карта размером 2
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And В инвентаре у актёра игрока есть предмет: <propSid1>
	And В инвентаре у актёра игрока есть предмет: <propSid2>
	When Экипирую предмет <propSid1> в слот Index: <slotIndex1>
	Then В слоте Index: <slotIndex1> актёра игрока есть <propSid1>
	And Невозможна экипировка предмета <propSid2> в слот Index: <slotIndex2>

	Examples: 
	| personSid    | propSid1    | slotIndex1 | propSid2 | slotIndex2 |
	| human-person | pistol      | 2          | pistol   | 3          |
	| human-person | short-sword | 2          | pistol   | 3          |

@equipment @dev1
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

@equipment @dev1
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

@props @equipment @dev1
Scenario: Берём ресурсы из сундука. После этого назначаем экипировку, которая уже была в инвентаре, персонажу.
	Given Персонаж стоит возле сундука с ресурсами
	When Я открываю сундук
	And Я открываю окно с содержимым сундука
	And Я переношу содержимое себе в инвентарь
	And Я открываю окна инвентаря
	And Я назначаю экипировку из инвентаря
	Then Персонажу назначена экипировка
	And Текущая экипировка перенесена в инвентарь