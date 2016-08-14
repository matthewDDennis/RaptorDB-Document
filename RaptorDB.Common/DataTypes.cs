﻿using System;
using System.Collections.Generic;

namespace RaptorDB
{
    /// <summary>
    /// Result of queries
    ///    OK : T = Query with data,  F = EX has the exception
    ///    Rows : query rows
    /// </summary>
    public class Result<T>
    {
        public Result()
        {

        }
        public Result(bool ok)
        {
            OK = ok;
        }
        public Result(bool ok, Exception ex)
        {
            OK = ok;
            EX = ex;
        }
        /// <summary>
        /// T=Values return, F=exceptions occurred 
        /// </summary>
        public bool OK { get; set; }
        public Exception EX { get; set; }
        /// <summary>
        /// Total number of rows of the query
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// Rows returned
        /// </summary>
        public int Count { get; set; }
        public List<T> Rows { get; set; }

        public string Title { get; set; }
        // FEATURE : data pending in results
        ///// <summary>
        ///// Data is being indexed, so results will not reflect all documents
        ///// </summary>
        //public bool DataPending { get; set; }
    }

    /// <summary>
    /// Base for row schemas : implements a docid property and is bindable
    /// </summary>
    public abstract class RDBSchema : BindableFields
    {
        public Guid docid;
    }
}

// no namespace -> available to all
public static class RDBExtension
{
    /// <summary>
    /// RDB Between checking for dates
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromdate">Must be yyyy-mm-dd</param>
    /// <param name="todate">Must be yyyy-mm-dd</param>
    /// <returns></returns>
    public static bool Between(this DateTime value, string fromdate, string todate)
    {
        return true;
    }

    /// <summary>
    /// RDB Between checking for dates
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromdate"></param>
    /// <param name="todate"></param>
    /// <returns></returns>
    public static bool Between(this DateTime value, DateTime fromdate, DateTime todate)
    {
        return true;
    }

    public static bool Between(this int value, int from, int to)
    {
        return true;
    }

    public static bool Between(this long value, long from, long to)
    {
        return true;
    }

    public static bool Between(this decimal value, decimal from, decimal to)
    {
        return true;
    }
}
