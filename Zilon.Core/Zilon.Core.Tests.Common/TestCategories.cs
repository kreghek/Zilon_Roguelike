namespace Zilon.Core.Tests.Common
{
    public static class TestCategories
    {
        /// <summary>
        /// Это категория тестов, выполнение которых занимает много времени.
        /// Их не требуется запускать максимально часто.
        /// Достаточно выполнять перед МР и на билд-сервере.
        /// </summary>
        public const string LONG_RUN = "long-run";

        /// <summary>
        /// Это категория тестов, которые опираются на реальные ресурсы,
        /// а не на подготовленные специально для теста данные.
        /// К таким ресурсам относятся сейчас только схемы игровых сущностей.
        /// </summary>
        public const string REAL_RESOURCE = "real-resource";
    }
}
