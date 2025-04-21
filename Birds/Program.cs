using System;

namespace Birds
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Birds.src.Game1();
            game.Run();
        }
    }
}