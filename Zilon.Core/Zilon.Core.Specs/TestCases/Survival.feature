Feature: Survival
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы при употреблении еды повышалась сытость персонажа
	Чтобы эмулировать восстановление сил персонажа при угрозах выживания
	Как разработчику
	Мне нужно, чтобы при употреблении провинта разного типа (еда/вода)
	сбрасывались соответствующие угрозы выживания при насыщении персонажа

Background:
	Given Есть карта размером 2
	And Есть актёр игрока класса human-person в ячейке (0, 0)

# Rewrite this test to use test-depending stats instead real
#@survival @dev1 @dev2 @dev17
#Scenario Outline: Снятие угроз выживания
#	Given Актёр имеет эффект <startEffect>
#	And В инвентаре у актёра есть еда: <propSid> количество: 100
#	When Актёр использует предмет <propSid> на себя 1 раз
#	Then Актёр под эффектом <effect>

#Examples: 
#    | startEffect  | propSid      | effect       |
#    | Слабый голод | packed-food  | нет          |
#    | Слабая жажда | water-bottle | нет          |
#    | Голод        | packed-food  | Слабый голод |
#    | Жажда        | water-bottle | нет          |

@survival @dev1 @dev17
Scenario Outline: Употребление медикаментов для восстановления Hp.
	Given Актёр имеет эффект <startEffect>
	And В инвентаре у актёра есть еда: <propSid> количество: 100
	When Актёр использует предмет <propSid> на себя <propCount> раз
	Then Актёр под эффектами <effects>

Examples: 
	| startEffect      | propSid | propCount | effects           |
	# Была смертельная рана. Но после применения медпакета (сильно лечит)
	# у актёра больше нет эффекта раны. Только интоксикация.
	| Смертельная рана | med-kit | 1         | Слабая токсикация |

@survival @dev2 @dev17
Scenario Outline: Употребление медикаментов снижает сытость и воду.
	Given В инвентаре у актёра есть еда: <propSid> количество: 100
	When Актёр использует предмет <propSid> на себя <iterations> раз
	Then Актёр под эффектом <effect>

	Examples: 
	| iterations | effect                 | propSid |
	| 1          | Слабая токсикация      | med-kit |
	| 2          | Сильная токсикация     | med-kit |
	| 3          | Смертельная токсикация | med-kit |

# Rewrite this test to use test-depending stats instead real
#@survival @dev1 @dev17
#Scenario Outline: Наступление выживальных состояний (жажда/голод/утомление)
#	# special perk to exclude other hazard influence
#	Given Актёр игрока получает перк <testPerk>
#	When Я жду <iterations> итераций
#	Then Актёр под эффектом <effect>
#
#	Examples: 
#	| iterations | stat    | effect        | testPerk        |
#	| 250        | сытость | Слабый голод  | thrist-immunity |
#	| 1000       | сытость | Голод         | thrist-immunity |
#	| 3300       | сытость | Голодание     | thrist-immunity |
#	| 268        | вода    | Слабая жажда  | hunger-immunity |
#	| 400        | вода    | Жажда         | hunger-immunity |
#	| 1288       | вода    | Обезвоживание | hunger-immunity |

#@survival @dev1 @dev17
#Scenario Outline: Эффекты угроз выживания наносят урон актёру.
#	Given Актёр имеет эффект <startEffect>
#	When Я жду <waitIterations> итераций
#	Then Актёр под эффектом <effect>
#
#	Examples: 
#	| startEffect   | waitIterations | effect      |
#	| Голодание     | 3              | Слабая рана |
#	| Обезвоживание | 3              | Слабая рана |

# Rewrite this test to use test-depending stats instead real
#@survival @dev1 @dev17
#Scenario Outline: Угрозы выживания (имеются изначально) снижают эффективность тактических действий у актёра игрока.
#	Given В инвентаре у актёра игрока есть предмет: <equipmentSid>
#	And Актёр имеет эффект <startEffect>
#	When Экипирую предмет <equipmentSid> в слот Index: <slotIndex>
#	And Жду 1000 единиц времени
#	Then Тактическое умение <tacticalActSid> имеет дебафф на эффективность

#Examples: 
#| startEffect   | equipmentSid | slotIndex | tacticalActSid |
#| Слабый голод  | short-sword  | 2         | weak-swing     |
#| Голод         | short-sword  | 2         | weak-swing     |
#| Голодание     | short-sword  | 2         | weak-swing     |
#| Слабая жажда  | short-sword  | 2         | weak-swing     |
#| Жажда         | short-sword  | 2         | weak-swing     |
#| Обезвоживание | short-sword  | 2         | weak-swing     |