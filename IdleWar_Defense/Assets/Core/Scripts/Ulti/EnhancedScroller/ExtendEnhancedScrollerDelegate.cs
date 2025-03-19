using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendEnhancedScrollerDelegate<T> : IEnhancedScrollerDelegate where T : EnhancedScrollerCellView
{

	public float CellSize;
	public T CellViewPrefab;
	public int NumberOfCells;

	public SetDataDelegate SetDataEvent;
	public delegate void SetDataDelegate(T cellView, int dataIndex);

	public void Init(float CellSize, int NumberOfCells, T CellPrefab, SetDataDelegate SetDataEvent)
	{
		this.CellSize = CellSize;
		this.NumberOfCells = NumberOfCells;
		this.CellViewPrefab = CellPrefab;
		this.SetDataEvent = SetDataEvent;
	}

	public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		T cellView = scroller.GetCellView(CellViewPrefab) as T;
		SetData(cellView, dataIndex);
		//    cellView.SetData(datas[dataIndex]);
		return cellView;
	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
	{
		return CellSize;
	}

	public int GetNumberOfCells(EnhancedScroller scroller)
	{
		return NumberOfCells;
	}

	void SetData(T cellView, int dataIndex)
	{
		if (SetDataEvent != null)
			SetDataEvent(cellView, dataIndex);
	}



}
