//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnsekDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class MeterReading
    {
        public int MeterReadingID { get; set; }
        public int AccountID { get; set; }
        public System.DateTime MeterReadingDateTime { get; set; }
        public string Value { get; set; }
    
        public virtual Account Account { get; set; }
    }
}
