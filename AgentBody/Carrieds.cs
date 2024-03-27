using Identification;
using Items;
using Saver;

namespace AgentBody
{
    public class Carrieds
    {
        public ITool? LeftHand { get; }
        public ITool? RightHand { get; }
        public IContainer? Back { get; }

        public IEnumerable<IItem> Everything
        {
            get
            {
                var everything = new List<IItem>();

                if(LeftHand is not null)
                    everything.Add(LeftHand);

                if(RightHand is not null)
                    everything.Add(RightHand);

                if(Back is not null)
                    everything.Add(Back);

                return everything;
            }
        }

        public Carrieds(ITool? leftHand, ITool? rightHand, IContainer? back)
        {
            LeftHand = leftHand;
            RightHand = rightHand;
            Back = back;
        }

        public bool IsEnlighted<T>(Repository<T> repository, bool isEnlightedOutside = false)
            where T : IItem, ISavable, ICloneable
        {
            if (LeftHand is not null && LeftHand is IEnlighted)
                return true;

            if (RightHand is not null && RightHand is IEnlighted)
                return true;

            if(Back is not null && Back.Inventory.IsEnlighted(repository, isEnlightedOutside))
                return true;

            return false;
        }
    }
}
