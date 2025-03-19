using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemKoTrungUnit
{
    List<List<Transform>> listPoolGroup = new List<List<Transform>>();

    List<List<Transform>> listGroupTrung = new List<List<Transform>>();

    float kcX, kcY;

    public void UpdateCheckKoTrung(List<EnemyControl> listCheck)
    {
        //listCheck.Sort(SortByPosY);
        int length = listCheck.Count;
        if (length > 0)
        {
            CleartListTrung();
            for (int i = 0; i < length - 1; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    CheckCoTrungKo(listCheck[i], listCheck[j]);
                }
            }
        }
        XuLyKhongTrung();
    }

    // public void UpdateCheckKoTrung(List<HeroControl> listCheck)
    // {
    //     //listCheck.Sort(SortByPosY);
    //     int length = listCheck.Count;
    //     if (length > 0)
    //     {
    //         CleartListTrung();
    //         for (int i = 0; i < length - 1; i++)
    //         {
    //             for (int j = i + 1; j < length; j++)
    //             {
    //                 CheckCoTrungKo(listCheck[i], listCheck[j]);
    //             }
    //         }
    //     }
    //     XuLyKhongTrung();
    // }

    float kcXTrung = 1.2f;
    float kcYTrung = 0.8f;
    float kcXTrungStep = 0.1f;
    float kcYTrungStep = 0f;
    //truoc thap hon sau
    void CheckCoTrungKo(EnemyControl enemyTruoc, EnemyControl enemySau)
    {
        bool check = false;

        var trTruoc = enemyTruoc.transform;
        var trSau = enemySau.transform;
        kcX = trTruoc.position.x - trSau.position.x;
        kcX = kcX > 0 ? kcX : -kcX;
        if (kcX <= kcXTrung)
        {
            check = true;
        }
        if (check)
        {
            check = false;
            kcY = trTruoc.position.y - trSau.position.y;
            kcY = kcY > 0 ? kcY : -kcY;

            if (kcY <= kcYTrung)
            {
                check = true;
            }
        }

        if (check)
        {
            //trung nhau
            int length2 = listGroupTrung.Count;
            bool isHasGroup = false;
            if (length2 > 0)
            {
                // enemyTruoc.StopMove(0.4f);
                // enemySau.StopMove(0.4f);

                for (int i = 0; i < length2; i++)
                {
                    List<Transform> group2 = listGroupTrung[i];
                    int length3 = group2.Count;
                    if (group2.Contains(trTruoc))
                    {
                        isHasGroup = true;
                        if (group2.Contains(trSau))
                        {
                            //ca 2 cung trong list
                            break;
                        }
                        else
                        {
                            //trSau ko co trong list
                            group2.Add(trSau);
                            break;
                        }
                    }
                    else
                    {
                        //trTruoc ko co trong list
                        if (!group2.Contains(trSau))
                        {
                            //ca 2 cung ko co' trong list
                            break;
                        }
                        else
                        {
                            //trSau co trong list
                            isHasGroup = true;
                            group2.Add(trTruoc);
                            break;
                        }
                    }
                }
            }
            if (!isHasGroup)
            {
                List<Transform> group = GetNewGroup();
                group.Add(trTruoc);
                group.Add(trSau);
                listGroupTrung.Add(group);
            }
        }
    }

    // void CheckCoTrungKo(HeroControl heroTruoc, HeroControl heroSau)
    // {
    //     bool check = false;
    //
    //     var trTruoc = heroTruoc.transform;
    //     var trSau = heroSau.transform;
    //     kcX = trTruoc.position.x - trSau.position.x;
    //     kcX = kcX > 0 ? kcX : -kcX;
    //     if (kcX <= kcXTrung)
    //     {
    //         check = true;
    //     }
    //     if (check)
    //     {
    //         check = false;
    //         kcY = trTruoc.position.y - trSau.position.y;
    //         kcY = kcY > 0 ? kcY : -kcY;
    //
    //         if (kcY <= kcYTrung)
    //         {
    //             check = true;
    //         }
    //     }
    //
    //     if (check)
    //     {
    //         //trung nhau
    //         int length2 = listGroupTrung.Count;
    //         bool isHasGroup = false;
    //         if (length2 > 0)
    //         {
    //             heroTruoc.StopMove(0.4f);
    //             heroSau.StopMove(0.4f);
    //
    //             for (int i = 0; i < length2; i++)
    //             {
    //                 List<Transform> group2 = listGroupTrung[i];
    //                 int length3 = group2.Count;
    //                 if (group2.Contains(trTruoc))
    //                 {
    //                     isHasGroup = true;
    //                     if (group2.Contains(trSau))
    //                     {
    //                         //ca 2 cung trong list
    //                         break;
    //                     }
    //                     else
    //                     {
    //                         //trSau ko co trong list
    //                         group2.Add(trSau);
    //                         break;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     //trTruoc ko co trong list
    //                     if (!group2.Contains(trSau))
    //                     {
    //                         //ca 2 cung ko co' trong list
    //                         break;
    //                     }
    //                     else
    //                     {
    //                         //trSau co trong list
    //                         isHasGroup = true;
    //                         group2.Add(trTruoc);
    //                         break;
    //                     }
    //                 }
    //             }
    //         }
    //         if (!isHasGroup)
    //         {
    //             List<Transform> group = GetNewGroup();
    //             group.Add(trTruoc);
    //             group.Add(trSau);
    //             listGroupTrung.Add(group);
    //         }
    //     }
    // }

    void XuLyKhongTrung()
    {
        if (listGroupTrung.Count > 0)
        {
            int length = listGroupTrung.Count;
            for (int i = 0; i < length; i++)
            {
                KoTrung(listGroupTrung[i]);
            }
        }
    }

    void KoTrung(List<Transform> group)
    {
        group.Sort(SortByPosY);
        int length = group.Count;
        Vector2 centerPos = Vector2.zero;
        int i;
        for (i = 0; i < length; i++)
        {
            centerPos += (Vector2)group[i].position;
        }
        centerPos /= length;
        for (i = 0; i < length; i++)
        {
            var item = group[i];
            var posX = item.position.x;
            var posY = item.position.y;
            if (posY >= centerPos.y) item.Translate(kcXTrungStep * (posX - centerPos.x), kcYTrungStep * (posY - centerPos.y), 0f);
            else item.Translate(kcXTrungStep * (posX - centerPos.x), kcYTrungStep * 2f * (posY - centerPos.y), 0f);
        }
    }

    //----------------
    List<Transform> GetNewGroup()
    {
        List<Transform> newList = null;
        if (listPoolGroup.Count > 0)
        {
            newList = listPoolGroup[0];
            listPoolGroup.Remove(newList);
            newList.Clear();
        }
        else
        {
            newList = new List<Transform>();
        }
        return newList;
    }

    void CleartListTrung()
    {
        int length = listGroupTrung.Count;
        if (length > 0)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                listPoolGroup.Add(listGroupTrung[i]);
                listGroupTrung.RemoveAt(i);
            }
        }
    }
    //---------------
    static int SortByPosY(Transform tr1, Transform tr2)
    {
        return tr1.position.y.CompareTo(tr2.position.y);
    }
}
