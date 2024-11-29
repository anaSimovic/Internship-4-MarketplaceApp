namespace Marketplace.Utilities
{
    public static class IDGenerator
    {
        private static int _currentId = 0;

        public static int GenerateId() => ++_currentId;
    }
}
