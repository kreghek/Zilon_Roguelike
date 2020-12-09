namespace Zilon.Core.Tests.Common
{
    public static class TestSpecialConstants
    {
        /// <summary>
        /// Timeout for any not long operation in the tests.
        /// Timeout strongly recomended to restrict potentially long operation for which long execution
        /// is not expected and cause of sync error. It means the test hanged.
        /// </summary>
        public static int ShortOperationTimeoutMs => 1000;
    }
}
