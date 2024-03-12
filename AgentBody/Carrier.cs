using Identification;
using Items;
using Items.Extensions;
using Saver;

namespace AgentBody
{
    public class Carrier : ISavable, ICloneable
    {
        private uint capacity;
        private uint weight;

        private string? leftHand;
        private string? rightHand;
        private string? back;

        public Carrier(uint capacity, uint weight)
        {
            this.capacity = capacity;
            this.weight = weight;
        }

        public Carrieds GetCarrieds<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            new Carrieds(
                leftHand is null ? null : (ITool)repository.GetOne(leftHand),
                rightHand is null ? null : (ITool)repository.GetOne(rightHand),
                back is null ? null : (IContainer)repository.GetOne(back));

        public BaggingResult Take<T>(IItem item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (back is null)
                return BaggingResult.WithoutBag();

            var bag = repository.GetOne(back);
            var addition = bag.Cast<IContainer>().Inventory.Add(item, repository);

            return BaggingResult.InBag(addition);
        }

        public CarryingResult SetInHand<T>(ITool item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (leftHand is not null && rightHand is not null)
                return CarryingResult.Full();

            var addition = checkAdditionCapabilities(item, repository);

            if (addition == ItemAddition.Good)
            {
                if (leftHand is null)
                    leftHand = item.Id;
                else
                    rightHand = item.Id;
            }

            return CarryingResult.Free(addition);
        }

        public CarryingResult SetLeftHand<T>(ITool item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (leftHand is not null)
                return CarryingResult.Full();

            var addition = checkAdditionCapabilities(item, repository);

            if (addition == ItemAddition.Good)
                leftHand = item.Id;

            return CarryingResult.Free(addition);
        }

        public CarryingResult SetRightHand<T>(ITool item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (rightHand is not null)
                return CarryingResult.Full();

            var addition = checkAdditionCapabilities(item, repository);

            if (addition == ItemAddition.Good)
                rightHand = item.Id;

            return CarryingResult.Free(addition);
        }

        public CarryingResult SetBack<T>(IContainer item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (back is not null)
                return CarryingResult.Full();

            var addition = checkAdditionCapabilities(item, repository);

            if (addition == ItemAddition.Good)
                back = item.Id;

            return CarryingResult.Free(addition);
        }

        public bool Leave<T>(IItem item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if(leftHand is not null && leftHand == item.Id)
            {
                leftHand = null;
                return true;
            }

            if (rightHand is not null && rightHand == item.Id)
            {
                rightHand = null;
                return true;
            }

            if(back is not null)
            {
                var bag = repository.GetOne(back);
                return bag.Cast<IContainer>().Inventory.Remove(item);
            }

            return false;
        }

        public IItem? RemoveLeftHand<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (leftHand is null)
                return null;

            var item = repository.GetOne(leftHand);
            leftHand = null;

            return item;
        }

        public IItem? RemoveRightHand<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (rightHand is null)
                return null;

            var item = repository.GetOne(rightHand);
            rightHand = null;

            return item;
        }

        public IItem? RemoveBack<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (back is null)
                return null;

            var item = repository.GetOne(back);
            back = null;

            return item;
        }

        private ItemAddition checkAdditionCapabilities<T>(IItem item, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            var actualWeight = getLeftHandWeight(repository) + getRightHandWeight(repository) + getBackWeight(repository);
            var actualSpace = getLeftHandSpace(repository) + getRightHandSpace(repository) + getBackSpace(repository);

            uint itemWeight = item.Weight;
            uint itemSpace = item.Space;

            if(item is IContainer)
            {
                itemWeight += item.Cast<IContainer>().Inventory.Weight(repository);
                itemSpace += item.Cast<IContainer>().Inventory.Space(repository);
            }

            if (itemWeight + actualWeight > weight)
                return ItemAddition.Heavy;

            if(itemSpace + actualSpace > capacity) 
                return ItemAddition.Big;

            return ItemAddition.Good;
        }

        private uint getLeftHandSpace<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            getCarrierItemSpace(leftHand, repository);

        private uint getRightHandSpace<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            getCarrierItemSpace(rightHand, repository);

        private uint getBackSpace<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            getCarrierItemSpace(back, repository);

        private uint getLeftHandWeight<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            getCarrierItemWeight(leftHand, repository);

        private uint getRightHandWeight<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            getCarrierItemWeight(rightHand, repository);

        private uint getBackWeight<T>(Repository<T> repository)
            where T : IItem, ISavable, ICloneable =>
            getCarrierItemWeight(back, repository);

        private static uint getCarrierItemSpace<T>(string? carrierItem, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (carrierItem is null) return 0;

            var item = repository.GetOne(carrierItem);
            uint total = item.Space;

            if (item is IContainer)
                total += item.Cast<IContainer>().Inventory.Space(repository);

            return total;
        }

        private static uint getCarrierItemWeight<T>(string? carrierItem, Repository<T> repository)
            where T : IItem, ISavable, ICloneable
        {
            if (carrierItem is null) return 0;

            var item = repository.GetOne(carrierItem);
            uint total = item.Weight;

            if (item is IContainer)
                total += item.Cast<IContainer>().Inventory.Weight(repository);

            return total;
        }

        public object Clone()
        {
            var clone = new Carrier(capacity, weight);

            clone.leftHand = leftHand;
            clone.rightHand = rightHand;
            clone.back = back;

            return clone;
        }

        public void Load(Save save)
        {
            capacity = save.GetUInt(nameof(capacity));
            weight = save.GetUInt(nameof(weight));

            if(save.Has(nameof(leftHand)))
                leftHand = save.GetString(nameof(leftHand));

            if (save.Has(nameof(rightHand)))
                rightHand = save.GetString(nameof(rightHand));

            if (save.Has(nameof(back)))
                back = save.GetString(nameof(back));
        }

        public Save ToSave()
        {
            var save = new Save(GetType().Name)
                .With(nameof(capacity), capacity)
                .With(nameof(weight), weight);

            if (leftHand is not null)
                save.With(nameof(leftHand), leftHand);

            if (rightHand is not null)
                save.With(nameof(rightHand), rightHand);

            if(back is not null)
                save.With(nameof(back), back);

            return save;
        }
    }
}