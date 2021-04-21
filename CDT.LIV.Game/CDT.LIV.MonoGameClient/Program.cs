using System;

namespace CDT.LIV.MonoGameClient
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new LivGame())
                game.Run();
        }
    }
}
