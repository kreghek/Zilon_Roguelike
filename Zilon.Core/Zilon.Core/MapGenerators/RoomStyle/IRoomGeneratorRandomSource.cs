﻿using System.Collections.Generic;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Интерфейс источника рандома для генератора карт из комнат.
    /// </summary>
    public interface IRoomGeneratorRandomSource
    {
        /// <summary>
        /// Выбирает комнаты, с которыми есть соединение.
        /// </summary>
        /// <param name="currentRoom"> Текущая комната, для которой ищуются соединённые соседи. </param>
        /// <param name="maxNeighbors"> Максимальное количество соединённых соседей. </param>
        /// <param name="availableRooms"> Набор доступных для соединения соседенй. </param>
        /// <returns> Возвращает целевые комнаты для соединения. </returns>
        Room[] RollConnectedRooms(Room currentRoom, int maxNeighbors, IList<Room> availableRooms);

        /// <summary>
        /// Выбрасывает случаный набор элементов интерьера комнаты.
        /// </summary>
        /// <param name="roomWidth">Ширина комнаты.</param>
        /// <param name="roomHeight">Высота комнаты.</param>
        /// <returns> Возвращает набор элементов интерьера комнаты. </returns>
        RoomInteriorObjectMeta[] RollInteriorObjects(int roomWidth, int roomHeight);

        /// <summary>
        /// Выбрасывает случайный набор уникальных координат в матрице комнат указаной длины.
        /// </summary>
        /// <param name="roomGridSize"> Размер матрицы комнат. </param>
        /// <param name="roomCount"> Количество комнат в секторе. </param>
        /// <returns> Возвращает массив координат из матрицы комнат. </returns>
        /// <remarks>
        /// Координаты не повторяются и их ровно roomCount. Эти 2 условия метод должен гарантировать.
        /// </remarks>
        IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount);

        /// <summary>
        /// Возвращает матрицу смежности между комнатами (сеть комнат).
        /// </summary>
        /// <param name="rooms"> Всё комнаты, которые должны быть соединены в сеть. </param>
        /// <param name="maxNeighbors"> Максимальное количество соседей у комнаты. </param>
        /// <returns>
        /// Возвращает словарь, представляющий собой матрицу смежности комнат.
        /// Минимальное число соседей - 1. Максимальное - не превышает указанное в аргументе значение.
        /// </returns>
        IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors);

        /// <summary>
        /// Выбрасывает случайный размер комнаты.
        /// </summary>
        /// <param name="minSize"> Минимальный размер комнаты. </param>
        /// <param name="maxSize"> Максимальный размер комнаты. </param>
        /// <returns> Возвращает размер с произвольными шириной и высотой в диапазоне (minSize, maxSize). </returns>
        /// <remarks>
        /// Источник рандома возвращает случайный размер комнаты в указанном диапазоне.
        /// </remarks>
        Size[] RollRoomSize(int minSize, int maxSize, int count);

        HexNode RollTransitionNode(IEnumerable<HexNode> openRoomNodes);
        IEnumerable<SectorTransition> RollTransitions(IEnumerable<SectorTransition> openTransitions);
    }
}