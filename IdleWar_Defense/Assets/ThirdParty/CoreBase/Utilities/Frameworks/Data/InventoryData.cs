using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pattern.Data
{
    public interface IInventory
    {
        int Id { get; set; }
        int Quantity { get; set; }
    }

    public class InventoryData<T> : DataGroup where T : IInventory
    {
        private ListData<T> mItems;
        private IntegerData mLastItemId;

        public int Count => mItems.Count;
        public T this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        public InventoryData(int pId) : base(pId)
        {
            mItems = AddData(new ListData<T>(0));
            mLastItemId = AddData(new IntegerData(1));
        }

        public bool Insert(T pInvItem)
        {
            if (pInvItem.Id > 0)
            {
                for (int i = 0; i < mItems.Count; i++)
                    if (mItems[i].Id == pInvItem.Id)
                    {
                        Debug.LogError("Id of inventory item must be unique!");
                        return false;
                    }
            }
            else
                pInvItem.Id = GetNewItemId();

            mItems.Add(pInvItem);

            if (pInvItem.Id > mLastItemId.Value)
                mLastItemId.Value = pInvItem.Id;

            return true;
        }

        public bool Insert(List<T> pInvItems)
        {
            return Insert(pInvItems.ToArray());
        }

        public bool Insert(params T[] pInvItems)
        {
            int lastestId = mLastItemId.Value;
            for (int j = 0; j < pInvItems.Length; j++)
            {
                if (pInvItems[j].Id > 0)
                {
                    for (int i = 0; i < mItems.Count; i++)
                        if (mItems[i].Id == pInvItems[j].Id)
                        {
                            Debug.LogError("Id of inventory item must be unique!");
                            return false;
                        }
                }
                else
                {
                    lastestId += 1;
                    pInvItems[j].Id = lastestId;
                }
            }

            for (int j = 0; j < pInvItems.Length; j++)
                mItems.Add(pInvItems[j]);
            return true;
        }

        public bool Update(T pInvItem)
        {
            for (int i = 0; i < mItems.Count; i++)
                if (mItems[i].Id == pInvItem.Id)
                {
                    mItems[i] = pInvItem;
                    return true;
                }
            Debug.LogError("Could not update item, because Id is not found!");
            return false;
        }

        public bool Delete(T pInvItem)
        {
            for (int i = 0; i < mItems.Count; i++)
                if (mItems[i].Id == pInvItem.Id)
                {
                    mItems.Remove(mItems[i]);
                    return true;
                }
            Debug.LogError("Could not delete item, because Id is not found!");
            return false;
        }

        private int GetNewItemId()
        {
            return mLastItemId.Value += 1;
        }

        public T GetItem(int pIndex)
        {
            return mItems[pIndex];
        }
    }
}