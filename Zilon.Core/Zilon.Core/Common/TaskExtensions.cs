using System;
using System.Threading.Tasks;

namespace Zilon.Core.Common
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Выполняет задачу не дольше указанного времени.
        /// </summary>
        /// <typeparam name="TResult">Тип результата задачи.</typeparam>
        /// <param name="task">Выполняемая задача.</param>
        /// <param name="millisecondsTimeout">Время в миллисекундах, доступное для выполнения задачи.</param>
        /// <returns>Возвращает агрегирующую задачу для вычисления результата TResult.</returns>
        /// <exception cref="System.TimeoutException">
        /// Исключение выбрасывается, если задача выполняется дольше,
        /// чем указанный таймаут.
        /// </exception>
        public static Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return TimeoutAfterInner(task, millisecondsTimeout);
        }

        /// <summary>
        /// Выполняет задачу не дольше указанного времени.
        /// </summary>
        /// <param name="task">Выполняемая задача.</param>
        /// <param name="millisecondsTimeout">Время в миллисекундах, доступное для выполнения задачи.</param>
        /// <returns>Возвращает агрегирующую задачу для вычисления результата TResult.</returns>
        /// <exception cref="System.TimeoutException">
        /// Исключение выбрасывается, если задача выполняется дольше,
        /// чем указанный таймаут.
        /// </exception>
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return TimeoutAfterInner(task, millisecondsTimeout);
        }

        private static async Task<TResult> TimeoutAfterInner<TResult>(Task<TResult> task, int millisecondsTimeout)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsTimeout)).ConfigureAwait(false);
            if (task == completedTask)
            {
                return await task.ConfigureAwait(false);
            }

            throw new TimeoutException();
        }

        private static async Task TimeoutAfterInner(Task task, int millisecondsTimeout)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(millisecondsTimeout)).ConfigureAwait(false);
            if (task == completedTask)
            {
                await task.ConfigureAwait(false);
            }
            else
            {
                throw new TimeoutException();
            }
        }
    }
}