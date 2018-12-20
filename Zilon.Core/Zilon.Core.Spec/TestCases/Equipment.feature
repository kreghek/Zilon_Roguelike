Feature: Equipment

@dev0 @equipment
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