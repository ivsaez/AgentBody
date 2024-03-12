using Items;

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
    }
}
