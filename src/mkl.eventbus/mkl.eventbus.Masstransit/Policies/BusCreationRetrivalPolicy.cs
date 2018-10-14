using mkl.eventbus.Abstractions;

namespace mkl.eventbus.Masstransit.Policies
{
    public class BusCreationRetrivalPolicy : IRetrivalPolicy
    {
        public int NumbersOfRetrival { get; }
        public BusCreationRetrivalPolicy(int numbersOfRetrival)
        {
            NumbersOfRetrival = numbersOfRetrival;
        }

        public static BusCreationRetrivalPolicy Default => new BusCreationRetrivalPolicy(3);
    }
}
