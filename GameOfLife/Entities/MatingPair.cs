namespace GameOfLife.Entities
{
    /// <summary>
    /// A class to form mating pairs for T animals.
    /// </summary>
    /// <typeparam name="T">The type of objects that can form mating pairs.</typeparam>
    public class MatingPair<T> where T : Animal
    {
        /// <summary>
        /// The first member of the mating pair.
        /// </summary>
        public T MatingPair1 { get; init; }

        /// <summary>
        /// The second member of the mating pair.
        /// </summary>
        public T MatingPair2 { get; init; }

        /// <summary>
        /// Initializes a new instance of the MatingPair class with the provided mating pair members.
        /// </summary>
        /// <param name="matingPair1">The first member of the mating pair.</param>
        /// <param name="matingPair2">The second member of the mating pair.</param>
        public MatingPair(T matingPair1, T matingPair2)
        {
            MatingPair1 = matingPair1;
            MatingPair2 = matingPair2;
        }

        public override string ToString()
        {
            return $"Mating Pair of {typeof(T).Name}:" +
                   $"\n\t\tPartner1: {MatingPair1}" +
                   $"\n\t\tPartner2: {MatingPair2}";
        }
    }
}