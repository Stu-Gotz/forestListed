﻿using System;
using System.Collections.Generic;

namespace forestListed
{
    /* 
     * Credit to user Viktor Lova @ Stackoverflow:
       https://stackoverflow.com/a/17275935/11790979
    */
    class SameTuplesComparer<T1, T2> : EqualityComparer<Tuple<T1, T2>>
    {
        public override bool Equals(Tuple<T1, T2> t1, Tuple<T1, T2> t2)
        {
            return t1.Item1.Equals(t2.Item1) && t1.Item2.Equals(t2.Item2);
        }


        public override int GetHashCode(Tuple<T1, T2> t)
        {
            return base.GetHashCode();
        }
    }
}