﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoPayment
    {
        public int paymentId {get;set;}
        public int?  supplierId {get;set;}
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string referenceNumber {get;set;}
        public DateTime? paymentDate {get;set;}
        public string TypeOfPayment { get; set; }
        public string ModeOfPayment { get; set; }
        public decimal? cashAmount {get;set;}
        public decimal? chequeAmount {get;set;}
        public string chequeNumber {get;set;}
        public DateTime? chequeDate {get;set;}
        public string chequeBank {get;set;}
        public decimal? totalPayment {get;set;}
        public int? modifiedBy { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int? createdBy { get; set; }
        public DateTime? dateCreated { get; set; }
        public decimal? runningBalance { get; set; }
        public decimal? remainingBalance { get; set; }

        public bool isNew { get; set; }
        public bool? isNewPayment { get; set; }
        public bool? isCash { get; set; }
        public decimal? excessPayment { get; set; }

        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int branchId { get; set; }
        public int records { get; set; }
    }
}
