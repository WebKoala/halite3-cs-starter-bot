using MyBot;

namespace Halite3
{
    public class Sharpie
    {
        private const string MyBotName = "Clarke3Bot";

        public static void Main(string[] args)
        {
            var gameEngine = new GameEngine(MyBotName, new RandomStrategy());

            gameEngine.Start();
        }
    }
}