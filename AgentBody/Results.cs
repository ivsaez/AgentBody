using Items;

namespace AgentBody
{
    public abstract class AbstractAdditionResult
    {
        private ItemAddition? addition;

        protected AbstractAdditionResult(ItemAddition? addition)
        {
            this.addition = addition;
        }

        public ItemAddition Addition
        {
            get
            {
                if (addition is null)
                    throw new InvalidOperationException("Cannot request addition.");

                return addition.Value;
            }
        }
    }

    public class CarryingResult: AbstractAdditionResult
    {

        private CarryingResult(bool occupied, ItemAddition? addition)
            : base(addition)
        {
            Occupied = occupied;
        }

        public bool Occupied { get; }

        public static CarryingResult Free(ItemAddition addition) =>
            new CarryingResult(true, addition);

        public static CarryingResult Full() =>
            new CarryingResult(false, null);
    }

    public class BaggingResult : AbstractAdditionResult
    {

        private BaggingResult(bool hasBag, ItemAddition? addition)
            : base(addition)
        {
            HasBag = hasBag;
        }

        public bool HasBag { get; }

        public static BaggingResult InBag(ItemAddition addition) =>
            new BaggingResult(true, addition);

        public static BaggingResult WithoutBag() =>
            new BaggingResult(false, null);
    }
}
