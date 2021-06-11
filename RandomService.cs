using System;

namespace async_request_processing
{
    public class RandomService
    {
        private readonly Random _random;

        public RandomService()
        {
            _random = new Random();
        }

        public int GetDelay() => _random.Next(3, 5);
    }
}