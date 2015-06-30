﻿using Genesis.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoPayment : RepoBase, IPayment
    {


        public dtoPayment Get(dtoPayment t)
        {
            throw new NotImplementedException();
        }

        public List<dtoPayment> GetAll(string search, object filter = null)
        {
            throw new NotImplementedException();
        }

        public dtoResult Insert(dtoPayment t)
        {
            dtoResult objResult = new dtoResult();

            try
            {
                var objpayment = new tbl_payment
                {
                    supplierId = t.supplierId,
                    referenceNumber = t.referenceNumber,
                    TypeOfPayment = t.TypeOfPayment,
                    ModeOfPayment = t.ModeOfPayment,
                    paymentDate = DateTime.Now,
                    cashAmount = t.cashAmount,
                    chequeAmount = t.chequeAmount,
                    chequeNumber = t.chequeNumber,
                    chequeDate = t.chequeDate,
                    chequeBank = t.chequeBank,
                    totalPayment = t.totalPayment,
                    dateCreated = DateTime.Now,
                    createdBy = t.createdBy
                };
                DBContext.tbl_payment.Add(objpayment);

                DBContext.SaveChanges();
                t.paymentId = objpayment.paymentId;
                objResult.isSuccessful = true;
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            return objResult;
        }

        public dtoResult Insert(ref dtoPayment t)
        {
            dtoResult objResult = new dtoResult();

            try
            {
                var objpayment = new tbl_payment
                {
                    supplierId = t.supplierId,
                    referenceNumber = t.referenceNumber,
                    paymentDate = DateTime.Now,
                    cashAmount = t.cashAmount,
                    chequeAmount = t.chequeAmount,
                    chequeNumber = t.chequeNumber,
                    TypeOfPayment = t.TypeOfPayment,
                    ModeOfPayment = t.ModeOfPayment,
                    chequeDate = t.chequeDate,
                    chequeBank = t.chequeBank,
                    totalPayment = t.totalPayment,
                    dateCreated = DateTime.Now,
                    createdBy = t.createdBy
                };
                DBContext.tbl_payment.Add(objpayment);

                DBContext.SaveChanges();
                t.paymentId = objpayment.paymentId;
                objResult.isSuccessful = true;
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            return objResult;
        }

        public dtoResult Update(dtoPayment t)
        {
            dtoResult objResult = new dtoResult();
            try
            {
                var item = DBContext.tbl_payment.FirstOrDefault(d => d.paymentId == t.paymentId);
                item.supplierId = t.supplierId;
                item.referenceNumber = t.referenceNumber;
                item.paymentDate = t.paymentDate;
                item.TypeOfPayment = t.TypeOfPayment;
                item.ModeOfPayment = t.ModeOfPayment;
                item.cashAmount = t.cashAmount;
                item.chequeAmount = t.chequeAmount;
                item.chequeNumber = t.chequeNumber;
                item.chequeDate = t.chequeDate;
                item.chequeBank = t.chequeBank;
                item.totalPayment = t.totalPayment;

                DBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            return objResult;
        }

        public dtoResult SoftDelete(dtoPayment t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoPayment t)
        {
            throw new NotImplementedException();
        }

        public List<dtoPayment> GetPaymentsByFilters(dtoPayment filter)
        {
            var searchResults = (from i in DBContext.tbl_payment
                            join s in DBContext.tbl_supplier on i.supplierId equals s.supplierId
                            where (filter.paymentId == null || i.paymentId == filter.paymentId)
                            && (filter.supplierId == null || i.supplierId == filter.supplierId)
                            && (filter.supplierCode == null || s.supplierCode.Contains(filter.supplierCode))
                            && (filter.supplierName == null || s.supplierName.Contains(filter.supplierName))
                            && (filter.referenceNumber == null || i.referenceNumber.Contains(filter.referenceNumber))
                            && (filter.paymentDate == null || i.paymentDate == filter.paymentDate)
                            && (filter.cashAmount == null || i.cashAmount == filter.cashAmount)
                            && (filter.chequeAmount == null || i.chequeAmount == filter.chequeAmount)
                            && (filter.chequeNumber == null || i.chequeNumber.Contains(filter.chequeNumber))
                            && (filter.chequeDate == null || i.chequeDate == filter.chequeDate)
                            && (filter.chequeBank == null || i.chequeBank.Contains(filter.chequeBank))
                            && (filter.totalPayment == null || i.totalPayment == filter.totalPayment)
                            && (filter.DateFrom == null || i.dateCreated >= filter.DateFrom)
                            && (filter.DateTo == null || i.dateCreated <= filter.DateTo)
                            select new dtoPayment {
                                paymentId = i.paymentId,
                                supplierId = i.supplierId,
                                supplierCode = s.supplierCode,
                                supplierName = s.supplierName,
                                referenceNumber = i.referenceNumber,
                                TypeOfPayment = i.TypeOfPayment,
                                ModeOfPayment = i.ModeOfPayment,
                                paymentDate = i.paymentDate,
                                cashAmount = i.cashAmount,
                                chequeAmount = i.chequeAmount,
                                chequeNumber = i.chequeNumber,
                                chequeDate = i.chequeDate,
                                chequeBank = i.chequeBank,
                                totalPayment = i.totalPayment,
                                dateCreated = i.dateCreated
                            }).ToList();

            return searchResults;
        }

        public dtoResult SavePaymentTransaction(dtoPayment Header, List<dtoPaymentDetail> Details)
        {
            int? originalPaymentId = Header.paymentId;
            var respose = new dtoResult { isSuccessful = true };
            try
            {
                if (Header.paymentId == null || Header.paymentId == 0)
                {
                    Insert(ref Header);
                }
                else
                {
                    //Update Header
                    Update(Header);

                    //Delete Removed details.
                    var updatedIds = Details.Where(i => i.paymentDetailsId != null).Select(i => i.paymentDetailsId).ToList();
                    var deletedDetails = DBContext.tbl_paymentDetails.Where(l => l.paymentId == originalPaymentId && !updatedIds.Contains(l.paymentDetailsId));

                    foreach (var del in deletedDetails)
                    {
                        DBContext.tbl_paymentDetails.Remove(del);
                    }
                }

                foreach (var item in Details)
                {
                    if (originalPaymentId == null || originalPaymentId == 0)
                    {
                        var objDT = new tbl_paymentDetails
                        {
                            paymentId = Header.paymentId,
                            documentId = item.documentId,
                            paymentPrice = item.paymentPrice,
                            createdBy = 1,
                            dateCreated = DateTime.Now
                        };

                        DBContext.tbl_paymentDetails.Add(objDT);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        

                        if (item.paymentDetailsId != null || item.paymentDetailsId > 0)
                        {
                            var objDT = DBContext.tbl_paymentDetails.FirstOrDefault(d => d.paymentDetailsId == item.paymentDetailsId);
                            objDT.paymentId = Header.paymentId;
                            objDT.documentId = item.documentId;
                            objDT.paymentPrice = item.paymentPrice;
                            objDT.lastModifiedBy = 1;
                            objDT.dateLastModified = DateTime.Now;
                            DBContext.SaveChanges();
                        }
                        else
                        {
                            var objDT = new tbl_paymentDetails
                            {
                                paymentId = Header.paymentId,
                                documentId = item.documentId,
                                paymentPrice = item.paymentPrice,
                                createdBy = 1,
                                dateCreated = DateTime.Now
                            };

                            DBContext.tbl_paymentDetails.Add(objDT);
                            DBContext.SaveChanges();
                        }
                    }
                }

                DBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                respose.isSuccessful = false;
                respose.errorMsg = ex.Message;
            }

            return respose;
        }





        public List<dtoPaymentDetail> GetPaymentDetails(dtoPaymentDetail filter)
        {
            var returnItems = (from i in DBContext.tbl_paymentDetails
                               join j in DBContext.tbl_document on i.documentId equals j.documentId
                               where i.paymentId == filter.paymentId
                               select new dtoPaymentDetail
                               {
                                   documentId = i.documentId,
                                   documentNo = j.documentNumber,
                                   paymentDetailsId = i.paymentDetailsId,
                                   paymentId = i.paymentId,
                                   paymentPrice = i.paymentPrice,
                                   totalPaid = DBContext.tbl_paymentDetails.Where(d => d.documentId == i.documentId).Sum(e => e.paymentPrice)
                               }).ToList();

            foreach (var item in returnItems)
            {
                var details = DBContext.tbl_transaction.Where(x => x.documentId == item.documentId).ToList();
                item.totalAmount = details.Sum(y => (y.discountA != 0 || y.discountB != 0 || y.discountC != 0) ? ((((y.unitPrice * ((100M - (decimal)y.discountA) * 0.01M)) * ((100M - (decimal)y.discountB) * 0.01M)) * ((100M - (decimal)y.discountC) * 0.01M)) * y.quantity) : (y.unitPrice * y.quantity));
            }

            return returnItems;
        }
    }
}