
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pattern.Data;

namespace FoodZombie
{
    public class ConsumableItemData : DataGroup, IComparable<ConsumableItemData>
    {
        public Action<int> onStockNumberChanged;
        public Action<int> onUsageNumberChanged;

        private IntegerData stockNumber;
        private IntegerData usageNumber;

        public int StockNumber => stockNumber.Value;
        public int UsageNumber => usageNumber.Value;

        public ConsumableItemData(int _id, int _stockNumber = 0) : base(_id)
        {
            stockNumber = AddData(new IntegerData(0, _stockNumber));
            usageNumber = AddData(new IntegerData(1));
        }

        public virtual bool CanUse(int _quantity = 1)
        {
            return stockNumber.Value >= _quantity;
        }

        public virtual void CountUsage(int _value)
        {
            usageNumber.Value+=(_value);
            if (onUsageNumberChanged != null)
                onUsageNumberChanged(_value);
        }

        public virtual void SetStock(int _value)
        {
            stockNumber.Value=(_value);
            if (onStockNumberChanged != null)
                onStockNumberChanged(_value);
        }

        public virtual void AddToStock(int _value)
        {
            stockNumber.Value+=(_value);
            if (onStockNumberChanged != null)
                onStockNumberChanged(_value);
        }

        public bool Use(int _value = 1)
        {
            if (CanUse(_value))
            {
                AddToStock(-_value);
                CountUsage(_value);
                return true;
            }
            return false;
        }

        public bool CanSell()
        {
            return stockNumber.Value > 0;
        }

        public int CompareTo(ConsumableItemData other)
        {
            return -stockNumber.Value.CompareTo(other.stockNumber.Value);
        }

        public virtual Sprite GetIcon() => null;
    }
}