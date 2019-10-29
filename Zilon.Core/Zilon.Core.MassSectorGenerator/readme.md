## Для отрисовки случайной карты

Будет выбрана случайна карта и случайный уровень карты из всех, которые есть в игре.

`Zilon.SectorMapDrawer.exe scheme_catalog=путь-к-проекту/Zilon.Client/Assets/Resources/Schemes out=/maps/map-1.bmp`

## Для отрисовки конкретной карты

Используется для отладки, если какая-то конкретная комбинация генерирует ошибку.

`Zilon.SectorMapDrawer.exe dice_seed=2342 scheme_catalog=путь-к-проекту/Zilon.Client/Assets/Resources/Schemes location=genomass-cave sector=lvl2 out=/maps/map-1.bmp`