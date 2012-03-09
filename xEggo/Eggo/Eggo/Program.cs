using System;

namespace Eggo
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Eggo game = Eggo.getInstance();
            using (game)
            {
                game.Run();
            }
        }
    }
#endif
}

