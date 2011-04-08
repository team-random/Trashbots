using System;

namespace Trashbots1
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Trashbots game = new Trashbots())
            {
                game.Run();
            }
        }
    }
#endif
}

